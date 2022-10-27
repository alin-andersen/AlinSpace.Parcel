using FluentAssertions;

namespace AlinSpace.Parcel.UnitTests.Test3
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            using var parcel = Parcel.New("Test3/Workspace");

            parcel.Metadata["Test"] = "Test";

            parcel.Pack("Test3/test.parcel");

            parcel.Unpack("Test3/test.parcel");

            parcel.Metadata["Test"].Should().Be("Test");
        }
    }
}