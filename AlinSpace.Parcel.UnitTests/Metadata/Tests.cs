using FluentAssertions;

namespace AlinSpace.Parcel.UnitTests.Metadata
{
    public class Tests
    {
        [Fact]
        public void Write_Read()
        {
            using (var parcel = Parcel.New())
            {
                parcel.Metadata["Test"] = "Test";
                parcel.Pack("Metadata/test.parcel");
            }

            using (var parcel = Parcel.New())
            {
                parcel.Unpack("Metadata/test.parcel");

                parcel.Metadata["Test"].Should().Be("Test");
            }
        }

        [Fact]
        public void Copy_From_Other_Parcel()
        {
            using var parcel1 = Parcel.New();
            parcel1.Metadata["Test"] = "Test";
            parcel1.Pack("Test3/test.parcel");

            using (var parcel2 = Parcel.New())
            {
                parcel2.CopyMetadataFromParcel(parcel1);
                parcel2.Metadata["Test"].Should().Be("Test");
            }
        }

        [Fact]
        public void Clear()
        {
            using var parcel1 = Parcel.New();
            parcel1.Metadata["Test"] = "Test";
            parcel1.Pack("Test3/test.parcel");

            using var parcel2 = Parcel.New();
            parcel2.Metadata.Clear();
            parcel2.Pack("Test3/test.parcel");

            using var parcel3 = Parcel.New();
            parcel3.Unpack("Test3/test.parcel");

            parcel3.Metadata.Count.Should().Be(0);
        }
    }
}