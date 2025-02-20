using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Features.Theaters;
using Selu383.SP25.P02.Api.Features.Users;


namespace Selu383.SP25.P02.Api.Data
{
    public static class SeedTheaters
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new DataContext(serviceProvider.GetRequiredService<DbContextOptions<DataContext>>()))
            {
                // Look for any theaters.
                if (context.Theaters.Any())
                {
                    return;   // DB has been seeded
                }
                var bobId = context.Users
                    .Where(u => u.UserName == "bob")
                    .Select(u => u.Id)
                    .FirstOrDefault();

                if (bobId == 0)
                {
                    throw new Exception("User missing");
                }


                context.Theaters.AddRange(
                    new Theater
                    {
                        Name = "AMC Palace 10",
                        Address = "123 Main St, Springfield",
                        SeatCount = 150,
                        ManagerId = bobId
                    },
                    new Theater
                    {
                        Name = "Regal Cinema",
                        Address = "456 Elm St, Shelbyville",
                        SeatCount = 200
                    },
                    new Theater
                    {
                        Name = "Grand Theater",

                        Address = "789 Broadway Ave, Metropolis",
                        SeatCount = 300
                    },
                    new Theater
                    {
                        Name = "Vintage Drive-In",
                        Address = "101 Retro Rd, Smallville",
                        SeatCount = 75
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
