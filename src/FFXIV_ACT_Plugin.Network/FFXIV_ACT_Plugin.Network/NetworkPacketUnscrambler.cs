#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Unscrambler;
using Unscrambler.Constants;
using Unscrambler.Derivation;
using Unscrambler.Derivation.Versions;
using Unscrambler.Unscramble;

namespace FFXIV_ACT_Plugin.Network;

/// <summary>
/// Deobfuscates the server packets that the FFXIV client scrambles, using
/// https://github.com/perchbirdd/Unscrambler with the CN constants and tables
/// from https://github.com/Latihas/Unscrambler.
///
/// The client obfuscates a fixed set of server opcodes with keys that the
/// server hands out in a zone packet.  A packet capture sees scrambled values
/// where a hooking capture tool sees plaintext; this class reverses that, so the
/// packet handlers and the log/overlay subscribers keep working without
/// injecting anything into the game.
///
/// The Latihas fork carries the CN constants and embeds the six key derivation
/// tables as resources.  It deliberately omits the key derivation algorithm,
/// because its in-process consumer (IINACT) reads the live keys out of the game
/// process; an external capture has no such keys, so the client-independent
/// Derivation code from upstream perchbirdd/Unscrambler is compiled alongside
/// it and derives the keys from the server's initialiser packet.
/// </summary>
public sealed class NetworkPacketUnscrambler : INetworkPacketUnscrambler
{
	/// <summary>
	/// The highest fixed offset the library writes to belongs to UpdateParty
	/// (party slot 8), at roughly 3.3 KB.  Unscrambling therefore happens in a
	/// padded copy of at least this size, because a short packet would otherwise
	/// be written past its end.
	/// </summary>
	private const int MinimumScratchSize = 4096;

	/// <summary>7.30 added the opcode-dependent key and the Unscrambler73 layout.</summary>
	private static readonly DateTime Unscrambler73Version = new DateTime(2025, 7, 30);

	/// <summary>7.40 changed the initialiser packet layout, so the derivation moved to KeyGenerator74.</summary>
	private static readonly DateTime KeyGenerator74Version = new DateTime(2025, 12, 9);

	private readonly object _sync = new object();

	private IKeyGenerator _keyGenerator;
	private IUnscrambler _unscrambler;
	private VersionConstants _constants;
	private HashSet<int> _obfuscatedOpcodes;

	private bool _useOpcodeKey;
	private bool _initZoneUnsupported;
	private byte[] _scratch;

	private bool _disposed;

	public bool Enabled { get; private set; }

	public string GameVersion { get; private set; } = string.Empty;

	public string LastError { get; private set; } = string.Empty;

	public bool Initialize(string gameExecutablePath)
	{
		lock (_sync)
		{
			if (_disposed)
			{
				return false;
			}

			try
			{
				if (string.IsNullOrWhiteSpace(gameExecutablePath) || !File.Exists(gameExecutablePath))
				{
					ResetState();
					return Fail("找不到游戏主程序，无法读取 ffxivgame.ver 中的客户端版本。");
				}

				if (!TryReadGameVersion(gameExecutablePath, out string version))
				{
					ResetState();
					return Fail("无法从 ffxivgame.ver 读取游戏版本，反混淆未启用。");
				}

				if (Enabled && string.Equals(version, GameVersion, StringComparison.Ordinal))
				{
					return true;
				}

				ResetState();
				LastError = string.Empty;
				GameVersion = version;

				if (!VersionConstants.Constants.TryGetValue(version, out VersionConstants constants))
				{
					return Fail($"当前 Unscrambler CN 数据尚未收录客户端版本 {version}，反混淆未启用。");
				}

				try
				{
					_keyGenerator = CreateKeyGenerator(constants);
					_unscrambler = UnscramblerFactory.ForGameVersion(version);
				}
				catch (Exception ex)
				{
					return Fail($"客户端版本 {version} 的密钥表不可用：{ex.Message}");
				}

				_constants = constants;
				_obfuscatedOpcodes = new HashSet<int>(constants.ObfuscatedOpcodes.Values);
				_useOpcodeKey = ParseClientDate(version) >= Unscrambler73Version;
				_initZoneUnsupported = false;
				Enabled = true;

				Trace.WriteLine($"Packet deobfuscation ready for client {version} (Unscrambler CN).", "FFXIV_ACT_Plugin");
				return true;
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Unable to initialise packet deobfuscation. " +
					ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
				return Fail("反混淆初始化失败：" + ex.Message);
			}
		}
	}

	public bool TryUnscramble(byte[] packet)
	{
		if (_disposed || !Enabled || packet == null || packet.Length < 4)
		{
			return false;
		}

		lock (_sync)
		{
			try
			{
				ushort opcode = OpcodeUtility.GetOpcodeFromPacketAtIpcStart(packet);

				// The initialiser packets are plaintext: they carry the seeds the
				// packet keys are derived from.  New keys take effect for the
				// packets that follow, not for the initialiser itself.
				if (_constants.UnknownObfuscationInitOpcode != 0 &&
					opcode == _constants.UnknownObfuscationInitOpcode)
				{
					if (packet.Length >= 33)
					{
						_keyGenerator.GenerateFromUnknownInitializer(packet);
					}
					return false;
				}

				if (!_initZoneUnsupported && _constants.InitZoneOpcode != 0 && opcode == _constants.InitZoneOpcode)
				{
					if (packet.Length >= 45)
					{
						try
						{
							_keyGenerator.GenerateFromInitZone(packet);
						}
						catch (NotImplementedException)
						{
							// 7.4 and later no longer put the seeds in InitZone.
							_initZoneUnsupported = true;
						}
					}
					return false;
				}

				if (_obfuscatedOpcodes == null || !_obfuscatedOpcodes.Contains(opcode) ||
					!_keyGenerator.ObfuscationEnabled)
				{
					return false;
				}

				byte[] keys = _keyGenerator.Keys;
				if (keys == null || keys.Length < 3 ||
					(keys[0] == 0 && keys[1] == 0 && keys[2] == 0))
				{
					// The server has not sent obfuscation keys yet.
					return false;
				}

				byte[] scratch = GetScratchBuffer(packet.Length);
				Buffer.BlockCopy(packet, 0, scratch, 0, packet.Length);

				if (_useOpcodeKey)
				{
					int opcodeKey = _keyGenerator.GetOpcodeBasedKey(opcode);
					_unscrambler.Unscramble(scratch, keys[0], keys[1], keys[2], opcodeKey);
				}
				else
				{
					_unscrambler.Unscramble(scratch, keys[0], keys[1], keys[2]);
				}

				Buffer.BlockCopy(scratch, 0, packet, 0, packet.Length);
				return true;
			}
			catch (Exception ex)
			{
				// A single malformed packet must not stop the capture.
				LastError = "反混淆数据包失败：" + ex.Message;
				Trace.WriteLine("Unable to unscramble a packet. " +
					ex.ToString().Replace(Environment.NewLine, " "), "FFXIV_ACT_Plugin");
				return false;
			}
		}
	}

	public void Dispose()
	{
		lock (_sync)
		{
			_disposed = true;
			ResetState();
		}
	}

	private bool Fail(string message)
	{
		Enabled = false;
		LastError = message;
		return false;
	}

	private void ResetState()
	{
		Enabled = false;
		GameVersion = string.Empty;
		_keyGenerator = null;
		_unscrambler = null;
		_constants = null;
		_obfuscatedOpcodes = null;
		_useOpcodeKey = false;
		_initZoneUnsupported = false;
		_scratch = null;
	}

	private byte[] GetScratchBuffer(int packetLength)
	{
		int needed = Math.Max(packetLength, MinimumScratchSize);
		if (_scratch == null || _scratch.Length < needed)
		{
			_scratch = new byte[needed];
		}
		return _scratch;
	}

	/// <summary>
	/// Creates the key generator for the given client constants.
	///
	/// The upstream <c>KeyGeneratorFactory</c> keeps a hard-coded table of client versions,
	/// which has to be extended in the submodule for every patch (and hotfix) before the plugin
	/// can use its data.  The layout only changed twice, though - 7.3 added the opcode-based
	/// key and 7.4 changed the initialiser packet - so pick by client date and let a new patch
	/// reuse the current layout until the derivation itself changes.
	/// </summary>
	private static IKeyGenerator CreateKeyGenerator(VersionConstants constants)
	{
		DateTime date = ParseClientDate(constants.GameVersion);

		IKeyGenerator generator =
			date >= KeyGenerator74Version ? (IKeyGenerator)new KeyGenerator74() :
			date >= Unscrambler73Version ? new KeyGenerator73() :
			new KeyGenerator72();

		generator.Initialize(constants);
		return generator;
	}

	private static DateTime ParseClientDate(string version)
	{
		if (version.Length >= 10 &&
			DateTime.TryParseExact(version.Substring(0, 10), "yyyy.MM.dd",
				CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
		{
			return result;
		}

		// Unknown format: assume a modern client rather than a 7.2 layout.
		return DateTime.MaxValue;
	}

	/// <summary>
	/// Finds ffxivgame.ver next to the executable, or in one of its parent
	/// directories (the CN launcher has kept it in both places over time).
	/// </summary>
	private static bool TryReadGameVersion(string gameExecutablePath, out string version)
	{
		version = string.Empty;
		string directory = Path.GetDirectoryName(Path.GetFullPath(gameExecutablePath));
		while (!string.IsNullOrEmpty(directory))
		{
			string candidate = Path.Combine(directory, "ffxivgame.ver");
			if (File.Exists(candidate))
			{
				string text = File.ReadAllText(candidate).Trim();
				// A UTF-8 BOM survives Trim().
				text = text.Trim('\uFEFF').Trim();
				if (text.Length > 0)
				{
					version = text;
					return true;
				}
			}

			DirectoryInfo parent = Directory.GetParent(directory);
			directory = parent?.FullName;
		}

		return false;
	}
}
