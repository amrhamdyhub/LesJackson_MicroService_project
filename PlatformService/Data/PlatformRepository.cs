using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly DbContext _context;

        public PlatformRepository(DbContext dbContext)
        {
            _context = dbContext;
        }
        public void CreatePlatform(Platform plat)
        {
            // Check if the platform is null
            if (plat == null)
            {
                throw new System.ArgumentNullException(nameof(plat));
            }
            // Add the platform to the context  
            _context.Set<Platform>().Add(plat);
            // Save the changes
            SaveChanges();
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Set<Platform>().ToList();
        }

        public Platform GetPlatformById(int id)
        {
            return _context.Set<Platform>().SingleOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
