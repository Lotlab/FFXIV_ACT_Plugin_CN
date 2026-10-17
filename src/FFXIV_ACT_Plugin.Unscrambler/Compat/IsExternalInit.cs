// `init` accessors (C# 9) require this marker type, which .NET Framework does
// not ship.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}
