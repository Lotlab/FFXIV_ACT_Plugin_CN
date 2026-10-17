// The Unscrambler sources rely on <ImplicitUsings>enable</ImplicitUsings>,
// which net48 does not default to.  These mirror the implicit global usings of
// a net10.0 SDK project.
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// BitConverter has no Span overloads on .NET Framework; see Compat/BitConverter.cs.
global using BitConverter = FFXIV_ACT_Plugin.Unscrambler.Compat.BitConverter;
