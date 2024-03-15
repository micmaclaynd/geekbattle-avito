using Microsoft.Extensions.Configuration;

namespace Shared.Configuration.Extensions {
    public static class JsonLoaderExtension {
        public static void AddJsonFilesFromDirectory(this IConfigurationBuilder configurationBuilder, string path) {
            foreach (string file in Directory.EnumerateFiles(path, "*.json")) {
                configurationBuilder.AddJsonFile(file);
            }
        }
    }
}