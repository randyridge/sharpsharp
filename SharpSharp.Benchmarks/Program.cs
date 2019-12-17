using System;

namespace SharpSharp.Benchmarks {
    internal static class Program {
        private static void Main() {
            ImagePipeline
                .FromFile(TestFiles.InputJpg)
                .Png()
                .ToBuffer(out var b);
            var hashLong = DifferenceHash.HashLong(b);
            var hashShort = DifferenceHash.HashShort(b);
            Console.WriteLine(Reverse(Convert.ToString(hashLong, 2)));
            Console.WriteLine(Reverse(Convert.ToString(hashShort, 2)));
        }

        private static string Reverse(string s) {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
