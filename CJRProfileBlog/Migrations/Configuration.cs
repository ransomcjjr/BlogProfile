using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CJRProfileBlog.Models;
using System.Data.Entity.Migrations;
using System.Linq;
namespace CJRProfileBlog.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "ransomcjjr@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "ransomcjjr@gmail.com",
                    Email = "ransomcjjr@gmail.com",
                    DistplayName = "CJR-Admin"
                }, "change2016");
            }
            if (!context.Users.Any(u => u.Email == "cjohnransom@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "cjohnransom@gmail.com",
                    Email = "cjohnransom@gmail.com",
                    DistplayName = "CJR-Moderator"
                }, "change2016");
            }

            var userId = userManager.FindByEmail("ransomcjjr@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");
            var user_Id = userManager.FindByEmail("cjohnransom@gmail.com").Id;
            userManager.AddToRole(user_Id, "Moderator");
        }
    }
}
