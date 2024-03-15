using Microsoft.Extensions.Configuration;

namespace Shared.Configuration.Extensions {
    public static class XmlLoaderExtension {
        public static void AddXmlFilesFromDirectory(this IConfigurationBuilder configurationBuilder, string path) {
            foreach (string file in Directory.EnumerateFiles(path, "*.xml")) {
                configurationBuilder.AddXmlFile(file);
            }
        }
    }
}