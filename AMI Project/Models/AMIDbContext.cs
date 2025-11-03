using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AMI_Project.Models;

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

    public virtual DbSet<OrgUnit> OrgUnits { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tariff> Tariffs { get; set; }

    public virtual DbSet<TariffSlab> TariffSlabs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AMI_DB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__Bill__11F2FC6A1342EC1E");

            entity.Property(e => e.BillGeneratedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Consumer).WithMany(p => p.Bills).HasConstraintName("FK_Bill_Consumer");

            entity.HasOne(d => d.MeterSerialNoNavigation).WithMany(p => p.Bills)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Bill_Meter");

            entity.HasOne(d => d.Tariff).WithMany(p => p.Bills)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bill_Tariff");
        });

        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasKey(e => e.BillDetailId).HasName("PK__BillDeta__793CAF95EB750C9C");

            entity.HasOne(d => d.Bill).WithMany(p => p.BillDetails).HasConstraintName("FK_BillDetail_Bill");

            entity.HasOne(d => d.TariffSlab).WithMany(p => p.BillDetails)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BillDetail_TariffSlab");
        });

        modelBuilder.Entity<Consumer>(entity =>
        {
            entity.HasKey(e => e.ConsumerId).HasName("PK__Consumer__63BBE9BA9CCC8B42");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CreatedBy).HasDefaultValue("system");
            entity.Property(e => e.Status).HasDefaultValue("Active");

            entity.HasOne(d => d.OrgUnit).WithMany(p => p.Consumers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumer_OrgUnit");

            entity.HasOne(d => d.Tariff).WithMany(p => p.Consumers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consumer_Tariff");
        });

        modelBuilder.Entity<Meter>(entity =>
        {
            entity.HasKey(e => e.MeterSerialNo).HasName("PK__Meter__5C498B0F56EE4CAA");

            entity.Property(e => e.Status).HasDefaultValue("Active");

            entity.HasOne(d => d.Consumer).WithMany(p => p.Meters)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Meter_Consumer");
        });

        modelBuilder.Entity<MeterReading>(entity =>
        {
            entity.HasKey(e => e.MeterReadingId).HasName("PK__MeterRea__AFB4FD99BCCEF6CF");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.MeterSerialNoNavigation).WithMany(p => p.MeterReadings).HasConstraintName("FK_MeterReading_Meter");
        });

        modelBuilder.Entity<OrgUnit>(entity =>
        {
            entity.HasKey(e => e.OrgUnitId).HasName("PK__OrgUnit__4A793BEE6B3A30C1");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_OrgUnit_Parent");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("PK__RefreshT__F5845E391434CDFC");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens).HasConstraintName("FK_RefreshToken_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A45305CDF");
        });

        modelBuilder.Entity<Tariff>(entity =>
        {
            entity.HasKey(e => e.TariffId).HasName("PK__Tariff__EBAF9DB301C4DDD3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<TariffSlab>(entity =>
        {
            entity.HasKey(e => e.TariffSlabId).HasName("PK__TariffSl__64EAAA2237287310");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Sequence).HasDefaultValue(1);

            entity.HasOne(d => d.Tariff).WithMany(p => p.TariffSlabs).HasConstraintName("FK_TariffSlab_Tariff");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C65495C4A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Consumer).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_User_Consumer");

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
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF2760ADE312416F");
                        j.ToTable("UserRole");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
