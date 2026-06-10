using System;
using System.Collections.Generic;
using System.Text;
using FutbolLeague.Domain; // For the User class
using FutbolLeague.Infrastructure.Data; // For the AppContext class
using Microsoft.EntityFrameworkCore; // For the Include method

namespace FutbolLeague.Infrastructure.Seeds
{
    public static class DBSeeds
    {
        public static async Task SeedAdminAsync(AppDbContext context)
        {
            var adminExists = await context.Users
                .AnyAsync(u => u.Role == UserRole.Admin);

            if (adminExists) 
                return;

            var admin = new User
            {
                UserName = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // Hash the password
                Role = UserRole.Admin
            };

            context.Users.Add(admin);

            await context.SaveChangesAsync(); // Save the new admin user to the database
        }
    }
}
