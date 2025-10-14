using Microsoft.AspNetCore.Identity;
using VitalHelse.Models;

namespace VitalHelse.Data;

public class ApplicationDbInitializer
{
    public static void Initialize(ApplicationDbContext db, UserManager<AspNetUsers> um, RoleManager<IdentityRole> rm)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        // Roles for the website. No role = private customer
        rm.CreateAsync(new IdentityRole("Admin")).Wait();
        rm.CreateAsync(new IdentityRole("Staff")).Wait();
        rm.CreateAsync(new IdentityRole("BusinessCustomer")).Wait();

        // Test users for all roles
        var adminUser = new AspNetUsers
            { UserName = "admin@VitalHelse.no", Email = "admin@VitalHelse.no", EmailConfirmed = true };
        um.CreateAsync(adminUser, "Password1.").Wait();
        um.AddToRoleAsync(adminUser, "Admin").Wait();

        var staffUser = new AspNetUsers
            { UserName = "staff@VitalHelse.no", Email = "staff@VitalHelse.no", EmailConfirmed = true };
        um.CreateAsync(staffUser, "Password.1");
        um.AddToRoleAsync(staffUser, "Staff").Wait();
        
        var businessUser = new AspNetUsers
            { UserName = "business@VitalHelse.no", Email = "business@VitalHelse.no", EmailConfirmed = true };
        um.CreateAsync(businessUser, "Password.1");
        um.AddToRoleAsync(businessUser, "BusinessCustomer").Wait();

        var customerUser = new AspNetUsers
            { UserName = "business@VitalHelse.no", Email = "customer@VitalHelse.no", EmailConfirmed = true };
        um.CreateAsync(customerUser, "Password.1");
        
        db.SaveChanges();
    }
}