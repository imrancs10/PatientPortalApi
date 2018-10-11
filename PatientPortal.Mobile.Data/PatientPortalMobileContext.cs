using Microsoft.EntityFrameworkCore;

namespace PatientPortal.Mobile.Data
{
    public class PatientPortalMobileContext : DbContext
    {
        public PatientPortalMobileContext(DbContextOptions<PatientPortalMobileContext> options) : base(options)
        { }
    }
}
