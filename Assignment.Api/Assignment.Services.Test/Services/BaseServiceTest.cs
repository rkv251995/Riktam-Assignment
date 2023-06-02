using Assignment.Infrastructures.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Assignment.Services.Test.Services
{
    public class BaseServiceTest
    {
        protected readonly DataContext _dataContext;

        public BaseServiceTest()
        {
            DbContextOptions<DataContext> options = new DbContextOptionsBuilder<DataContext>()
                     .UseInMemoryDatabase(databaseName: "AssignmentDatabase")
                     .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                     .Options;

            _dataContext = new DataContext(options);
        }
    }
}
