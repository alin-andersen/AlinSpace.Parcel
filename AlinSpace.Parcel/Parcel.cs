using System.IO.Compression;
using System.Text.Json;

namespace AlinSpace.Parcel
{
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
            return name?.Trim();
        }

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
        }

        public void Reset()
        {
            workspace.Reset();
        }

        public IEnumerable<string> GetNames()
        {
            return Directory
                .GetFiles(workspace.FilesPath)
                .Select(x => Path.GetFileName(x));
        }

        #region Write

        public Parcel WriteText(string name, string text)
        {
            name = PrepareName(name);

            File.WriteAllText(
                path: Path.Combine(workspace.FilesPath, name),
                contents: text);

            return this;
        }

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

        public string ReadText(string name)
        {
            name = PrepareName(name);

            return File.ReadAllText(Path.Combine(workspace.FilesPath, name));
        }

        public T? ReadJson<T>(string name, JsonSerializerOptions? options = null)
        {
            name = PrepareName(name);

            return JsonSerializer.Deserialize<T>(
                json: File.ReadAllText(Path.Combine(workspace.FilesPath, name)),
                options: options);
        }

        #endregion

        #region Copy

        public Parcel CopyFile(string name, string filePath)
        {
            name = PrepareName(name);
            filePath = PathHelper.MakeRoot(filePath);

            File.Copy(
                sourceFileName: filePath,
                destFileName: Path.Combine(workspace.FilesPath, name));

            return this;
        }

        public Parcel CopyFiles(string path, Action<FileCopyRequest> onRequest)
        {
            path = PathHelper.MakeRoot(path);

            foreach (var filePath in Directory.GetFiles(path))
            {
                var request = new FileCopyRequest(filePath);

                onRequest(request);

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

        public Parcel CopyFromParcel(Parcel parcel, string name)
        {
            name = PrepareName(name);

            File.Copy(
                sourceFileName: Path.Combine(parcel.workspace.FilesPath, name),
                destFileName: Path.Combine(workspace.FilesPath, name));

            return this;
        }

        #endregion

        #region Remove

        public Parcel Delete(string name)
        {
            name = PrepareName(name);

            File.Delete(Path.Combine(workspace.FilesPath, name));
            return this;
        }

        #endregion

        #region Open

        public FileStream OpenFile(string name)
        {
            name = PrepareName(name);

            return new FileStream(
                path: Path.Combine(workspace.FilesPath, name),
                mode: FileMode.OpenOrCreate,
                access: FileAccess.ReadWrite,
                share: FileShare.Write);
        }

        public FileStream OpenFileReadOnly(string name)
        {
            name = PrepareName(name);

            return new FileStream(
                path: Path.Combine(workspace.FilesPath, name),
                mode: FileMode.Open,
                access: FileAccess.Read,
                share: FileShare.Read);
        }

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

        public void Dispose()
        {
            workspace.Dispose();
        }
    }
}
