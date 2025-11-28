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
        public DbSet<SchemaVersion> SchemaVersions { get; set; }
        public DbSet<ChecklistTemplate> ChecklistTemplates { get; set; }
        public DbSet<ChecklistTemplateItem> ChecklistTemplateItems { get; set; }
        public DbSet<Checklist> Checklists { get; set; }
        public DbSet<ChecklistItem> ChecklistItems { get; set; }
        public DbSet<ProspectiveTenant> ProspectiveTenants { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<RentalApplication> RentalApplications { get; set; }
        public DbSet<ApplicationScreening> ApplicationScreenings { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<CalendarSettings> CalendarSettings { get; set; }
        public DbSet<Note> Notes { get; set; }

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

                entity.HasOne(l => l.Document)
                    .WithMany()
                    .HasForeignKey(l => l.DocumentId)
                    .OnDelete(DeleteBehavior.SetNull);

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

                entity.HasOne(i => i.Document)
                    .WithMany()
                    .HasForeignKey(i => i.DocumentId)
                    .OnDelete(DeleteBehavior.SetNull);

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

                entity.HasOne(p => p.Document)
                    .WithMany()
                    .HasForeignKey(p => p.DocumentId)
                    .OnDelete(DeleteBehavior.SetNull);

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

                entity.HasOne(i => i.Document)
                    .WithMany()
                    .HasForeignKey(i => i.DocumentId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.CompletedOn);
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

            // Configure ChecklistTemplate entity
            modelBuilder.Entity<ChecklistTemplate>(entity =>
            {
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.Category);
            });

            // Configure ChecklistTemplateItem entity
            modelBuilder.Entity<ChecklistTemplateItem>(entity =>
            {
                entity.HasOne(cti => cti.ChecklistTemplate)
                    .WithMany(ct => ct.Items)
                    .HasForeignKey(cti => cti.ChecklistTemplateId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ChecklistTemplateId);
            });

            // Configure Checklist entity
            modelBuilder.Entity<Checklist>(entity =>
            {
                entity.HasOne(c => c.Property)
                    .WithMany()
                    .HasForeignKey(c => c.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Lease)
                    .WithMany()
                    .HasForeignKey(c => c.LeaseId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(c => c.ChecklistTemplate)
                    .WithMany(ct => ct.Checklists)
                    .HasForeignKey(c => c.ChecklistTemplateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Document)
                    .WithMany()
                    .HasForeignKey(c => c.DocumentId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.PropertyId);
                entity.HasIndex(e => e.LeaseId);
                entity.HasIndex(e => e.ChecklistType);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CompletedOn);
            });

            // Configure ChecklistItem entity
            modelBuilder.Entity<ChecklistItem>(entity =>
            {
                entity.HasOne(ci => ci.Checklist)
                    .WithMany(c => c.Items)
                    .HasForeignKey(ci => ci.ChecklistId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ChecklistId);
            });

            // Configure ProspectiveTenant entity
            modelBuilder.Entity<ProspectiveTenant>(entity =>
            {
                entity.HasOne(pt => pt.InterestedProperty)
                    .WithMany()
                    .HasForeignKey(pt => pt.InterestedPropertyId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.Status);
            });

            // Configure Tour entity
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasOne(s => s.ProspectiveTenant)
                    .WithMany(pt => pt.Tours)
                    .HasForeignKey(s => s.ProspectiveTenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Property)
                    .WithMany()
                    .HasForeignKey(s => s.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.ScheduledOn);
                entity.HasIndex(e => e.Status);
            });

            // Configure RentalApplication entity
            modelBuilder.Entity<RentalApplication>(entity =>
            {
                entity.HasOne(ra => ra.ProspectiveTenant)
                    .WithOne(pt => pt.Application)
                    .HasForeignKey<RentalApplication>(ra => ra.ProspectiveTenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ra => ra.Property)
                    .WithMany()
                    .HasForeignKey(ra => ra.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.CurrentRent).HasPrecision(18, 2);
                entity.Property(e => e.MonthlyIncome).HasPrecision(18, 2);
                entity.Property(e => e.ApplicationFee).HasPrecision(18, 2);

                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.AppliedOn);
                entity.HasIndex(e => e.Status);
            });

            // Configure ApplicationScreening entity
            modelBuilder.Entity<ApplicationScreening>(entity =>
            {
                entity.HasOne(asc => asc.RentalApplication)
                    .WithOne(ra => ra.Screening)
                    .HasForeignKey<ApplicationScreening>(asc => asc.RentalApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.OverallResult);
            });

            // Configure CalendarEvent entity
            modelBuilder.Entity<CalendarEvent>(entity =>
            {
                entity.HasOne(ce => ce.Property)
                    .WithMany()
                    .HasForeignKey(ce => ce.PropertyId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.StartOn);
                entity.HasIndex(e => e.EventType);
                entity.HasIndex(e => e.SourceEntityId);
                entity.HasIndex(e => new { e.SourceEntityType, e.SourceEntityId });
            });

            // Configure CalendarSettings entity
            modelBuilder.Entity<CalendarSettings>(entity =>
            {
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => new { e.OrganizationId, e.EntityType }).IsUnique();
            });
        }

    }
}