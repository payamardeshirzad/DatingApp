using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
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

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _dataContext.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _dataContext.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            // AS photo is navigational property we need to use include to get access to the data: Eager Loading
            var user = await _dataContext.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            //  var users = await _dataContext.Users.Include(p => p.Photos).ToListAsync();
            // Filter users: opposite gender and exclude the user itself from the list of users
            var users = _dataContext.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);
            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            }
            if(!string.IsNullOrEmpty(userParams.OrderBy)){
                switch(userParams.OrderBy)
                {
                    case "created":
                    users = users.OrderByDescending(u=>u.Created);
                    break;
                    default: 
                    users=users.OrderByDescending(u=> u.LastActive);
                    break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _dataContext.SaveChangesAsync() > 0;

        }
    }
}