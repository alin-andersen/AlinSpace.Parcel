namespace AlinSpace.Parcel
{
    /// <summary>
    /// Represents the file copy request.
    /// </summary>
    sealed class FileCopyRequest : IFileCopyRequest
    {
        public string FilePath { get; }

        public string FileName => Path.GetFileName(FilePath);

        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FilePath);

        public string FileExtension => Path.GetExtension(FilePath)[1..];

        public bool Accept { get; set; } = false;

        public string? Name { get; set; }

        /// <param name="filePath"></param>
        public FileCopyRequest(string filePath)
        {
            FilePath = filePath;
        }
    }
}
