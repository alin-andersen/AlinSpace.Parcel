namespace AlinSpace.Parcel.UnitTests.Versioning
{
    public class Tests
    {
        [Fact]
        public void Test()
        {
            using (var parcel = Parcel.New())
            {
                parcel.Specification.ParcelVersion = new Version(1, 2, 3);
                parcel.Pack("Versioning/parcel.parcel");
            }

            using (var parcel = Parcel.New())
            {
                parcel.Unpack("Versioning/parcel.parcel", versioning: AlinSpace.Parcel.Versioning.HigherThan(1, 2));
                parcel.Reset();

                Assert.Throws<ParcelVersionUnsupportedException>(() =>
                {
                    parcel.Unpack("Versioning/parcel.parcel", versioning: AlinSpace.Parcel.Versioning.HigherThan(2));
                });
            }
        }
    }
}
