namespace AlinSpace.Parcel
{
    public sealed class ParcelVersionUnsupportedException : Exception
    {
        public Version CurrentVersion { get; }

        public Version ParcelVersion { get; }

        public ParcelVersionUnsupportedException(Version currentVersion, Version parcelVersion) 
            : base($"Parcel version {parcelVersion} does not support current version {currentVersion}.")
        {
            CurrentVersion = currentVersion;
            ParcelVersion = parcelVersion;
        }
    }
}
