// The Unscrambler library targets net10.0, where System.Index / System.Range
// (and therefore the `x[a..b]` syntax) are part of the BCL.  .NET Framework has
// no such types, so they are supplied here.  They are only needed for the
// compiler's range lowering - the library never uses them directly.
namespace System
{
    public readonly struct Index : IEquatable<Index>
    {
        private readonly int _value;

        public Index(int value, bool fromEnd = false)
        {
            if (value < 0 && !fromEnd)
                throw new ArgumentOutOfRangeException(nameof(value));

            _value = fromEnd ? ~value : value;
        }

        public int Value => _value < 0 ? ~_value : _value;

        public bool IsFromEnd => _value < 0;

        public static Index Start => new Index(0);

        public static Index End => new Index(0, fromEnd: true);

        public static implicit operator Index(int value) => new Index(value);

        public int GetOffset(int length)
        {
            int offset = IsFromEnd ? length - Value : Value;
            if ((uint)offset > (uint)length)
                throw new ArgumentOutOfRangeException(nameof(length));
            return offset;
        }

        public bool Equals(Index other) => _value == other._value;

        public override bool Equals(object obj) => obj is Index other && Equals(other);

        public override int GetHashCode() => _value;

        public override string ToString() => IsFromEnd ? "^" + Value : Value.ToString();
    }

    public readonly struct Range : IEquatable<Range>
    {
        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        public Index Start { get; }

        public Index End { get; }

        public static Range StartAt(Index start) => new Range(start, Index.End);

        public static Range EndAt(Index end) => new Range(Index.Start, end);

        public static Range All => new Range(Index.Start, Index.End);

        public bool Equals(Range other) => Start.Equals(other.Start) && End.Equals(other.End);

        public override bool Equals(object obj) => obj is Range other && Equals(other);

        public override int GetHashCode() => (Start.GetHashCode() * 397) ^ End.GetHashCode();

        public override string ToString() => Start + ".." + End;
    }
}
