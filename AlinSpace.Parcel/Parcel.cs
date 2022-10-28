using System.IO.Compression;
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

        string PrepareName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            return name.Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workspacePath"></param>
        /// <returns></returns>
        public static Parcel New(string? workspacePath = null)
        {
            workspacePath = PrepareWorkspacePath(workspacePath);

            var workspace = Workspace.Create(workspacePath);
            var parcel = new Parcel(workspace);

            return parcel;
        }

        public static Parcel Open(string parcelFilePath, string? workspacePath = null)
        {
            workspacePath = PrepareWorkspacePath(workspacePath);

            var workspace = Workspace.Create(workspacePath);
            var parcel = new Parcel(workspace);

            parcel.Unpack(parcelFilePath);

            return parcel;
        }

        public void Pack(string filePath, bool resetAfterPacking = true)
        {
            filePath = PathHelper.MakeRoot(filePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            WriteSpecificationToWorkspace();
            WriteMetadataToWorkspace();

            ZipFile.CreateFromDirectory(workspace.PathToWorkspace, filePath);

            if (resetAfterPacking)
            {
                workspace.Reset();
            }
        }

        public void Unpack(string filePath, bool resetBeforeUnpacking = true)
        {
            if (resetBeforeUnpacking)
            {
                workspace.Reset();
            }

            filePath = PathHelper.MakeRoot(filePath);
            ZipFile.ExtractToDirectory(filePath, workspace.PathToWorkspace);

            ReadSpecificationFromWorkspace();
            ReadMetadataFromWorkspace();
        }

        public void Reset()
        {
            workspace.Reset();
        }

        #region Specification

        public ISpecification Specification { get; private set; } = new Specification();

        void ReadSpecificationFromWorkspace()
        {
            var specificationFilePath = Path.Combine(workspace.FilesPath, "specification.json");
            Specification = AlinSpace.Parcel.Specification.ReadFromJsonFile(specificationFilePath);
        }

        void WriteSpecificationToWorkspace()
        {
            var specificationFilePath = Path.Combine(workspace.FilesPath, "specification.json");
            AlinSpace.Parcel.Specification.WriteFromJsonFile(specificationFilePath, Specification);
        }

        #endregion

        #region Metadata

        public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        void ReadMetadataFromWorkspace()
        {
            var metadataFilePath = Path.Combine(workspace.PathToWorkspace, "metadata.json");

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
            var metadataFilePath = Path.Combine(workspace.PathToWorkspace, "metadata.json");
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

        public IEnumerable<string> GetNames()
        {
            return Directory
                .GetFiles(workspace.FilesPath)
                .Select(x => Path.GetFileName(x));
        }

        #region Write

        /// <summary>
        /// Writes text.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="text">Text.</param>
        /// <returns>Parcel.</returns>
        public Parcel WriteText(string name, string text)
        {
            name = PrepareName(name);

            File.WriteAllText(
                path: Path.Combine(workspace.FilesPath, name),
                contents: text);

            return this;
        }

        /// <summary>
        /// Writes JSON object.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="options">Options.</param>
        /// <returns>Parcel.</returns>
        public Parcel WriteJson<T>(string name, T value, JsonSerializerOptions? options = null)
        {
            name = PrepareName(name);

            File.WriteAllText(
                path: Path.Combine(workspace.FilesPath, name),
                contents: JsonSerializer.Serialize<T>(value, options));

            return this;
        }

        #endregion

        #region Read

        /// <summary>
        /// Reads text.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>Text.</returns>
        public string ReadText(string name)
        {
            name = PrepareName(name);

            return File.ReadAllText(Path.Combine(workspace.FilesPath, name));
        }

        /// <summary>
        /// Reads JSON object.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="name">Name.</param>
        /// <param name="options">Options.</param>
        /// <returns>Value.</returns>
        public T? ReadJson<T>(string name, JsonSerializerOptions? options = null)
        {
            name = PrepareName(name);

            return JsonSerializer.Deserialize<T>(
                json: File.ReadAllText(Path.Combine(workspace.FilesPath, name)),
                options: options);
        }

        #endregion

        #region Copy

        /// <summary>
        /// Copy file.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="filePath">File path.</param>
        /// <returns>Parcel.</returns>
        public Parcel CopyFile(string name, string filePath)
        {
            name = PrepareName(name);
            filePath = PathHelper.MakeRoot(filePath);

            File.Copy(
                sourceFileName: filePath,
                destFileName: Path.Combine(workspace.FilesPath, name));

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
                    if (string.IsNullOrWhiteSpace(request.Name))
                        throw new Exception("Name not valid.");

                    request.Name = PrepareName(request.Name);

                    CopyFile(request.Name, filePath);
                }
            }

            return this;
        }

        /// <summary>
        /// Copy from parcel.
        /// </summary>
        /// <param name="parcel">Parcel.</param>
        /// <param name="name">Name.</param>
        /// <returns>Parcel.</returns>
        public Parcel CopyFromParcel(Parcel parcel, string name)
        {
            name = PrepareName(name);

            File.Copy(
                sourceFileName: Path.Combine(parcel.workspace.FilesPath, name),
                destFileName: Path.Combine(workspace.FilesPath, name));

            return this;
        }

        /// <summary>
        /// Copy all from parcel.
        /// </summary>
        /// <param name="parcel">Parcel to copy from.</param>
        /// <returns>Parcel.</returns>
        public Parcel CopyAllFromParcel(Parcel parcel)
        {
            var items = GetNames();

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
        /// <param name="name">Name.</param>
        /// <returns>Parcel.</returns>
        public Parcel Delete(string name)
        {
            name = PrepareName(name);

            File.Delete(Path.Combine(workspace.FilesPath, name));
            return this;
        }

        /// <summary>
        /// Deletes all resources.
        /// </summary>
        /// <returns>Parcel.</returns>
        public Parcel DeleteAll()
        {
            var items = GetNames();

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
        /// <param name="name">Name.</param>
        /// <returns>File stream.</returns>
        public FileStream OpenFile(string name)
        {
            name = PrepareName(name);

            return new FileStream(
                path: Path.Combine(workspace.FilesPath, name),
                mode: FileMode.OpenOrCreate,
                access: FileAccess.ReadWrite,
                share: FileShare.Write);
        }

        /// <summary>
        /// Open file stream read only.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>File stream.</returns>
        public FileStream OpenFileReadOnly(string name)
        {
            name = PrepareName(name);

            return new FileStream(
                path: Path.Combine(workspace.FilesPath, name),
                mode: FileMode.Open,
                access: FileAccess.Read,
                share: FileShare.Read);
        }

        /// <summary>
        /// Open file stream write only.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>File stream.</returns>
        public FileStream OpenFileWriteOnly(string name)
        {
            name = PrepareName(name);

            return new FileStream(
                path: Path.Combine(workspace.FilesPath, name),
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
