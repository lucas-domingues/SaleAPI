using Xunit;
using Sales.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Sales.Tests.Tests
{
    public class SalesTests
    {
        [Fact]
        public void Test_Creating_Sale()
        {
            var options = new DbContextOptionsBuilder<SalesDbContext>()
                .UseInMemoryDatabase(databaseName: "SalesDB")
                .Options;

            using var context = new SalesDbContext(options);
            Assert.NotNull(context);
        }
    }
}
