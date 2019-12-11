using System.Net.Http;
using Microsoft.IO;

namespace SharpSharp {
    internal static class GlobalStatics {
        public static HttpClient HttpClient { get; } = new HttpClient();

        public static RecyclableMemoryStreamManager RecyclableMemoryStreamManager { get; } = new RecyclableMemoryStreamManager();
    }
}
