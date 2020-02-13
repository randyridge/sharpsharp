using System;

namespace SharpSharp {
    public readonly struct Rgba : IEquatable<Rgba> {
        public Rgba(byte red, byte green, byte blue, byte alpha) {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public byte Red { get; }

        public byte Green { get; }

        public byte Blue { get; }

        public byte Alpha { get; }

        public bool Equals(Rgba other) => Red == other.Red && Green == other.Green && Blue == other.Blue && Alpha.Equals(other.Alpha);

        public override bool Equals(object? obj) => obj is Rgba other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Red, Green, Blue, Alpha);

        public double[] ToDoubles() => new double[] {Red, Green, Blue, Alpha};

        public static bool operator ==(Rgba left, Rgba right) => left.Equals(right);

        public static bool operator !=(Rgba left, Rgba right) => !left.Equals(right);
    }
}
