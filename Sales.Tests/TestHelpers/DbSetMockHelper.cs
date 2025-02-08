using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Tests.TestHelpers
{
    public static class DbSetMockHelper
    {
        public static Mock<DbSet<T>> CreateMockDbSet<T>(this IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }
    }
}
