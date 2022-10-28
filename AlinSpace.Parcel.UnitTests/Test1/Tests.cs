using FluentAssertions;

namespace AlinSpace.Parcel.UnitTests.Test1
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            using (var parcel = Parcel.New("Test1/Workspace1"))
            {
                parcel.CopyFile("test1", "Test1/File1.txt");
                parcel.CopyFile("test2", "Test1/File2.txt");

                parcel.Pack("Test1/test.parcel");
            }

            using (var parcel = Parcel.New("Test1/Workspace2"))
            {
                parcel.Unpack("Test1/test.parcel");

                parcel.ReadText("test1").Should().Be("123");
                parcel.ReadText("test2").Should().Be("abc");
            }
        }
    }
}