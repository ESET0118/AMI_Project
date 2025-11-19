using System;
using System.Collections.Generic;
using AMI_Project.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Data;

public partial class AMIDbContext : DbContext
{
    public AMIDbContext()
    {
    }

    public AMIDbContext(DbContextOptions<AMIDbContext> options)
        : base(options)
    {
    }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AMI_DB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__Bill__11F2FC6A1342EC1E");

            entity.ToTable("Bill");

            entity.Property(e => e.BillGeneratedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MeterSerialNo).HasMaxLength(50);
            entity.Property(e => e.OutstandingDue).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.PaidOn).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalPayable).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.UnitsConsumed).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.Consumer).WithMany(p => p.Bills)
                .HasForeignKey(d => d.ConsumerId)
                .HasConstraintName("FK_Bill_Consumer");

            entity.HasOne(d => d.MeterSerialNoNavigation).WithMany(p => p.Bills)
                .HasForeignKey(d => d.MeterSerialNo)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Bill_Meter");

            entity.HasOne(d => d.Tariff).WithMany(p => p.Bills)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bill_Tariff");
        });

        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasKey(e => e.BillDetailId).HasName("PK__BillDeta__793CAF95EB750C9C");

            entity.ToTable("BillDetail");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Rate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Units).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.Bill).WithMany(p => p.BillDetails)
                .HasForeignKey(d => d.BillId)
                .HasConstraintName("FK_BillDetail_Bill");

            entity.HasOne(d => d.TariffSlab).WithMany(p => p.BillDetails)
                .HasForeignKey(d => d.TariffSlabId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BillDetail_TariffSlab");
        });

        modelBuilder.Entity<Consumer>(entity =>
        {
            entity.HasKey(e => e.ConsumerId).HasName("PK__Consumer__63BBE9BA9CCC8B42");

            entity.ToTable("Consumer");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValue("system");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Lat).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Lon).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");
            entity.Property(e => e.UpdatedAt).HasPrecision(3);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);

            entity.HasOne(d => d.OrgUnit).WithMany(p => p.Consumers)
                .HasForeignKey(d => d.OrgUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumer_OrgUnit");

            entity.HasOne(d => d.Tariff).WithMany(p => p.Consumers)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumer_Tariff");
        });

        modelBuilder.Entity<Meter>(entity =>
        {
            entity.HasKey(e => e.MeterSerialNo).HasName("PK__Meter__5C498B0F56EE4CAA");

            entity.ToTable("Meter");

            entity.HasIndex(e => e.IpAddress, "UQ_Meter_IpAddress").IsUnique();

            entity.Property(e => e.MeterSerialNo).HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Firmware).HasMaxLength(50);
            entity.Property(e => e.ICCID).HasMaxLength(30);
            entity.Property(e => e.IMSI).HasMaxLength(30);
            entity.Property(e => e.InstallTsUtc).HasPrecision(3);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Consumer).WithMany(p => p.Meters)
                .HasForeignKey(d => d.ConsumerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Meter_Consumer");
        });

        modelBuilder.Entity<MeterReading>(entity =>
        {
            entity.HasKey(e => e.MeterReadingId).HasName("PK__MeterRea__AFB4FD99BCCEF6CF");

            entity.ToTable("MeterReading");

            entity.Property(e => e.Ampere).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ConsumptionKwh).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Frequency).HasColumnType("decimal(10, 4)");
            entity.Property(e => e.MeterSerialNo).HasMaxLength(50);
            entity.Property(e => e.PowerFactor).HasColumnType("decimal(10, 4)");
            entity.Property(e => e.ReadingDateTime).HasPrecision(3);
            entity.Property(e => e.Voltage).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.MeterSerialNoNavigation).WithMany(p => p.MeterReadings)
                .HasForeignKey(d => d.MeterSerialNo)
                .HasConstraintName("FK_MeterReading_Meter");
        });

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

        modelBuilder.Entity<OrgUnit>(entity =>
        {
            entity.HasKey(e => e.OrgUnitId).HasName("PK__OrgUnit__4A793BEE6B3A30C1");

            entity.ToTable("OrgUnit");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_OrgUnit_Parent");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("PK__RefreshT__F5845E391434CDFC");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CreatedByIp).HasMaxLength(100);
            entity.Property(e => e.ExpiresAt).HasPrecision(3);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            entity.Property(e => e.RevokedAt).HasPrecision(3);
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_RefreshToken_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A6E0E944A");

            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "UQ__Role__737584F64341A942").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Tariff>(entity =>
        {
            entity.HasKey(e => e.TariffId).HasName("PK__Tariff__EBAF9DB301C4DDD3");

            entity.ToTable("Tariff");

            entity.Property(e => e.BaseRate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TaxRate).HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<TariffSlab>(entity =>
        {
            entity.HasKey(e => e.TariffSlabId).HasName("PK__TariffSl__64EAAA2237287310");

            entity.ToTable("TariffSlab");

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FromKwh).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.RatePerKwh).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Sequence).HasDefaultValue(1);
            entity.Property(e => e.ToKwh).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.Tariff).WithMany(p => p.TariffSlabs)
                .HasForeignKey(d => d.TariffId)
                .HasConstraintName("FK_TariffSlab_Tariff");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C8B1E3306");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D105343395D1CF").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.EmailConfirmed).HasDefaultValue(false);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_UserRole_Role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRole_User"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRole");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
