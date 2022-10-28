using System.IO.Compression;
using System.Text.Json;

namespace AlinSpace.Parcel
{
    internal class Specification : ISpecification
    {
        public Version Version { get; set; } = Constants.CurrentVersion;

        public DateTime? CreationTimestamp { get; set; }

        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

        public static Specification ReadFromJsonFile(string filePath)
        {
            var specificationJson = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Specification>(specificationJson) ?? new Specification();
        }

        public static void WriteFromJsonFile(string filePath, ISpecification specification)
        {
            var specificationJson = JsonSerializer.Serialize(specification);
            File.WriteAllText(filePath, specificationJson);
        }
    }
}
