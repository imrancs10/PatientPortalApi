using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace OnSolve.Mobile.Data
{
    public class OnSolveMobileContextFactory : IDesignTimeDbContextFactory<OnSolveMobileContext>
    {
        public OnSolveMobileContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<OnSolveMobileContext>();
            builder.UseSqlServer("Server= .\\;Database=OnSolveMobile;Trusted_Connection=True;",
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(OnSolveMobileContextFactory).GetTypeInfo().Assembly.GetName().Name));

            return new OnSolveMobileContext(builder.Options);
        }
    }
}
