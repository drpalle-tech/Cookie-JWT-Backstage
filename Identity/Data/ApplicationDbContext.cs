using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data
{
    // This class is the root to establish Db Connection
    //As we inherited from IdentityDbCOntext, it will take care of all tables, constraits in db
    //We need not do anything apart from giving connection string as options to the base
    public class ApplicationDbContext : IdentityDbContext
    {
        //Pass options to base
        //Options can be found in the Program.cs
        //Options must have connection string.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
    }
}
