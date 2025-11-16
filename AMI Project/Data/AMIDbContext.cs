using AMI_Project.Data.Models;
using AMI_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace AMI_Project.Data
{
    public partial class AMIDbContext : DbContext
    {
        public AMIDbContext() { }

        public AMIDbContext(DbContextOptions<AMIDbContext> options)
            : base(options) { }

        // ------------------ DbSets ------------------
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<BillDetail> BillDetails { get; set; }
        public virtual DbSet<Consumer> Consumers { get; set; }
        public virtual DbSet<Meter> Meters { get; set; }
        public virtual DbSet<MeterReading> MeterReadings { get; set; }
        public virtual DbSet<MonthlyMeterReading> MonthlyMeterReadings { get; set; }
        public virtual DbSet<OrgUnit> OrgUnits { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Tariff> Tariffs { get; set; }
        public virtual DbSet<TariffSlab> TariffSlabs { get; set; }
        public virtual DbSet<User> Users { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(
        //        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AMI_DB;Integrated Security=True;Trust Server Certificate=True"
        //    );
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ------------------ Meter ------------------
            modelBuilder.Entity<Meter>(entity =>
            {
                entity.HasKey(e => e.MeterSerialNo).HasName("PK__Meter__5C498B0F56EE4CAA");

                entity.ToTable("Meter");

                entity.Property(e => e.MeterSerialNo).HasMaxLength(50);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.Firmware).HasMaxLength(50);
                entity.Property(e => e.Iccid)
                    .HasMaxLength(30)
                    .HasColumnName("ICCID");
                entity.Property(e => e.Imsi)
                    .HasMaxLength(30)
                    .HasColumnName("IMSI");
                entity.Property(e => e.InstallTsUtc).HasPrecision(3);
                entity.Property(e => e.Manufacturer).HasMaxLength(100);
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValue("Active");

                entity.Property(e => e.IpAddress).HasMaxLength(45);

                entity.HasIndex(e => e.IpAddress)
                      .IsUnique()
                      .HasFilter("[IpAddress] IS NOT NULL")
                      .HasDatabaseName("UX_Meter_IpAddress");

            });

            // ------------------ MeterReading ------------------
            modelBuilder.Entity<MeterReading>(entity =>
            {
                entity.HasKey(e => e.MeterReadingId);
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("(sysutcdatetime())");
            });

            // ------------------ MonthlyMeterReading ------------------


            modelBuilder.Entity<MonthlyMeterReading>(entity =>
            {
                entity.HasKey(e => e.MonthlyMeterReadingId).HasName("PK__MonthlyM__67F98BA529A4227F");

                entity.ToTable("MonthlyMeterReading");

                entity.Property(e => e.MeterSerialNo).HasMaxLength(50);
                entity.Property(e => e.TotalConsumptionKwh).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.MeterSerialNoNavigation).WithMany(p => p.MonthlyMeterReadings)
                    .HasForeignKey(d => d.MeterSerialNo)
                    .HasConstraintName("FK_MonthlyMeterReading_Meter");
            });

            // ------------------ OrgUnit ------------------
            modelBuilder.Entity<OrgUnit>(entity =>
            {
                entity.ToTable("OrgUnit"); // Matches your existing table exactly

                entity.HasKey(e => e.OrgUnitId);
                entity.Property(e => e.Type).HasMaxLength(20).IsUnicode(false);
                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasOne(e => e.Parent)
                      .WithMany(p => p.InverseParent)
                      .HasForeignKey(e => e.ParentId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_OrgUnit_Parent");
            });

            // ------------------ Consumer ------------------
            modelBuilder.Entity<Consumer>(entity =>
            {
                entity.HasKey(e => e.ConsumerId);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
                entity.Property(e => e.Status).HasDefaultValue("Active");

                entity.HasOne(e => e.OrgUnit)
                      .WithMany(o => o.Consumers)
                      .HasForeignKey(e => e.OrgUnitId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Consumer_OrgUnit");

                entity.HasOne(e => e.Tariff)
                      .WithMany(t => t.Consumers)
                      .HasForeignKey(e => e.TariffId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Consumer_Tariff");
            });

            // ------------------ User ------------------
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.HasOne(e => e.Consumer)
                      .WithMany(c => c.Users)
                      .HasForeignKey(e => e.ConsumerId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_User_Consumer");

                entity.HasMany(e => e.Roles)
    .WithMany(r => r.Users)
    .UsingEntity<Dictionary<string, object>>(
        "UserRole",
        r => r.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
        u => u.HasOne<User>().WithMany().HasForeignKey("UserId"),
        j =>
        {
            j.HasKey("UserId", "RoleId");
            j.ToTable("UserRole");
        }
    );

            });

            // ------------------ RefreshToken ------------------
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.RefreshTokenId);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_RefreshToken_User");
            });

            // ------------------ Tariff ------------------
            modelBuilder.Entity<Tariff>(entity =>
            {
                entity.HasKey(e => e.TariffId);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            });

            // ------------------ TariffSlab ------------------
            modelBuilder.Entity<TariffSlab>(entity =>
            {
                entity.HasKey(e => e.TariffSlabId);
                entity.Property(e => e.Sequence).HasDefaultValue(1);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.HasOne(e => e.Tariff)
                      .WithMany(t => t.TariffSlabs)
                      .HasForeignKey(e => e.TariffId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_TariffSlab_Tariff");
            });

            // ------------------ Bill ------------------
            modelBuilder.Entity<Bill>(entity =>
            {
                entity.HasKey(e => e.BillId);
                entity.Property(e => e.BillGeneratedAt).HasDefaultValueSql("(sysutcdatetime())");

                entity.HasOne(e => e.Consumer)
                      .WithMany(c => c.Bills)
                      .HasForeignKey(e => e.ConsumerId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Bill_Consumer");

                entity.HasOne(e => e.MeterSerialNoNavigation)
                      .WithMany(m => m.Bills)
                      .HasForeignKey(e => e.MeterSerialNo)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_Bill_Meter");

                entity.HasOne(e => e.Tariff)
                      .WithMany(t => t.Bills)
                      .HasForeignKey(e => e.TariffId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Bill_Tariff");
            });

            // ------------------ BillDetail ------------------
            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.HasKey(e => e.BillDetailId);

                entity.HasOne(e => e.Bill)
                      .WithMany(b => b.BillDetails)
                      .HasForeignKey(e => e.BillId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_BillDetail_Bill");

                entity.HasOne(e => e.TariffSlab)
                      .WithMany(s => s.BillDetails)
                      .HasForeignKey(e => e.TariffSlabId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_BillDetail_TariffSlab");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
