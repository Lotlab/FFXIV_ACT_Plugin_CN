// Constants for the Chinese game client, taken from the official
// Unscrambler.XIV.CN packages (https://www.nuget.org/packages/Unscrambler.XIV.CN).
//
// The upstream repository only carries the global-client constants: the CN
// client is a separate build with its own .rdata layout, so the table offsets
// and the opcode numbers differ even when the patch date is the same.
//
// One entry per CN client version, keyed by the contents of ffxivgame.ver.
// Add a new entry when the CN client is patched.
namespace Unscrambler.Constants.Versions;

public static partial class GameConstants
{
    /// <summary>2026.04.21.0000.0000 (CN, Unscrambler.XIV.CN 7.50.0).</summary>
    [VersionConstant]
    public static VersionConstants For26_2026042100000000()
    {
        return new VersionConstants
        {
            GameVersion = "2026.04.21.0000.0000",
            TableOffsets = new long[] { 36553168, 36627904, 36673280 },
            TableSizes = new int[] { 74732, 45368, 98000 },
            TableRadixes = new int[] { 119, 107, 100 },
            TableMax = new int[] { 157, 106, 245 },
            MidTableOffset = 36551504,
            MidTableSize = 1656,
            DayTableOffset = 36771280,
            DayTableSize = 88,
            OpcodeKeyTableOffset = 36771376,
            OpcodeKeyTableSize = 728,
            ObfuscationEnabledMode = 31,
            InitZoneOpcode = 152,
            UnknownObfuscationInitOpcode = 264,
            ObfuscatedOpcodes = new Dictionary<string, int>
            {
                { "PlayerSpawn", 835 },
                { "NpcSpawn", 686 },
                { "NpcSpawn2", 585 },
                { "ActionEffect01", 922 },
                { "ActionEffect08", 498 },
                { "ActionEffect16", 576 },
                { "ActionEffect24", 458 },
                { "ActionEffect32", 968 },
                { "StatusEffectList", 279 },
                { "StatusEffectList3", 822 },
                { "Examine", 256 },
                { "UpdateGearset", 158 },
                { "UpdateParty", 569 },
                { "ActorControl", 808 },
                { "ActorCast", 837 },
                { "UnknownEffect01", 372 },
                { "UnknownEffect16", 355 },
                { "ActionEffect02", 917 },
                { "ActionEffect04", 942 },
            }
        };
    }

    /// <summary>2026.05.01.0000.0000 (CN, Unscrambler.XIV.CN 7.50.1).</summary>
    [VersionConstant]
    public static VersionConstants For26_2026050100000000()
    {
        return new VersionConstants
        {
            GameVersion = "2026.05.01.0000.0000",
            TableOffsets = new long[] { 36562416, 36596240, 36625520 },
            TableSizes = new int[] { 33820, 29280, 44880 },
            TableRadixes = new int[] { 95, 122, 110 },
            TableMax = new int[] { 89, 60, 102 },
            MidTableOffset = 36560544,
            MidTableSize = 1864,
            DayTableOffset = 36670400,
            DayTableSize = 144,
            OpcodeKeyTableOffset = 36670544,
            OpcodeKeyTableSize = 140,
            ObfuscationEnabledMode = 93,
            InitZoneOpcode = 567,
            UnknownObfuscationInitOpcode = 962,
            ObfuscatedOpcodes = new Dictionary<string, int>
            {
                { "PlayerSpawn", 682 },
                { "NpcSpawn", 246 },
                { "NpcSpawn2", 161 },
                { "ActionEffect01", 511 },
                { "ActionEffect08", 643 },
                { "ActionEffect16", 777 },
                { "ActionEffect24", 277 },
                { "ActionEffect32", 666 },
                { "StatusEffectList", 101 },
                { "StatusEffectList3", 585 },
                { "Examine", 708 },
                { "UpdateGearset", 182 },
                { "UpdateParty", 881 },
                { "ActorControl", 835 },
                { "ActorCast", 604 },
                { "UnknownEffect01", 994 },
                { "UnknownEffect16", 362 },
                { "ActionEffect02", 519 },
                { "ActionEffect04", 168 },
            }
        };
    }

    /// <summary>2026.05.25.0000.0000 (CN, Unscrambler.XIV.CN 7.51.0).</summary>
    [VersionConstant]
    public static VersionConstants For26_2026052500000000()
    {
        return new VersionConstants
        {
            GameVersion = "2026.05.25.0000.0000",
            TableOffsets = new long[] { 36565520, 36612528, 36682528 },
            TableSizes = new int[] { 47008, 70000, 85768 },
            TableRadixes = new int[] { 113, 125, 151 },
            TableMax = new int[] { 104, 140, 142 },
            MidTableOffset = 36564384,
            MidTableSize = 1136,
            DayTableOffset = 36740512,
            DayTableSize = 204,
            OpcodeKeyTableOffset = 36740720,
            OpcodeKeyTableSize = 572,
            ObfuscationEnabledMode = 159,
            InitZoneOpcode = 150,
            UnknownObfuscationInitOpcode = 163,
            ObfuscatedOpcodes = new Dictionary<string, int>
            {
                { "PlayerSpawn", 597 },
                { "NpcSpawn", 619 },
                { "NpcSpawn2", 578 },
                { "ActionEffect01", 915 },
                { "ActionEffect08", 211 },
                { "ActionEffect16", 167 },
                { "ActionEffect24", 327 },
                { "ActionEffect32", 977 },
                { "StatusEffectList", 661 },
                { "StatusEffectList3", 312 },
                { "Examine", 930 },
                { "UpdateGearset", 295 },
                { "UpdateParty", 235 },
                { "ActorControl", 488 },
                { "ActorCast", 758 },
                { "UnknownEffect01", 923 },
                { "UnknownEffect16", 617 },
                { "ActionEffect02", 343 },
                { "ActionEffect04", 239 },
            }
        };
    }

    /// <summary>2026.06.10.0000.0000 (CN, Unscrambler.XIV.CN 7.51.1).</summary>
    [VersionConstant]
    public static VersionConstants For26_2026061000000000()
    {
        return new VersionConstants
        {
            GameVersion = "2026.06.10.0000.0000",
            TableOffsets = new long[] { 36545536, 36574640, 36667632 },
            TableSizes = new int[] { 29104, 92984, 84972 },
            TableRadixes = new int[] { 107, 118, 97 },
            TableMax = new int[] { 68, 197, 219 },
            MidTableOffset = 36543792,
            MidTableSize = 1744,
            DayTableOffset = 36752608,
            DayTableSize = 120,
            OpcodeKeyTableOffset = 36752736,
            OpcodeKeyTableSize = 412,
            ObfuscationEnabledMode = 240,
            InitZoneOpcode = 139,
            UnknownObfuscationInitOpcode = 464,
            ObfuscatedOpcodes = new Dictionary<string, int>
            {
                { "PlayerSpawn", 948 },
                { "NpcSpawn", 275 },
                { "NpcSpawn2", 184 },
                { "ActionEffect01", 473 },
                { "ActionEffect08", 321 },
                { "ActionEffect16", 401 },
                { "ActionEffect24", 561 },
                { "ActionEffect32", 907 },
                { "StatusEffectList", 299 },
                { "StatusEffectList3", 239 },
                { "Examine", 443 },
                { "UpdateGearset", 452 },
                { "UpdateParty", 844 },
                { "ActorControl", 639 },
                { "ActorCast", 760 },
                { "UnknownEffect01", 531 },
                { "UnknownEffect16", 564 },
                { "ActionEffect02", 761 },
                { "ActionEffect04", 931 },
            }
        };
    }

    /// <summary>2026.06.18.0000.0000 (CN, Unscrambler.XIV.CN 7.51.2).</summary>
    [VersionConstant]
    public static VersionConstants For26_2026061800000000()
    {
        return new VersionConstants
        {
            GameVersion = "2026.06.18.0000.0000",
            TableOffsets = new long[] { 36556736, 36597680, 36631952 },
            TableSizes = new int[] { 40936, 34272, 63232 },
            TableRadixes = new int[] { 119, 119, 104 },
            TableMax = new int[] { 86, 72, 152 },
            MidTableOffset = 36555904,
            MidTableSize = 832,
            DayTableOffset = 36695184,
            DayTableSize = 196,
            OpcodeKeyTableOffset = 36695392,
            OpcodeKeyTableSize = 420,
            ObfuscationEnabledMode = 72,
            InitZoneOpcode = 823,
            UnknownObfuscationInitOpcode = 215,
            ObfuscatedOpcodes = new Dictionary<string, int>
            {
                { "PlayerSpawn", 736 },
                { "NpcSpawn", 888 },
                { "NpcSpawn2", 392 },
                { "ActionEffect01", 893 },
                { "ActionEffect08", 848 },
                { "ActionEffect16", 638 },
                { "ActionEffect24", 420 },
                { "ActionEffect32", 674 },
                { "StatusEffectList", 306 },
                { "StatusEffectList3", 651 },
                { "Examine", 812 },
                { "UpdateGearset", 819 },
                { "UpdateParty", 988 },
                { "ActorControl", 415 },
                { "ActorCast", 457 },
                { "UnknownEffect01", 324 },
                { "UnknownEffect16", 838 },
                { "ActionEffect02", 474 },
                { "ActionEffect04", 696 },
            }
        };
    }

    /// <summary>2026.08.05.0000.0000 (CN, Unscrambler.XIV.CN 7.55.1).</summary>
    [VersionConstant]
    public static VersionConstants For26_2026080500000000()
    {
        return new VersionConstants
        {
            GameVersion = "2026.08.05.0000.0000",
            TableOffsets = new long[] { 36574736, 36679936, 36742592 },
            TableSizes = new int[] { 105196, 62648, 83168 },
            TableRadixes = new int[] { 119, 82, 113 },
            TableMax = new int[] { 221, 191, 184 },
            MidTableOffset = 36573552,
            MidTableSize = 1184,
            DayTableOffset = 36825760,
            DayTableSize = 152,
            OpcodeKeyTableOffset = 36825920,
            OpcodeKeyTableSize = 356,
            ObfuscationEnabledMode = 140,
            InitZoneOpcode = 653,
            UnknownObfuscationInitOpcode = 296,
            ObfuscatedOpcodes = new Dictionary<string, int>
            {
                { "PlayerSpawn", 920 },
                { "NpcSpawn", 111 },
                { "NpcSpawn2", 647 },
                { "ActionEffect01", 662 },
                { "ActionEffect08", 356 },
                { "ActionEffect16", 433 },
                { "ActionEffect24", 923 },
                { "ActionEffect32", 882 },
                { "StatusEffectList", 497 },
                { "StatusEffectList3", 339 },
                { "Examine", 699 },
                { "UpdateGearset", 822 },
                { "UpdateParty", 440 },
                { "ActorControl", 474 },
                { "ActorCast", 396 },
                { "UnknownEffect01", 191 },
                { "UnknownEffect16", 277 },
                { "ActionEffect02", 773 },
                { "ActionEffect04", 333 },
            }
        };
    }

    /// <summary>2026.09.01.0000.0000 (CN, Unscrambler.XIV.CN 7.56.0).</summary>
    [VersionConstant]
    public static VersionConstants For26_2026090100000000()
    {
        return new VersionConstants
        {
            GameVersion = "2026.09.01.0000.0000",
            TableOffsets = new long[] { 36605104, 36694144, 36713008 },
            TableSizes = new int[] { 89040, 18860, 43648 },
            TableRadixes = new int[] { 105, 115, 124 },
            TableMax = new int[] { 212, 41, 88 },
            MidTableOffset = 36603120,
            MidTableSize = 1984,
            DayTableOffset = 36756656,
            DayTableSize = 84,
            OpcodeKeyTableOffset = 36756752,
            OpcodeKeyTableSize = 772,
            ObfuscationEnabledMode = 12,
            InitZoneOpcode = 929,
            UnknownObfuscationInitOpcode = 102,
            ObfuscatedOpcodes = new Dictionary<string, int>
            {
                { "PlayerSpawn", 946 },
                { "NpcSpawn", 452 },
                { "NpcSpawn2", 618 },
                { "ActionEffect01", 748 },
                { "ActionEffect08", 253 },
                { "ActionEffect16", 855 },
                { "ActionEffect24", 180 },
                { "ActionEffect32", 334 },
                { "StatusEffectList", 584 },
                { "StatusEffectList3", 525 },
                { "Examine", 105 },
                { "UpdateGearset", 884 },
                { "UpdateParty", 479 },
                { "ActorControl", 908 },
                { "ActorCast", 266 },
                { "UnknownEffect01", 475 },
                { "UnknownEffect16", 844 },
                { "ActionEffect02", 854 },
                { "ActionEffect04", 881 },
            }
        };
    }

    /// <summary>2026.09.15.0000.0000 (CN, Unscrambler.XIV.CN 7.56.1).</summary>
    [VersionConstant]
    public static VersionConstants For26_2026091500000000()
    {
        return new VersionConstants
        {
            GameVersion = "2026.09.15.0000.0000",
            TableOffsets = new long[] { 36605680, 36647424, 36708944 },
            TableSizes = new int[] { 41736, 61516, 56700 },
            TableRadixes = new int[] { 94, 91, 105 },
            TableMax = new int[] { 111, 169, 135 },
            MidTableOffset = 36603792,
            MidTableSize = 1888,
            DayTableOffset = 36765648,
            DayTableSize = 176,
            OpcodeKeyTableOffset = 36765824,
            OpcodeKeyTableSize = 660,
            ObfuscationEnabledMode = 176,
            InitZoneOpcode = 811,
            UnknownObfuscationInitOpcode = 676,
            ObfuscatedOpcodes = new Dictionary<string, int>
            {
                { "PlayerSpawn", 452 },
                { "NpcSpawn", 524 },
                { "NpcSpawn2", 445 },
                { "ActionEffect01", 787 },
                { "ActionEffect08", 540 },
                { "ActionEffect16", 140 },
                { "ActionEffect24", 778 },
                { "ActionEffect32", 938 },
                { "StatusEffectList", 131 },
                { "StatusEffectList3", 988 },
                { "Examine", 498 },
                { "UpdateGearset", 271 },
                { "UpdateParty", 401 },
                { "ActorControl", 607 },
                { "ActorCast", 354 },
                { "UnknownEffect01", 957 },
                { "UnknownEffect16", 637 },
                { "ActionEffect02", 779 },
                { "ActionEffect04", 658 },
            }
        };
    }

    /// <summary>2026.10.06.0000.0000 (CN, Unscrambler.XIV.CN 7.57.0).</summary>
    [VersionConstant]
    public static VersionConstants For26_2026100600000000()
    {
        return new VersionConstants
        {
            GameVersion = "2026.10.06.0000.0000",
            TableOffsets = new long[] { 36613152, 36688416, 36708592 },
            TableSizes = new int[] { 75264, 20172, 87264 },
            TableRadixes = new int[] { 84, 123, 108 },
            TableMax = new int[] { 224, 41, 202 },
            MidTableOffset = 36612432,
            MidTableSize = 712,
            DayTableOffset = 36795856,
            DayTableSize = 60,
            OpcodeKeyTableOffset = 36795920,
            OpcodeKeyTableSize = 332,
            ObfuscationEnabledMode = 254,
            InitZoneOpcode = 716,
            UnknownObfuscationInitOpcode = 766,
            ObfuscatedOpcodes = new Dictionary<string, int>
            {
                { "PlayerSpawn", 613 },
                { "NpcSpawn", 276 },
                { "NpcSpawn2", 754 },
                { "ActionEffect01", 189 },
                { "ActionEffect08", 319 },
                { "ActionEffect16", 912 },
                { "ActionEffect24", 158 },
                { "ActionEffect32", 630 },
                { "StatusEffectList", 718 },
                { "StatusEffectList3", 151 },
                { "Examine", 412 },
                { "UpdateGearset", 981 },
                { "UpdateParty", 362 },
                { "ActorControl", 408 },
                { "ActorCast", 769 },
                { "UnknownEffect01", 729 },
                { "UnknownEffect16", 295 },
                { "ActionEffect02", 851 },
                { "ActionEffect04", 892 },
            }
        };
    }

}
