using System;
using System.Linq;
using System.Runtime.CompilerServices;
using RandyRidge.Common;

namespace SharpSharp {
    internal static class StringExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWithAnyOrdinalIgnoreCase(this string text, params string[] values) {
            Guard.ArgumentNotNull(text, nameof(text));
            Guard.ArgumentNotNullOrEmpty(values, nameof(values));
            return values.Any(value => text.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        } 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWithOrdinalIgnoreCase(this string text, string value) {
            Guard.ArgumentNotNull(text, nameof(text));
            return text.EndsWith(value, StringComparison.OrdinalIgnoreCase);
        } 
    }
}
