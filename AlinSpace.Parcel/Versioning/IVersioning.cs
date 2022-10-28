namespace AlinSpace.Parcel
{
    /// <summary>
    /// Represents the versioning.
    /// </summary>
    public interface IVersioning
    {
        /// <summary>
        /// Thorw if the given parcel version is not supported.
        /// </summary>
        /// <param name="parcelVersion">Parcel version.</param>
        void ThrowIfUnsupported(Version? parcelVersion);
    }
}
