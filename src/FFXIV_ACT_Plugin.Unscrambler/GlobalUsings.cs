// The upstream Unscrambler sources are written for a net10.0 SDK project, which
// turns these namespaces on implicitly.  net48 does not, so they are declared
// here instead of adding a using to every file.
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// .NET Framework has no Span-based BitConverter; see Compat/SpanBitConverter.cs.
// Aliasing the name keeps the upstream call sites unchanged.
global using BitConverter = FFXIV_ACT_Plugin.Unscrambler.Compat.SpanBitConverter;
