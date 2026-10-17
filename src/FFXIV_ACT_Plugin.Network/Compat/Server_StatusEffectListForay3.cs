using System.Runtime.InteropServices;

// The Lotlab Machina fork (feature/oodle-state-recovery) ships
// Server_StatusEffectListForay3 for the Global, Korean and TraditionalChinese
// header sets, but the Chinese one is missing.  The layout below is the Chinese
// layout as it was present in the Machina.FFXIV 2.4.7.1 binary this plugin
// release was originally built against, and it is identical to the Global one.
//
// Remove this file once Machina.FFXIV declares
// Machina.FFXIV.Headers.Chinese.Server_StatusEffectListForay3 itself.
namespace Machina.FFXIV.Headers.Chinese
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Server_StatusEffectListForay3
    {
        public Server_MessageHeader MessageHeader;
        public byte PhantomJobLevel;
        public byte PhantomJobLevelSync;
        public ushort unk1;
        public byte JobID;
        public byte Level1;
        public byte Level2;
        public byte Level3;
        public uint CurrentHP;
        public uint MaxHP;
        public ushort CurrentMP;
        public ushort MaxMP;
        public byte DamageShield;
        public ushort Unknown1;
        public byte Unknown2;
        public fixed byte Effects[30 * 3 * 4];
    }
}
