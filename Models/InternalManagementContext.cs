using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace BachBinHoangManagement.Models;

public partial class InternalManagementContext : DbContext
{
    public InternalManagementContext()
    {
    }

    public InternalManagementContext(DbContextOptions<InternalManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<ProvidedCompany> ProvidedCompanies { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<TransportCompany> TransportCompanies { get; set; }

    public virtual DbSet<TransportCompanyService> TransportCompanyServices { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=internal_management;uid=root;pwd=bach", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.31-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("district");

            entity.HasIndex(e => e.ProvinceId, "province_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");

            entity.HasOne(d => d.Province).WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("district_ibfk_1");
        });

        modelBuilder.Entity<ProvidedCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("provided_company");

            entity.HasIndex(e => e.DistrictId, "district_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bill)
                .HasMaxLength(255)
                .HasColumnName("bill");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("contact");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(255)
                .HasColumnName("job_title");
            entity.Property(e => e.KilnCharacteristics)
                .HasMaxLength(255)
                .HasColumnName("kiln_characteristics");
            entity.Property(e => e.MaximumCapacity)
                .HasMaxLength(255)
                .HasColumnName("maximum_capacity");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");
            entity.Property(e => e.Product)
                .HasMaxLength(255)
                .HasColumnName("product");
            entity.Property(e => e.ProductQuote)
                .HasMaxLength(255)
                .HasColumnName("product_quote");
            entity.Property(e => e.SpecificAddress)
                .HasMaxLength(255)
                .HasColumnName("specific_address");

            entity.HasOne(d => d.District).WithMany(p => p.ProvidedCompanies)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("provided_company_ibfk_1");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("province");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("service");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TransportCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transport_company");

            entity.HasIndex(e => e.DistrictId, "district_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .HasColumnName("contact");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.SpecificAddress)
                .HasMaxLength(255)
                .HasColumnName("specific_address");

            entity.HasOne(d => d.District).WithMany(p => p.TransportCompanies)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transport_company_ibfk_1");
        });

        modelBuilder.Entity<TransportCompanyService>(entity =>
        {
            entity.HasKey(e => new { e.TransportCompanyId, e.ServiceId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("transport_company_service");

            entity.HasIndex(e => e.ServiceId, "service_id");

            entity.Property(e => e.TransportCompanyId).HasColumnName("transport_company_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");

            entity.HasOne(d => d.Service).WithMany(p => p.TransportCompanyServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transport_company_service_ibfk_2");

            entity.HasOne(d => d.TransportCompany).WithMany(p => p.TransportCompanyServices)
                .HasForeignKey(d => d.TransportCompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transport_company_service_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("last_login");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.ResetToken)
                .HasMaxLength(255)
                .HasColumnName("reset_token");
            entity.Property(e => e.ResetTokenExpired)
                .HasColumnType("datetime")
                .HasColumnName("reset_token_expired");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
            entity.Property(e => e.VerifyToken)
                .HasMaxLength(255)
                .HasColumnName("verify_token");
            entity.Property(e => e.VerifyTokenExpired)
                .HasColumnType("datetime")
                .HasColumnName("verify_token_expired");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
