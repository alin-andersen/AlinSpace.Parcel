namespace AlinSpace.Parcel
{
    /// <summary>
    /// Represents the versioning.
    /// </summary>
    public abstract class Versioning : IVersioning
    {
        public Version Version { get; private set; }

        public Versioning(Version version)
        {
            Version = version;
        }

        public Versioning(string version)
        {
            Version = ConvertVersion(version);
        }

        static Version ConvertVersion(string version)
        {
            var v = version?.Split('.');

            return new Version(
                major: Convert.ToInt32(v?.FirstOrDefault()?.Trim() ?? "0"),
                minor: Convert.ToInt32(v?.Skip(1).FirstOrDefault()?.Trim() ?? "0"),
                build: Convert.ToInt32(v?.Skip(2).FirstOrDefault()?.Trim() ?? "0"));
        }

        public abstract void ThrowIfUnsupported(Version? parcelVersion);

        #region Any

        private class AnyImpl : Versioning
        {
            public AnyImpl(Version version) : base(version) { }

            public override void ThrowIfUnsupported(Version? parcelVersion)
            {
            }
        }

        public static Versioning Any()
        {
            return new AnyImpl(new Version());
        }

        #endregion

        #region HigherThan

        private class HigherThanImpl : Versioning
        {
            public HigherThanImpl(Version version) : base(version) { }

            public override void ThrowIfUnsupported(Version? parcelVersion)
            {
                if (parcelVersion <= Version)
                {
                    throw new ParcelVersionUnsupportedException(Version, parcelVersion);
                }
            }
        }

        public static Versioning HigherThan(Version version)
        {
            return new HigherThanImpl(version);
        }

        public static Versioning HigherThan(int major = 0, int minor = 0, int build = 0)
        {
            return new HigherThanImpl(new Version(major, minor, build));
        }

        public static Versioning HigherThan(string version)
        {
            return new HigherThanImpl(ConvertVersion(version));
        }

        #endregion

        #region HigherOrEqual

        private class HigherOrEqualThanImpl : Versioning
        {
            public HigherOrEqualThanImpl(Version version) : base(version) { }

            public override void ThrowIfUnsupported(Version? parcelVersion)
            {
                if (parcelVersion < Version)
                {
                    throw new ParcelVersionUnsupportedException(Version, parcelVersion);
                }
            }
        }

        public static Versioning HigherOrEqual(Version version)
        {
            return new HigherOrEqualThanImpl(version);
        }

        public static Versioning HigherOrEqual(int major = 0, int minor = 0, int build = 0)
        {
            return new HigherOrEqualThanImpl(new Version(major, minor, build));
        }

        public static Versioning HigherOrEqual(string version)
        {
            return new HigherOrEqualThanImpl(ConvertVersion(version));
        }

        #endregion
    }
}
