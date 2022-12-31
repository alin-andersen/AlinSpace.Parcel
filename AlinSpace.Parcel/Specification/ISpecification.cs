using System.IO.Compression;

namespace AlinSpace.Parcel
{
    /// <summary>
    /// Represents the specification interface.
    /// </summary>
    public interface ISpecification
    {
        /// <summary>
        /// Gets or sets the library version.
        /// </summary>
        Version LibraryVersion { get; }

        /// <summary>
        /// Gets or sets the parcel version.
        /// </summary>
        Version ParcelVersion { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        DateTime? CreationTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the compression level.
        /// </summary>
        CompressionLevel CompressionLevel { get; set; }
    }
}
