using System;

namespace FFXIV_ACT_Plugin.Network;

/// <summary>
/// Reverses the packet obfuscation the game client applies to a subset of the
/// server's IPC packets.
/// </summary>
public interface INetworkPacketUnscrambler : IDisposable
{
	/// <summary>
	/// True when the client version was recognised, its key derivation tables
	/// could be read and obfuscation keys have been received.
	/// </summary>
	bool Enabled { get; }

	/// <summary>
	/// Client version the tables were loaded for, or an empty string.
	/// </summary>
	string GameVersion { get; }

	/// <summary>
	/// Human readable reason why unscrambling is unavailable, empty when it is
	/// working.  Surfaced by the settings window's problem diagnosis.
	/// </summary>
	string LastError { get; }

	/// <summary>
	/// Loads the key derivation tables for the client at
	/// <paramref name="gameExecutablePath"/>.  Returns false (and sets
	/// <see cref="LastError"/>) when deobfuscation is not possible.
	/// </summary>
	bool Initialize(string gameExecutablePath);

	/// <summary>
	/// Deobfuscates <paramref name="packet"/> in place.  The array must start at
	/// the IPC header of a packet received from the server.  Returns true when
	/// the packet was obfuscated and has been rewritten.
	/// </summary>
	bool TryUnscramble(byte[] packet);
}
