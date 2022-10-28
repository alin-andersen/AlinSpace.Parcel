using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace AlinSpace.Parcel
{
    /// <summary>
    /// Represents the parcel.
    /// </summary>
    public sealed class Parcel : IDisposable
    {
        private readonly Workspace workspace;

        private Parcel(Workspace workspace)
        {
            this.workspace = workspace;
        }

        static string PrepareWorkspacePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), $".AlinSpace.Parcel.{Guid.NewGuid()}");
            }

            return path;
        }

        string PrepareResourceName(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentNullException(nameof(resourceName));

            return resourceName.Trim();
        }

        /// <summary>
        /// Creates a new parcel.
        /// </summary>
        /// <param name="workspacePath">Workspace path.</param>
        /// <returns>Parcel.</returns>
        public static Parcel New(string? workspacePath = null)
        {
            workspacePath = PrepareWorkspacePath(workspacePath);

            var workspace = Workspace.Create(workspacePath);
            var parcel = new Parcel(workspace);

            return parcel;
        }

        /// <summary>
        /// Opens the parcel.
        /// </summary>
        /// <param name="parcelFilePath">Parcel file path.</param>
        /// <param name="workspacePath">Workspace path.</param>
        /// <returns>Parcel.</returns>
        public static Parcel Open(string parcelFilePath, string? workspacePath = null)
        {
            workspacePath = PrepareWorkspacePath(workspacePath);

            var workspace = Workspace.Create(workspacePath);
            var parcel = new Parcel(workspace);

            parcel.Unpack(parcelFilePath);

            return parcel;
        }

        /// <summary>
        /// Packs the parcel.
        /// </summary>
        /// <param name="parcelFilePath">File path to save the parcel to.</param>
        /// <param name="resetAfterPacking">Reset the parcel after packing it.</param>
        public void Pack(string parcelFilePath, bool resetAfterPacking = true)
        {
            parcelFilePath = PathHelper.MakeRoot(parcelFilePath);

            // Check if parcel file is a directory path.
            if (Directory.Exists(parcelFilePath))
            {
                throw new Exception($"Parcel file path is a directory path");
            }

            #region Create directory of parcel file if it does not exist

            var parcelDirectoryPath = Path.GetDirectoryName(parcelFilePath);

            if (string.IsNullOrWhiteSpace(parcelDirectoryPath))
            {
                throw new Exception($"Parcel file directory path not found.");
            }

            if (!Directory.Exists(parcelDirectoryPath))
            {
                Directory.CreateDirectory(parcelDirectoryPath);
            }

            #endregion

            // Delete file if it does already exist.
            if (File.Exists(parcelFilePath))
            {
                File.Delete(parcelFilePath);
            }

            WriteSpecificationToWorkspace();
            WriteMetadataToWorkspace();

            ZipFile.CreateFromDirectory(
                sourceDirectoryName: workspace.PathToWorkspace,
                destinationArchiveFileName: parcelFilePath, 
                compressionLevel: Specification.CompressionLevel,
                includeBaseDirectory: false,
                entryNameEncoding: Encoding.UTF8);

            if (resetAfterPacking)
            {
                workspace.Reset();
            }
        }

        /// <summary>
        /// Unpacks the parcel.
        /// </summary>
        /// <param name="parcelFilePath">File path to the parcel.</param>
        /// <param name="resetBeforeUnpacking">Reset the parcel beforing unpacking.</param>
        /// <param name="overwriteFiles">Overwrite files in workspace.</param>
        public void Unpack(string parcelFilePath, bool resetBeforeUnpacking = true, bool overwriteFiles = true)
        {
            try
            {
                if (resetBeforeUnpacking)
                {
                    workspace.Reset();
                }

                parcelFilePath = PathHelper.MakeRoot(parcelFilePath);

                ZipFile.ExtractToDirectory(
                    sourceArchiveFileName: parcelFilePath,
                    destinationDirectoryName: workspace.PathToWorkspace,
                    entryNameEncoding: Encoding.UTF8,
                    overwriteFiles: overwriteFiles);

                ReadSpecificationFromWorkspace();
                ReadMetadataFromWorkspace();
            }
            catch(Exception)
            {
                Reset();
                throw;
            }
        }

        /// <summary>
        /// Resets the parcel.
        /// </summary>
        public void Reset()
        {
            workspace.Reset();
        }

        #region Specification

        /// <summary>
        /// Gets the specification.
        /// </summary>
        public ISpecification Specification { get; private set; } = new Specification();

        void UpdateSpecification()
        {
            if (Specification == null)
                return;

            Specification.CreationTimestamp = DateTime.UtcNow;
        }

        void ReadSpecificationFromWorkspace()
        {
            var specificationFilePath = Path.Combine(workspace.FilesPath, Constants.SpecificationFileName);
            Specification = AlinSpace.Parcel.Specification.ReadFromJsonFile(specificationFilePath);
        }

        void WriteSpecificationToWorkspace()
        {
            UpdateSpecification();

            var specificationFilePath = Path.Combine(workspace.FilesPath, Constants.SpecificationFileName);
            AlinSpace.Parcel.Specification.WriteFromJsonFile(specificationFilePath, Specification);
        }

        #endregion

        #region Metadata

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        void ReadMetadataFromWorkspace()
        {
            var metadataFilePath = Path.Combine(workspace.PathToWorkspace, Constants.MetadataFileName);

            if (!File.Exists(metadataFilePath))
            {
                Metadata = new Dictionary<string, string>();
                return;
            }

            var metadataJson = File.ReadAllText(metadataFilePath);

            if (metadataJson == null || string.IsNullOrWhiteSpace(metadataJson))
            {
                Metadata = new Dictionary<string, string>();
                return;
            }

            Metadata = JsonSerializer.Deserialize<IDictionary<string, string>>(metadataJson) ?? new Dictionary<string, string>();
        }

        void WriteMetadataToWorkspace()
        {
            var metadataFilePath = Path.Combine(workspace.PathToWorkspace, Constants.MetadataFileName);
            var metadataJson = JsonSerializer.Serialize(Metadata);
            
            File.WriteAllText(metadataFilePath, metadataJson);
        }

        /// <summary>
        /// Copy metadata from parcel.
        /// </summary>
        /// <param name="parcel">Parcel to copy from.</param>
        /// <returns>Parcel.</returns>
        public Parcel CopyMetadataFromParcel(Parcel parcel)
        {
            Metadata = new Dictionary<string, string>(parcel.Metadata);
            return this;
        }

        #endregion

        /// <summary>
        /// Gets all resource names.
        /// </summary>
        /// <returns>Enumerable of resource names.</returns>
        public IEnumerable<string> GetResourceNames()
        {
            return Directory
                .GetFiles(workspace.FilesPath)
                .Select(x => Path.GetFileName(x));
        }

        #region Write

        /// <summary>
        /// Writes text.
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="text">Text.</param>
        /// <returns>Parcel.</returns>
        public Parcel WriteText(string resourceName, string text)
        {
            resourceName = PrepareResourceName(resourceName);

            File.WriteAllText(
                path: Path.Combine(workspace.FilesPath, resourceName),
                contents: text);

            return this;
        }

        /// <summary>
        /// Writes JSON object.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="value">Value.</param>
        /// <param name="options">Options.</param>
        /// <returns>Parcel.</returns>
        public Parcel WriteJson<T>(string resourceName, T value, JsonSerializerOptions? options = null)
        {
            resourceName = PrepareResourceName(resourceName);

            File.WriteAllText(
                path: Path.Combine(workspace.FilesPath, resourceName),
                contents: JsonSerializer.Serialize<T>(value, options));

            return this;
        }

        #endregion

        #region Read

        /// <summary>
        /// Reads text.
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        /// <returns>Text.</returns>
        public string ReadText(string resourceName)
        {
            resourceName = PrepareResourceName(resourceName);

            return File.ReadAllText(Path.Combine(workspace.FilesPath, resourceName));
        }

        /// <summary>
        /// Reads JSON object.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="resourceName">Name.</param>
        /// <param name="options">Options.</param>
        /// <returns>Value.</returns>
        public T? ReadJson<T>(string resourceName, JsonSerializerOptions? options = null)
        {
            resourceName = PrepareResourceName(resourceName);

            return JsonSerializer.Deserialize<T>(
                json: File.ReadAllText(Path.Combine(workspace.FilesPath, resourceName)),
                options: options);
        }

        #endregion

        #region Copy

        /// <summary>
        /// Copy file.
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="filePath">File path.</param>
        /// <returns>Parcel.</returns>
        public Parcel CopyFile(string resourceName, string filePath)
        {
            resourceName = PrepareResourceName(resourceName);
            filePath = PathHelper.MakeRoot(filePath);

            File.Copy(
                sourceFileName: filePath,
                destFileName: Path.Combine(workspace.FilesPath, resourceName));

            return this;
        }
        
        /// <summary>
        /// Copy files.
        /// </summary>
        /// <param name="filesPath">Files path.</param>
        /// <param name="requestHandler">Request handler.</param>
        /// <returns>Parcel.</returns>
        public Parcel CopyFiles(string filesPath, Action<IFileCopyRequest> requestHandler)
        {
            filesPath = PathHelper.MakeRoot(filesPath);

            if (!Directory.Exists(filesPath))
                throw new Exception("Files path does not exist.");

            foreach (var filePath in Directory.GetFiles(filesPath))
            {
                var request = new FileCopyRequest(filePath);

                requestHandler(request);

                if (request.Accept)
                {
                    if (string.IsNullOrWhiteSpace(request.ResourceName))
                        throw new Exception("Resource name not set.");

                    request.ResourceName = PrepareResourceName(request.ResourceName);

                    CopyFile(request.ResourceName, filePath);
                }
            }

            return this;
        }

        /// <summary>
        /// Copy from parcel.
        /// </summary>
        /// <param name="parcel">Parcel.</param>
        /// <param name="resourceName">Name.</param>
        /// <returns>Parcel.</returns>
        public Parcel CopyFromParcel(Parcel parcel, string resourceName)
        {
            resourceName = PrepareResourceName(resourceName);

            File.Copy(
                sourceFileName: Path.Combine(parcel.workspace.FilesPath, resourceName),
                destFileName: Path.Combine(workspace.FilesPath, resourceName));

            return this;
        }

        /// <summary>
        /// Copy all from parcel.
        /// </summary>
        /// <param name="parcel">Parcel to copy from.</param>
        /// <returns>Parcel.</returns>
        public Parcel CopyAllFromParcel(Parcel parcel)
        {
            var items = GetResourceNames();

            foreach(var item in items)
            {
                CopyFromParcel(parcel, item);
            }

            return this;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes resource.
        /// </summary>
        /// <param name="resourceName">Name.</param>
        /// <returns>Parcel.</returns>
        public Parcel Delete(string resourceName)
        {
            resourceName = PrepareResourceName(resourceName);

            File.Delete(Path.Combine(workspace.FilesPath, resourceName));
            return this;
        }

        /// <summary>
        /// Deletes all resources.
        /// </summary>
        /// <returns>Parcel.</returns>
        public Parcel DeleteAll()
        {
            var items = GetResourceNames();

            foreach (var item in items)
            {
                Delete(item);
            }

            return this;
        }

        #endregion

        #region Open

        /// <summary>
        /// Open file stream.
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        /// <returns>File stream.</returns>
        public FileStream OpenFile(string resourceName)
        {
            resourceName = PrepareResourceName(resourceName);

            return new FileStream(
                path: Path.Combine(workspace.FilesPath, resourceName),
                mode: FileMode.OpenOrCreate,
                access: FileAccess.ReadWrite,
                share: FileShare.Write);
        }

        /// <summary>
        /// Open file stream read only.
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        /// <returns>File stream.</returns>
        public FileStream OpenFileReadOnly(string resourceName)
        {
            resourceName = PrepareResourceName(resourceName);

            return new FileStream(
                path: Path.Combine(workspace.FilesPath, resourceName),
                mode: FileMode.Open,
                access: FileAccess.Read,
                share: FileShare.Read);
        }

        /// <summary>
        /// Open file stream write only.
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        /// <returns>File stream.</returns>
        public FileStream OpenFileWriteOnly(string resourceName)
        {
            resourceName = PrepareResourceName(resourceName);

            return new FileStream(
                path: Path.Combine(workspace.FilesPath, resourceName),
                mode: FileMode.OpenOrCreate,
                access: FileAccess.Write,
                share: FileShare.Write);
        }

        #endregion

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            workspace.Dispose();
        }
    }
}
