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
        private readonly DataContext _context;
        public DatingRepository(DataContext _context)
        {
            this._context = _context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoFromUser(int userId)
        {
            return await _context.Photos.Where(p => p.UserId == userId).FirstOrDefaultAsync(p => p.AsMainPhoto);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            return await _context.Users.Include(u => u.Photos).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(u => u.Photos).AsQueryable();

            var minimumDateOfBirth = DateTime.Today.AddYears(-userParams.MaximumAge - 1 ?? -150);
            var maximumDateOfBirth = DateTime.Today.AddYears(-userParams.MinimumAge ?? -18);

            // Filter
            users = users.Where(u => u.Id != userParams.UserId)
                         .Where(u => u.Gender == userParams.Gender)
                         .Where(u => u.DateOfBirth >= minimumDateOfBirth &&
                                     u.DateOfBirth <= maximumDateOfBirth);

            return await PagedList<User>.CreatePageAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}