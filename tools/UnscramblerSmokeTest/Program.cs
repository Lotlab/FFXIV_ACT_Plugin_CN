using System;
using System.IO;
using FFXIV_ACT_Plugin.Network;
using Unscrambler.Constants;

namespace UnscramblerSmokeTest
{
    /// <summary>
    /// Drives <see cref="NetworkPacketUnscrambler"/> through the same steps the
    /// plugin uses at capture start.
    ///
    /// The CN constants and the six key derivation tables now come from the
    /// Latihas/Unscrambler submodule (embedded resources), so no game executable
    /// is needed: only an ffxivgame.ver telling the code which client version it
    /// is looking at.
    /// </summary>
    internal static class Program
    {
        private static int _failures;

        private static int Main()
        {
            var directory = Path.Combine(Path.GetTempPath(), "UnscramblerSmokeTest-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                if (VersionConstants.Constants.Count == 0)
                {
                    return Fail("the Unscrambler CN build contains no client constants");
                }

                string version = null;
                VersionConstants constants = null;
                foreach (var pair in VersionConstants.Constants)
                {
                    version = pair.Key;
                    constants = pair.Value;
                    break;
                }

                string executable = Path.Combine(directory, "ffxiv_dx11.exe");
                File.WriteAllBytes(executable, new byte[0]);
                File.WriteAllText(Path.Combine(directory, "ffxivgame.ver"), version);

                using (var unscrambler = new NetworkPacketUnscrambler())
                {
                    Check("initialise", unscrambler.Initialize(executable));
                    Check("version recognised", unscrambler.GameVersion == version);
                    Console.WriteLine("  version      : " + unscrambler.GameVersion);
                    if (!unscrambler.Enabled)
                    {
                        Console.WriteLine("  init error   : " + unscrambler.LastError);
                    }

                    // The initialiser is plaintext and only derives the keys.
                    byte[] initialiser = BuildUnknownInitialiser(constants);
                    Check("initialiser is not rewritten", !unscrambler.TryUnscramble(initialiser));

                    // A packet with an obfuscated opcode must now be rewritten.
                    byte[] obfuscated = BuildObfuscatedPacket(constants, 512);
                    byte[] before = (byte[])obfuscated.Clone();
                    bool rewritten = unscrambler.TryUnscramble(obfuscated);
                    Check("obfuscated packet rewritten", rewritten);
                    Check("packet bytes changed", !Same(before, obfuscated));

                    // An opcode that is not in the obfuscated set stays untouched.
                    byte[] plain = BuildObfuscatedPacket(constants, 128);
                    WriteUInt16(plain, 2, 0x0001);
                    before = (byte[])plain.Clone();
                    Check("plain packet untouched", !unscrambler.TryUnscramble(plain) && Same(before, plain));
                }

                // An unknown client version must leave deobfuscation off instead of
                // corrupting packets.
                string otherDirectory = Path.Combine(directory, "old");
                Directory.CreateDirectory(otherDirectory);
                string otherExe = Path.Combine(otherDirectory, "ffxiv_dx11.exe");
                File.WriteAllBytes(otherExe, new byte[0]);
                File.WriteAllText(Path.Combine(otherDirectory, "ffxivgame.ver"), "1999.01.01.0000.0000");
                using (var unscrambler = new NetworkPacketUnscrambler())
                {
                    Check("unknown client rejected", !unscrambler.Initialize(otherExe));
                    Check("unknown client explains itself", !string.IsNullOrEmpty(unscrambler.LastError));
                    Console.WriteLine("  reason       : " + unscrambler.LastError);
                }

                // A missing executable must be reported, not thrown.
                using (var unscrambler = new NetworkPacketUnscrambler())
                {
                    Check("missing client rejected", !unscrambler.Initialize(Path.Combine(directory, "nope.exe")));
                }

                Console.WriteLine();
                if (_failures != 0)
                {
                    Console.Error.WriteLine("FAIL - " + _failures + " problem(s)");
                    return 1;
                }
                Console.WriteLine("PASS - CN packet deobfuscation works end to end");
                return 0;
            }
            finally
            {
                try { Directory.Delete(directory, true); } catch { /* best effort */ }
            }
        }

        private static byte[] BuildUnknownInitialiser(VersionConstants constants)
        {
            byte[] packet = new byte[64];
            new Random(1).NextBytes(packet);
            WriteUInt16(packet, 2, (ushort)constants.UnknownObfuscationInitOpcode);
            packet[22] = constants.ObfuscationEnabledMode;
            packet[23] = 0x37;
            packet[24] = 0x91;
            WriteUInt32(packet, 28, 0x12345678);
            return packet;
        }

        private static byte[] BuildObfuscatedPacket(VersionConstants constants, int length)
        {
            byte[] packet = new byte[length];
            new Random(length).NextBytes(packet);
            WriteUInt16(packet, 2, (ushort)constants.ObfuscatedOpcodes["StatusEffectList"]);
            return packet;
        }

        private static bool Same(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
            {
                return false;
            }
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static void WriteUInt16(byte[] buffer, int offset, ushort value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
        }

        private static void WriteUInt32(byte[] buffer, int offset, uint value)
        {
            buffer[offset] = (byte)value;
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }

        private static void Check(string what, bool condition)
        {
            if (condition)
            {
                Console.WriteLine("  OK   " + what);
            }
            else
            {
                _failures++;
                Console.Error.WriteLine("  FAIL " + what);
            }
        }

        private static int Fail(string message)
        {
            Console.Error.WriteLine("FAIL - " + message);
            return 1;
        }
    }
}
