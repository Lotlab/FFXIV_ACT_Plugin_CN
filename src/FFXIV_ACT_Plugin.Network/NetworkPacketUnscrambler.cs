#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Unscrambler;
using Unscrambler.Constants;
using Unscrambler.Derivation;
using Unscrambler.Unscramble;

namespace FFXIV_ACT_Plugin.Network;

/// <summary>
/// Deobfuscates the server packets the game client scrambles, using
/// https://github.com/perchbirdd/Unscrambler.
///
/// Since 7.2 the client obfuscates a fixed set of opcodes with keys the server
/// hands out in a zone-down packet.  A packet capture therefore sees scrambled
/// data where a hooking capture tool would see plaintext; this class reverses
/// that so the packet handlers keep working without injecting anything into the
/// game.
///
/// The keys are derived with the tables the client carries in its executable.
/// The Unscrambler project publishes per-version table blobs, but only for the
/// global client - the CN client is a separate build with its own .rdata
/// layout, so the tables are read out of the client on disk instead, using the
/// offsets from <see cref="VersionConstants"/>.  This mirrors what upstream's
/// DataGenerator does at release time.
/// </summary>
public sealed class NetworkPacketUnscrambler : INetworkPacketUnscrambler
{
	/// <summary>
	/// Upper bound of any obfuscated packet.  Packets are unscrambled in a
	/// padded copy of this size because the Unscrambler writes to fixed offsets
	/// that assume a full sized packet.
	/// </summary>
	private const int MaximumPacketSize = 4096;

	private static readonly byte[] EmptyKeys = new byte[3];

	private IKeyGenerator _keyGenerator;
	private IUnscrambler _unscrambler;
	private VersionConstants _constants;
	private HashSet<int> _obfuscatedOpcodes;
	private bool _initZoneUnsupported;
	private bool _disposed;
	private string _initializedFor;

	public bool Enabled { get; private set; }

	public string GameVersion { get; private set; } = string.Empty;

	public string LastError { get; private set; } = string.Empty;

	public bool Initialize(string gameExecutablePath)
	{
		// Reading the tables means loading the whole client executable; skip the
		// work when the capture is restarted for the same client.
		if (Enabled && !string.IsNullOrEmpty(_initializedFor) && string.Equals(_initializedFor, gameExecutablePath, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		Enabled = false;
		GameVersion = string.Empty;
		LastError = string.Empty;
		_initializedFor = null;

		try
		{
			if (string.IsNullOrWhiteSpace(gameExecutablePath) || !File.Exists(gameExecutablePath))
			{
				LastError = "找不到游戏主程序，无法读取反混淆所需的密钥表。";
				return false;
			}

			if (!TryResolveGameVersion(gameExecutablePath, out string version))
			{
				LastError = "无法从 ffxivgame.ver 读取游戏版本。";
				return false;
			}
			GameVersion = version;

			if (!VersionConstants.Constants.TryGetValue(version, out VersionConstants constants))
			{
				LastError = $"Unscrambler 尚未收录该客户端版本（{version}），暂不进行反混淆。";
				return false;
			}

			byte[] image = File.ReadAllBytes(gameExecutablePath);
			PortableExecutable pe = new PortableExecutable(image);

			byte[] table0 = pe.ReadBytes((int)constants.TableOffsets[0], constants.TableSizes[0]);
			byte[] table1 = pe.ReadBytes((int)constants.TableOffsets[1], constants.TableSizes[1]);
			byte[] table2 = pe.ReadBytes((int)constants.TableOffsets[2], constants.TableSizes[2]);
			byte[] midTable = pe.ReadBytes((int)constants.MidTableOffset, constants.MidTableSize);
			byte[] dayTable = pe.ReadBytes((int)constants.DayTableOffset, constants.DayTableSize);
			byte[] opcodeKeyTable = pe.ReadBytes((int)constants.OpcodeKeyTableOffset, constants.OpcodeKeyTableSize);

			_constants = constants;
			_obfuscatedOpcodes = new HashSet<int>(constants.ObfuscatedOpcodes.Values);
			_keyGenerator = KeyGeneratorFactory.WithConstants(constants, table0, table1, table2, midTable, dayTable, opcodeKeyTable);
			_unscrambler = UnscramblerFactory.ForGameVersion(constants.GameVersion);
			_initZoneUnsupported = false;
			// Key generation happens on the first obfuscation-initialising packet.
			Enabled = true;
			_initializedFor = gameExecutablePath;
			Trace.WriteLine($"Unscrambler initialised for client version {version}.", "FFXIV_ACT_Plugin");
			return true;
		}
		catch (Exception ex)
		{
			Enabled = false;
			LastError = "反混淆初始化失败：" + ex.Message;
			Trace.WriteLine("Unable to initialise packet deobfuscation. " + ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
			return false;
		}
	}

	public bool TryUnscramble(byte[] packet)
	{
		if (_disposed || !Enabled || packet == null || packet.Length < 4)
		{
			return false;
		}

		try
		{
			int opcode = OpcodeUtility.GetOpcodeFromPacketAtIpcStart(packet);

			// The obfuscation initialiser is itself plaintext: it carries the
			// seeds the keys are derived from.
			if (opcode == _constants.UnknownObfuscationInitOpcode)
			{
				_keyGenerator.GenerateFromUnknownInitializer(packet);
				return false;
			}

			if (!_initZoneUnsupported && opcode == _constants.InitZoneOpcode)
			{
				try
				{
					_keyGenerator.GenerateFromInitZone(packet);
				}
				catch (NotImplementedException)
				{
					// 7.4 and later only initialise from the packet above.
					_initZoneUnsupported = true;
				}
				return false;
			}

			if (!_obfuscatedOpcodes.Contains(opcode) || !_keyGenerator.ObfuscationEnabled)
			{
				return false;
			}

			byte[] keys = _keyGenerator.Keys ?? EmptyKeys;
			if (keys.Length < 3)
			{
				return false;
			}
			if (keys[0] == 0 && keys[1] == 0 && keys[2] == 0)
			{
				return false;
			}

			// Unscramble a padded copy: the library writes to fixed offsets that
			// assume a full sized packet, so operating on a short array in place
			// would run past its end.
			int size = Math.Max(packet.Length, MaximumPacketSize);
			byte[] scratch = new byte[size];
			Buffer.BlockCopy(packet, 0, scratch, 0, packet.Length);
			_unscrambler.Unscramble(scratch, keys[0], keys[1], keys[2], _keyGenerator.GetOpcodeBasedKey(opcode));
			Buffer.BlockCopy(scratch, 0, packet, 0, packet.Length);
			return true;
		}
		catch (Exception ex)
		{
			// A failure here must never take the parser down.
			Trace.WriteLine("Unable to unscramble packet. " + ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
			LastError = "反混淆数据包时发生错误：" + ex.Message;
			return false;
		}
	}

	public void Dispose()
	{
		_disposed = true;
		Enabled = false;
		_keyGenerator = null;
		_unscrambler = null;
	}

	/// <summary>
	/// Reads the client version from ffxivgame.ver next to the executable, the
	/// same file upstream's DataGenerator keys its constants off.
	/// </summary>
	private static bool TryResolveGameVersion(string gameExecutablePath, out string version)
	{
		version = string.Empty;
		string directory = Path.GetDirectoryName(Path.GetFullPath(gameExecutablePath));
		while (!string.IsNullOrEmpty(directory))
		{
			string candidate = Path.Combine(directory, "ffxivgame.ver");
			if (File.Exists(candidate))
			{
				string text = File.ReadAllText(candidate).Trim().Trim('\uFEFF');
				if (text.Length > 0)
				{
					version = text;
					return true;
				}
			}
			directory = Path.GetDirectoryName(directory);
		}
		return false;
	}
}
