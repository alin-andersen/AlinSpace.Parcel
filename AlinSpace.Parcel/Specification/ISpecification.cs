namespace AlinSpace.Parcel
{
    public interface ISpecification
    {
        Version Version { get; set; }

        DateTime? CreationTimestamp { get; set; }

        #region Compression

        bool CompressFiles { get; set; }

        CompressAlgorithm CompressAlgorithm { get; set; }

        #endregion
    }
}
