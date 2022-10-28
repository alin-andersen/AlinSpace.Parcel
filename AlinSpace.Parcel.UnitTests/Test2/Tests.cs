using FluentAssertions;

namespace AlinSpace.Parcel.UnitTests.Test2
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            using (var parcel = Parcel.New("Test2/Workspace1"))
            {
                parcel.CopyFiles("Test2", request =>
                {
                    request.Accept = true;
                    request.ResourceName = Path.GetFileName(request.FilePath);
                });

                parcel.Pack("Test2/test.parcel");
            }

            using (var parcel = Parcel.New("Test2/Workspace2"))
            {
                parcel.Unpack("Test2/test.parcel");

                parcel.ReadText("File1.txt").Should().Be("123");
                parcel.ReadText("File2.txt").Should().Be("abc");

                parcel.GetResourceNames().Any(x => x == "File1.txt").Should().BeTrue();
                parcel.GetResourceNames().Any(x => x == "File2.txt").Should().BeTrue();
            }
        }
    }
}