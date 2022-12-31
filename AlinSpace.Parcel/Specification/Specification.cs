using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlinSpace.Parcel
{
    /// <summary>
    /// Represents the parcel specification.
    /// </summary>
    public class Specification : ISpecification
    {
        /// <summary>
        /// Gets the library version.
        /// </summary>
        [JsonIgnore]
        public Version LibraryVersion { get; } = Constants.CurrentVersion;

        /// <summary>
        /// Gets or sets the parcel version.
        /// </summary>
        public Version ParcelVersion { get; set; } = new Version(0, 0, 0);

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime? CreationTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the compression level.
        /// </summary>
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

        /// <summary>
        /// Reads from JSON file.
        /// </summary>
        /// <param name="filePath">File path to read from.</param>
        /// <returns>Specification.</returns>
        public static Specification ReadFromJsonFile(string filePath)
        {
            var specificationJson = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Specification>(specificationJson) ?? new Specification();
        }

        /// <summary>
        /// Writes to JSON file.
        /// </summary>
        /// <param name="filePath">File path to write to.</param>
        /// <param name="specification">Specification.</param>
        public static void WriteFromJsonFile(string filePath, ISpecification specification)
        {
            var specificationJson = JsonSerializer.Serialize(specification);
            File.WriteAllText(filePath, specificationJson);
        }
    }
}
