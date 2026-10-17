using System;

namespace FFXIV_ACT_Plugin.Network;

/// <summary>
/// Reverses the obfuscation that the game client applies to a subset of the
/// server IPC packets.  Only plain packet capture needs this: Deucalion reads
/// the traffic after the client has already deobfuscated it.
/// </summary>
public interface INetworkPacketUnscrambler : IDisposable
{
	/// <summary>
	/// True once the client version was recognised and its key derivation
	/// tables were loaded.  Keys still have to be received from the server
	/// before any packet can be unscrambled.
	/// </summary>
	bool Enabled { get; }

	/// <summary>
	/// Client version the tables were loaded for, empty when not initialised.
	/// </summary>
	string GameVersion { get; }

	/// <summary>
	/// Why unscrambling is unavailable, empty while it is working.  Shown by
	/// the settings window's connection diagnosis.
	/// </summary>
	string LastError { get; }

	/// <summary>
	/// Loads the constants and key derivation tables for the client executable
	/// at <paramref name="gameExecutablePath"/>.  Returns false and sets
	/// <see cref="LastError"/> when deobfuscation cannot be supported.
	/// </summary>
	bool Initialize(string gameExecutablePath);

	/// <summary>
	/// Deobfuscates one packet in place.  <paramref name="packet"/> must start
	/// at the IPC header.  Returns true when the packet was obfuscated and has
	/// been rewritten; false for plaintext/unknown packets.
	/// </summary>
	bool TryUnscramble(byte[] packet);
}
