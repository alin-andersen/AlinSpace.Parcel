namespace AlinSpace.Parcel
{
    /// <summary>
    /// Represents the file copy request.
    /// </summary>
    sealed class FileCopyRequest : IFileCopyRequest
    {
        public string FilePath { get; }

        public string Filename => Path.GetFileName(FilePath);

        public string FilenameWithoutExtension => Path.GetFileNameWithoutExtension(FilePath);

        public string FileExtension => Path.GetExtension(FilePath)[1..];

        public bool Accept { get; set; } = false;

        public string? ResourceName { get; set; }

        public FileCopyRequest(string filePath)
        {
            FilePath = filePath;
        }
    }
}
