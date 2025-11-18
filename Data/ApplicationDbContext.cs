using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Components.PropertyManagement.Properties;
using Aquiis.SimpleStart.Components.PropertyManagement.Leases;
using Aquiis.SimpleStart.Components.PropertyManagement.Tenants;
using Aquiis.SimpleStart.Components.PropertyManagement.Invoices;
using Aquiis.SimpleStart.Components.PropertyManagement.Payments;
using Aquiis.SimpleStart.Components.PropertyManagement.Documents;
using Aquiis.SimpleStart.Components.PropertyManagement.Inspections;
using Aquiis.SimpleStart.Components.PropertyManagement.MaintenanceRequests;
using Aquiis.SimpleStart.Components.Account;
using Aquiis.SimpleStart.Models;
using Microsoft.AspNetCore.Identity;

namespace Aquiis.SimpleStart.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Lease> Leases { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<OrganizationSettings> OrganizationSettings { get; set; }

         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Property entity
            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasIndex(e => e.Address);
                entity.Property(e => e.MonthlyRent).HasPrecision(18, 2);
                
                // Configure relationship with User
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure Tenant entity
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.IdentificationNumber).IsUnique();
                
                // Configure relationship with User
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Configure Lease entity
            modelBuilder.Entity<Lease>(entity =>
            {
                entity.HasOne(l => l.Property)
                    .WithMany(p => p.Leases)
                    .HasForeignKey(l => l.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Tenant)
                    .WithMany(t => t.Leases)
                    .HasForeignKey(l => l.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.MonthlyRent).HasPrecision(18, 2);
                entity.Property(e => e.SecurityDeposit).HasPrecision(18, 2);
                
                // Configure relationship with User
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Invoice entity
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasOne(i => i.Lease)
                    .WithMany(l => l.Invoices)
                    .HasForeignKey(i => i.LeaseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.AmountPaid).HasPrecision(18, 2);
                
                // Configure relationship with User
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Invoice)
                    .WithMany(i => i.Payments)
                    .HasForeignKey(p => p.InvoiceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Amount).HasPrecision(18, 2);
                
                // Configure relationship with User
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Document entity
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasOne(d => d.Property)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Tenant)
                    .WithMany()
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Lease)
                    .WithMany(l => l.Documents)
                    .HasForeignKey(d => d.LeaseId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Invoice)
                    .WithMany()
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Payment)
                    .WithMany()
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                // FileData is automatically stored as BLOB in SQLite
                // No need to specify column type
                
                // Configure relationship with User
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Inspection entity
            modelBuilder.Entity<Inspection>(entity =>
            {
                entity.HasOne(i => i.Property)
                    .WithMany()
                    .HasForeignKey(i => i.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Lease)
                    .WithMany()
                    .HasForeignKey(i => i.LeaseId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.InspectionDate);
            });

            // Configure MaintenanceRequest entity
            modelBuilder.Entity<MaintenanceRequest>(entity =>
            {
                entity.HasOne(m => m.Property)
                    .WithMany()
                    .HasForeignKey(m => m.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Lease)
                    .WithMany()
                    .HasForeignKey(m => m.LeaseId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(e => e.EstimatedCost).HasPrecision(18, 2);
                entity.Property(e => e.ActualCost).HasPrecision(18, 2);
                
                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.RequestedOn);
            });

            // Configure OrganizationSettings entity
            modelBuilder.Entity<OrganizationSettings>(entity =>
            {
                entity.Property(e => e.OrganizationId).HasConversion<string>();
                entity.HasIndex(e => e.OrganizationId).IsUnique();
                entity.Property(e => e.LateFeePercentage).HasPrecision(5, 4);
                entity.Property(e => e.MaxLateFeeAmount).HasPrecision(18, 2);
            });
        }

    }
}