using Microsoft.EntityFrameworkCore;
using OnSolve.Mobile.Data.Entites;

namespace OnSolve.Mobile.Data
{
    public class OnSolveMobileContext : DbContext
    {
        public OnSolveMobileContext(DbContextOptions<OnSolveMobileContext> options) : base(options)
        { }

        public DbSet<ResetPasswordCode> ResetPasswordCode { get; set; }
        public DbSet<MobileUser> MobileUser { get; set; }
        public DbSet<MessageDetail> MessageDetails { get; set; }
        public DbSet<MobileRecipient> MobileRecipients { get; set; }
        public DbSet<FCMTokenInfo> FCMTokenInfo { get; set; }
        public DbSet<MessageSenderDetail> MessageSenderDetails { get; set; }
        public DbSet<EmailVerificationCode> EmailVerificationCode { get; set; }
        public DbSet<ConferenceBridgeDetail> ConferenceBridgeDetails { get; set; }
    }
}
