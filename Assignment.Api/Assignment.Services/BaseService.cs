using Assignment.Infrastructures.EntityFrameworkCore;

namespace Assignment.Services
{
    public class BaseService
    {
        protected readonly DataContext _dataContext;
        public BaseService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
    }
}
