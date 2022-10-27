using FluentAssertions;

namespace AlinSpace.Parcel.UnitTests.Test2
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            using var parcel = Parcel.New("Test2/Workspace");

            parcel.CopyFiles("Test2", request =>
            {
                request.Accept = true;
                request.Name = Path.GetFileName(request.FilePath);
            });

            parcel.Pack("Test2/test.parcel");

            parcel.Unpack("Test2/test.parcel");

            parcel.ReadText("File1.txt").Should().Be("123");
            parcel.ReadText("File2.txt").Should().Be("abc");

            parcel.GetNames().Any(x => x == "File1.txt").Should().BeTrue();
            parcel.GetNames().Any(x => x == "File2.txt").Should().BeTrue();
        }
    }
}