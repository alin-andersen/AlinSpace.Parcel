namespace AlinSpace.Parcel
{
    /// <summary>
    /// Represents the workspace.
    /// </summary>
    public class Workspace : IDisposable
    {
        private readonly OneTimeSwitch ots = new();

        /// <summary>
        /// Gets the path to the workspace.
        /// </summary>
        public string PathToWorkspace { get; }

        /// <summary>
        /// Gets the files path.
        /// </summary>
        public string FilesPath { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pathToWorkspace">Path to workspace.</param>
        private Workspace(string pathToWorkspace)
        {
            PathToWorkspace = PathHelper.MakeRoot(pathToWorkspace);
            FilesPath = Path.Combine(PathToWorkspace, "files");

            Setup();
        }

        private void Setup()
        {
            ots.ThrowObjectDisposedIfSet<Workspace>();

            if (File.Exists(PathToWorkspace))
            {
                throw new Exception("Workspace path is a file.");
            }

            if (!Directory.Exists(PathToWorkspace))
            {
                Directory.CreateDirectory(PathToWorkspace);
            }

            if (File.Exists(FilesPath))
            {
                throw new Exception("Parcel files path is a file.");
            }

            if (!Directory.Exists(FilesPath))
            {
                Directory.CreateDirectory(FilesPath);
            }
        }

        /// <summary>
        /// Creates the workspace.
        /// </summary>
        /// <param name="pathToWorkspace">Path to workspace.</param>
        /// <returns>Workspace.</returns>
        public static Workspace Create(string pathToWorkspace)
        {
            return new Workspace(pathToWorkspace);
        }

        /// <summary>
        /// Resets the workspace.
        /// </summary>
        public void Reset()
        {
            ots.ThrowObjectDisposedIfSet<Workspace>();

            Directory.Delete(PathToWorkspace, true);
            Setup();
        }

        /// <summary>
        /// Resets the files of the workspace.
        /// </summary>
        public void ResetFiles()
        {
            ots.ThrowObjectDisposedIfSet<Workspace>();

            Directory.Delete(FilesPath, true);
            Setup();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            if (!ots.TrySet())
                return;

            try
            {
                Directory.Delete(PathToWorkspace, true);
            }
            catch
            {
                // ignore
            }
        }
    }
}
