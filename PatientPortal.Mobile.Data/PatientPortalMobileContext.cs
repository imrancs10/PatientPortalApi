using Microsoft.EntityFrameworkCore;
using PatientPortal.Mobile.Data.Entites;

namespace PatientPortal.Mobile.Data
{
    public class PatientPortalMobileContext : DbContext
    {
        public PatientPortalMobileContext(DbContextOptions<PatientPortalMobileContext> options) : base(options)
        { }

        public virtual DbSet<AppointmentInfo> AppointmentInfo { get; set; }
        public virtual DbSet<AppointmentSetting> AppointmentSetting { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<DayMaster> DayMaster { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Doctor> Doctor { get; set; }
        public virtual DbSet<DoctorLeave> DoctorLeave { get; set; }
        public virtual DbSet<DoctorSchedule> DoctorSchedule { get; set; }
        public virtual DbSet<GblMasterLogin> GblMasterLogin { get; set; }
        public virtual DbSet<GblMasterUser> GblMasterUser { get; set; }
        public virtual DbSet<HospitalDetail> HospitalDetail { get; set; }
        public virtual DbSet<LabReport> LabReport { get; set; }
        public virtual DbSet<MeridiemMaster> MeridiemMaster { get; set; }
        public virtual DbSet<PatientBillReport> PatientBillReport { get; set; }
        public virtual DbSet<PatientInfo> PatientInfo { get; set; }
        public virtual DbSet<PatientLabReport> PatientLabReport { get; set; }
        public virtual DbSet<PatientLoginEntry> PatientLoginEntry { get; set; }
        public virtual DbSet<PatientLoginHistory> PatientLoginHistory { get; set; }
        public virtual DbSet<PatientTransaction> PatientTransaction { get; set; }
        public virtual DbSet<State> State { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\;Database=PatientPortal;user id=sa;password=Passw0rd;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppointmentInfo>(entity =>
            {
                entity.HasKey(e => e.AppointmentId);

                entity.Property(e => e.AppointmentDateFrom).HasColumnType("datetime");

                entity.Property(e => e.AppointmentDateTo).HasColumnType("datetime");

                entity.Property(e => e.CancelDate).HasColumnType("datetime");

                entity.Property(e => e.CancelReason).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.AppointmentInfo)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppointmentInfo_Doctor");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.AppointmentInfo)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppointmentInfo_PatientInfo");
            });

            modelBuilder.Entity<AppointmentSetting>(entity =>
            {
                entity.Property(e => e.AppointmentMessage).HasMaxLength(500);

                entity.Property(e => e.AutoCancelMessage).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.CityId).ValueGeneratedNever();

                entity.Property(e => e.CityName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.State)
                    .WithMany(p => p.City)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_City_State");
            });

            modelBuilder.Entity<DayMaster>(entity =>
            {
                entity.HasKey(e => e.DayId);

                entity.Property(e => e.DayName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.DepartmentId)
                    .HasColumnName("DepartmentID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.Property(e => e.DoctorId).HasColumnName("DoctorID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Degree)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Designation)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DoctorName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Doctor)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Doctor_Doctor");
            });

            modelBuilder.Entity<DoctorLeave>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LeaveDate).HasColumnType("date");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.DoctorLeave)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DoctorLeave_Doctor");
            });

            modelBuilder.Entity<DoctorSchedule>(entity =>
            {
                entity.Property(e => e.DoctorScheduleId).HasColumnName("DoctorScheduleID");

                entity.Property(e => e.DayId).HasColumnName("DayID");

                entity.Property(e => e.DoctorId).HasColumnName("DoctorID");

                entity.Property(e => e.TimeFromMeridiemId).HasColumnName("TimeFromMeridiemID");

                entity.Property(e => e.TimeToMeridiemId).HasColumnName("TimeToMeridiemID");

                entity.HasOne(d => d.Day)
                    .WithMany(p => p.DoctorSchedule)
                    .HasForeignKey(d => d.DayId)
                    .HasConstraintName("FK_DoctorScheduleDay_DayMaster1");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.DoctorSchedule)
                    .HasForeignKey(d => d.DoctorId)
                    .HasConstraintName("FK_DoctorScheduleDay_DoctorSchedule");

                entity.HasOne(d => d.TimeFromMeridiem)
                    .WithMany(p => p.DoctorScheduleTimeFromMeridiem)
                    .HasForeignKey(d => d.TimeFromMeridiemId)
                    .HasConstraintName("FK_DoctorScheduleDay_MeridiemMaster");

                entity.HasOne(d => d.TimeToMeridiem)
                    .WithMany(p => p.DoctorScheduleTimeToMeridiem)
                    .HasForeignKey(d => d.TimeToMeridiemId)
                    .HasConstraintName("FK_DoctorScheduleDay_MeridiemMaster1");
            });

            modelBuilder.Entity<GblMasterLogin>(entity =>
            {
                entity.HasKey(e => e.LoginId);

                entity.ToTable("Gbl_Master_Login");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.ModifiedAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.GblMasterLogin)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Gbl_Master_Login_Gbl_Master_User");
            });

            modelBuilder.Entity<GblMasterUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("Gbl_Master_User");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DoB).HasColumnType("datetime");

                entity.Property(e => e.EmailId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.IsdCode)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MiddleName).HasMaxLength(50);

                entity.Property(e => e.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(13);

                entity.Property(e => e.ModifiedAt).HasColumnType("datetime");

                entity.Property(e => e.PasswordHash).HasMaxLength(250);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<HospitalDetail>(entity =>
            {
                entity.Property(e => e.HospitalName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LabReport>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FileName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ReportName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.LabReport)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK_LabReport_PatientInfo");
            });

            modelBuilder.Entity<MeridiemMaster>(entity =>
            {
                entity.HasKey(e => e.MeridiemId);

                entity.Property(e => e.MeridiemId).HasColumnName("MeridiemID");

                entity.Property(e => e.MeridiemValue)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PatientBillReport>(entity =>
            {
                entity.HasKey(e => e.BillId);

                entity.Property(e => e.BillDate).HasColumnType("datetime");

                entity.Property(e => e.BillNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BillType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.ReportUrl).HasMaxLength(500);

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientBillReport)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__PatientBi__Patie__3A4CA8FD");
            });

            modelBuilder.Entity<PatientInfo>(entity =>
            {
                entity.HasKey(e => e.PatientId);

                entity.Property(e => e.PatientId).ValueGeneratedOnAdd();

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Crnumber)
                    .HasColumnName("CRNumber")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FatherOrHusbandName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MaritalStatus)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Otp)
                    .HasColumnName("OTP")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RegistrationNumber)
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.Religion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ResetCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ValidUpto).HasColumnType("datetime");

                entity.HasOne(d => d.CityNavigation)
                    .WithMany(p => p.PatientInfo)
                    .HasForeignKey(d => d.City)
                    .HasConstraintName("FK_PatientInfo_City");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.PatientInfo)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_PatientInfo_Department");

                entity.HasOne(d => d.Patient)
                    .WithOne(p => p.InversePatient)
                    .HasForeignKey<PatientInfo>(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PatientInfo_PatientInfo");

                entity.HasOne(d => d.StateNavigation)
                    .WithMany(p => p.PatientInfo)
                    .HasForeignKey(d => d.State)
                    .HasConstraintName("FK_PatientInfo_State");
            });

            modelBuilder.Entity<PatientLabReport>(entity =>
            {
                entity.HasKey(e => e.ReferanceId);

                entity.Property(e => e.BillNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LabName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.RefNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ReportDate).HasColumnType("datetime");

                entity.Property(e => e.ReportUrl).HasMaxLength(500);

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientLabReport)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__PatientLa__Patie__3B40CD36");
            });

            modelBuilder.Entity<PatientLoginEntry>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.LoginAttemptDate).HasColumnType("datetime");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientLoginEntry)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PatientLoginEntry_PatientInfo");
            });

            modelBuilder.Entity<PatientLoginHistory>(entity =>
            {
                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPAddress")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LoginDate).HasColumnType("datetime");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientLoginHistory)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK_PatientLoginHistory_PatientInfo");
            });

            modelBuilder.Entity<PatientTransaction>(entity =>
            {
                entity.Property(e => e.OrderId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ResponseCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StatusCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientTransaction)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK_PatientTransaction_PatientInfo");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.Property(e => e.StateName).HasMaxLength(500);
            });
        }
    }
}
