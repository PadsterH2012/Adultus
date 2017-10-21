using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //REFME Database.SetInitializer(new MySqlInitializer());//Initalize in mysql database
            string[] typeNames = { "admin", "client", "member" };
            var TypeManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this));
            IdentityResult roleResult;
            foreach (var typeName in typeNames)
            {
                if (!TypeManager.RoleExists(typeName))
                {
                    roleResult = TypeManager.Create(new IdentityRole(typeName));
                }
            }
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<WebApplication.userinfo> userinfoes { get; set; }
    }
}