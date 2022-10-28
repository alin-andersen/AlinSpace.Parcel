using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlinSpace.Parcel
{
    internal class Specification : ISpecification
    {
        [JsonIgnore]
        public Version Version { get; } = Constants.CurrentVersion;

        public Version ParcelVersion { get; set; } = new Version(0, 0, 0);

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
