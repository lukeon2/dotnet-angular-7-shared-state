using VendingApp.Infrastructure.Helpers;
using Xunit;

namespace VendingApp.Test
{
    public class FileImportHelperTest
    {
        [Fact]
        public void GetRate_WhenInventoryBaseRateSameAsSelected_ReturnsBaseRate()
        {
            var a1 = FileImportHelper.GenerateProductNr(0);
            Assert.True(a1 == "A1");

            var b10 = FileImportHelper.GenerateProductNr(19);
            Assert.True(b10 == "B10");

            var a10 = FileImportHelper.GenerateProductNr(9);
            Assert.True(a10 == "A10");

            var b4 = FileImportHelper.GenerateProductNr(13);
            Assert.True(b4 == "B4");

            var d9 = FileImportHelper.GenerateProductNr(48);
            Assert.True(d9 == "E9");

            var b6 = FileImportHelper.GenerateProductNr(25);
            Assert.True(b6 == "C6");

            var c6 = FileImportHelper.GenerateProductNr(35);
            Assert.True(c6 == "D6");
        }
    }
}
