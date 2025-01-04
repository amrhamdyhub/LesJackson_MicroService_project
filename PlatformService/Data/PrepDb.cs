using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        // This method will seed the database with some initial data
        public static void PrepPopulation(IApplicationBuilder app)
        {    
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<IPlatformRepository>());
            }
        }
        private static void SeedData(IPlatformRepository platformRepository)
        {
            if (platformRepository.GetAllPlatforms().Any())
            {
                Console.WriteLine("--> We already have data");
            }
            else
            {
                Console.WriteLine("--> Seeding data...");
                // Add some platforms to the database
                platformRepository.CreatePlatform(new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" });
                platformRepository.CreatePlatform(new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" });
                platformRepository.CreatePlatform(new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" });
                // Save the changes
                platformRepository.SaveChanges(); 
            }
        }
    }
}
