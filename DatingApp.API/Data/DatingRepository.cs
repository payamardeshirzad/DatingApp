using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _dataContext;

        public DatingRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public void Add<T>(T Entity) where T : class
        {
            _dataContext.Add(Entity);
        }

        public void Delete<T>(T Entity) where T : class
        {
            _dataContext.Remove(Entity);
        }

        public async Task<User> GetUser(int id)
        {
            // AS photo is navigational property we need to use include to get access to the data: Eager Loading
            var user = await _dataContext.Users.Include(p  => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _dataContext.Users.Include(p => p.Photos).ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;

        }
    }
}