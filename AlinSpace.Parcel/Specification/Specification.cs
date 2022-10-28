using System.Text.Json;

namespace AlinSpace.Parcel
{
    internal class Specification : ISpecification
    {
        public Version Version { get; set; } = new Version(1, 0, 0);

        public DateTime? CreationTimestamp { get; set; }

        #region Compression

        public bool CompressFiles { get; set; }

        public CompressAlgorithm CompressAlgorithm { get; set; }

        #endregion

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
