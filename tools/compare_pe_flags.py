"""Compare the PE / CLR header flags of the original assemblies and the rebuilt ones.

Useful for checking that the reconstruction did not change the target platform or the
"prefer 32-bit" / strong-name characteristics of the shipped binaries.
"""
import os
import struct
import sys

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))


def pe_info(path):
    data = open(path, "rb").read()
    e_lfanew = struct.unpack_from("<I", data, 0x3C)[0]
    assert data[e_lfanew:e_lfanew + 4] == b"PE\0\0"
    coff = e_lfanew + 4
    machine, nsec, _, _, _, optsize, _ = struct.unpack_from("<HHIIIHH", data, coff)
    opt = coff + 20
    magic = struct.unpack_from("<H", data, opt)[0]
    dd = opt + (0x60 if magic == 0x10B else 0x70)
    cor_rva, cor_size = struct.unpack_from("<II", data, dd + 14 * 8)
    info = {
        "file": os.path.basename(path),
        "machine": hex(machine),
        "pe": "PE32" if magic == 0x10B else "PE32+",
        "managed": bool(cor_rva),
    }
    if not cor_rva:
        return info
    section = opt + optsize
    off = None
    for i in range(nsec):
        _, vsize, vaddr, rawsize, rawptr = struct.unpack_from("<8sIIII", data, section + i * 40)
        if vaddr <= cor_rva < vaddr + max(vsize, rawsize):
            off = rawptr + (cor_rva - vaddr)
            break
    cor = data[off:off + cor_size]
    _, major, minor, _, _, flags, _ = struct.unpack_from("<IHHIIII", cor, 0)
    info.update(
        runtime="%d.%d" % (major, minor),
        flags=hex(flags),
        ILONLY=bool(flags & 0x1),
        x86_32bit_required=bool(flags & 0x2),
        strongname_signed=bool(flags & 0x8),
        x86_32bit_preferred=bool(flags & 0x20000),
    )
    return info


PAIRS = [
    (".work/bin/ffxiv_act_plugin_main.dll", "artifacts/bin/Release/FFXIV_ACT_Plugin.dll"),
    (".work/bin/ffxiv_act_plugin.common.dll", "artifacts/bin/Release/FFXIV_ACT_Plugin.Common.dll"),
    (".work/bin/ffxiv_act_plugin.config.dll", "artifacts/bin/Release/FFXIV_ACT_Plugin.Config.dll"),
    (".work/bin/ffxiv_act_plugin.logfile.dll", "artifacts/bin/Release/FFXIV_ACT_Plugin.Logfile.dll"),
    (".work/bin/ffxiv_act_plugin.memory.dll", "artifacts/bin/Release/FFXIV_ACT_Plugin.Memory.dll"),
    (".work/bin/ffxiv_act_plugin.network.dll", "artifacts/bin/Release/FFXIV_ACT_Plugin.Network.dll"),
    (".work/bin/ffxiv_act_plugin.parse.dll", "artifacts/bin/Release/FFXIV_ACT_Plugin.Parse.dll"),
    (".work/bin/ffxiv_act_plugin.resource.dll", "artifacts/bin/Release/FFXIV_ACT_Plugin.Resource.dll"),
]

KEYS = ("machine", "pe", "runtime", "ILONLY", "x86_32bit_required",
        "x86_32bit_preferred", "strongname_signed")


def main():
    mismatches = 0
    for original, built in PAIRS:
        a = pe_info(os.path.join(REPO, original))
        b = pe_info(os.path.join(REPO, built))
        diffs = [k for k in KEYS if a.get(k) != b.get(k)]
        if diffs:
            mismatches += 1
        print("%-32s %s" % (a["file"], "OK" if not diffs else "DIFF " + ", ".join(
            "%s %s->%s" % (k, a.get(k), b.get(k)) for k in diffs)))
    print("\n%d/%d assemblies with identical PE/CLR flags" % (len(PAIRS) - mismatches, len(PAIRS)))
    return 1 if mismatches else 0


if __name__ == "__main__":
    sys.exit(main())
