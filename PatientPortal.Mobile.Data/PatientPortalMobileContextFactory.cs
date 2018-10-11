using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace PatientPortal.Mobile.Data
{
    public class PatientPortalMobileContextFactory : IDesignTimeDbContextFactory<PatientPortalMobileContext>
    {
        public PatientPortalMobileContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PatientPortalMobileContext>();
            builder.UseSqlServer("Server= .\\;Database=PatientPortal;Trusted_Connection=True;",
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(PatientPortalMobileContextFactory).GetTypeInfo().Assembly.GetName().Name));

            return new PatientPortalMobileContext(builder.Options);
        }
    }
}
