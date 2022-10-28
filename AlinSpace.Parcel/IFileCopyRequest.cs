namespace AlinSpace.Parcel
{
    public interface IFileCopyRequest
    {
        /// <summary>
        /// Gets the file path.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Gets the file name without extension.
        /// </summary>
        string FileNameWithoutExtension { get; }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// Gets or sets the flag indicating whether or not the copy request has been granted.
        /// </summary>
        bool Accept { get; set; }

        /// <summary>
        /// Gets or sets the name to be used when copied.
        /// </summary>
        string? ResourceName { get; set; }
    }
}