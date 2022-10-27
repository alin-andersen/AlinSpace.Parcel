namespace AlinSpace.Parcel
{
    public sealed class FileCopyRequest
    {
        public string FilePath { get; }

        public string FileName => Path.GetFileName(FilePath);

        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FilePath);

        public string Extension => Path.GetExtension(FilePath)[1..];

        public FileCopyRequest(string filePath)
        {
            FilePath = filePath;
        }

        public bool Accept { get; set; } = false;

        public string? Name { get; set; }
    }
}
