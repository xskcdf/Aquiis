using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Components.Account;
using Aquiis.SimpleStart.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Aquiis.SimpleStart.Infrastructure.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Lease> Leases { get; set; }
        public DbSet<LeaseOffer> LeaseOffers { get; set; }
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
        public DbSet<SecurityDeposit> SecurityDeposits { get; set; }
        public DbSet<SecurityDepositInvestmentPool> SecurityDepositInvestmentPools { get; set; }
        public DbSet<SecurityDepositDividend> SecurityDepositDividends { get; set; }

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
                entity.Property(e => e.DefaultApplicationFee).HasPrecision(18, 2);
                entity.Property(e => e.OrganizationSharePercentage).HasPrecision(18, 6);
                entity.Property(e => e.SecurityDepositMultiplier).HasPrecision(18, 2);
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

            // Configure SecurityDeposit entity
            modelBuilder.Entity<SecurityDeposit>(entity =>
            {
                entity.HasOne(sd => sd.Lease)
                    .WithMany()
                    .HasForeignKey(sd => sd.LeaseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sd => sd.Tenant)
                    .WithMany()
                    .HasForeignKey(sd => sd.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.RefundAmount).HasPrecision(18, 2);
                entity.Property(e => e.DeductionsAmount).HasPrecision(18, 2);

                entity.HasIndex(e => e.LeaseId).IsUnique();
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.InInvestmentPool);
            });

            // Configure SecurityDepositInvestmentPool entity
            modelBuilder.Entity<SecurityDepositInvestmentPool>(entity =>
            {
                entity.Property(e => e.StartingBalance).HasPrecision(18, 2);
                entity.Property(e => e.EndingBalance).HasPrecision(18, 2);
                entity.Property(e => e.TotalEarnings).HasPrecision(18, 2);
                entity.Property(e => e.ReturnRate).HasPrecision(18, 6);
                entity.Property(e => e.OrganizationSharePercentage).HasPrecision(18, 6);
                entity.Property(e => e.OrganizationShare).HasPrecision(18, 2);
                entity.Property(e => e.TenantShareTotal).HasPrecision(18, 2);
                entity.Property(e => e.DividendPerLease).HasPrecision(18, 2);

                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.Year).IsUnique();
                entity.HasIndex(e => e.Status);
            });

            // Configure SecurityDepositDividend entity
            modelBuilder.Entity<SecurityDepositDividend>(entity =>
            {
                entity.HasOne(sdd => sdd.SecurityDeposit)
                    .WithMany(sd => sd.Dividends)
                    .HasForeignKey(sdd => sdd.SecurityDepositId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sdd => sdd.InvestmentPool)
                    .WithMany(ip => ip.Dividends)
                    .HasForeignKey(sdd => sdd.InvestmentPoolId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sdd => sdd.Lease)
                    .WithMany()
                    .HasForeignKey(sdd => sdd.LeaseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sdd => sdd.Tenant)
                    .WithMany()
                    .HasForeignKey(sdd => sdd.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.BaseDividendAmount).HasPrecision(18, 2);
                entity.Property(e => e.ProrationFactor).HasPrecision(18, 6);
                entity.Property(e => e.DividendAmount).HasPrecision(18, 2);

                entity.HasIndex(e => e.SecurityDepositId);
                entity.HasIndex(e => e.InvestmentPoolId);
                entity.HasIndex(e => e.LeaseId);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Year);
                entity.HasIndex(e => e.Status);
            });

            // Seed System Checklist Templates
            SeedChecklistTemplates(modelBuilder);
        }

        private void SeedChecklistTemplates(ModelBuilder modelBuilder)
        {
            var systemTimestamp = DateTime.Parse("2025-11-30T00:00:00Z").ToUniversalTime();
            
            // Seed ChecklistTemplates
            modelBuilder.Entity<ChecklistTemplate>().HasData(
                new ChecklistTemplate
                {
                    Id = 1,
                    Name = "Property Tour",
                    Description = "Standard property showing checklist",
                    Category = "Tour",
                    IsSystemTemplate = true,
                    OrganizationId = "SYSTEM",
                    CreatedOn = systemTimestamp,
                    CreatedBy = "SYSTEM",
                    IsDeleted = false
                },
                new ChecklistTemplate
                {
                    Id = 2,
                    Name = "Move-In",
                    Description = "Move-in inspection checklist",
                    Category = "MoveIn",
                    IsSystemTemplate = true,
                    OrganizationId = "SYSTEM",
                    CreatedOn = systemTimestamp,
                    CreatedBy = "SYSTEM",
                    IsDeleted = false
                },
                new ChecklistTemplate
                {
                    Id = 3,
                    Name = "Move-Out",
                    Description = "Move-out inspection checklist",
                    Category = "MoveOut",
                    IsSystemTemplate = true,
                    OrganizationId = "SYSTEM",
                    CreatedOn = systemTimestamp,
                    CreatedBy = "SYSTEM",
                    IsDeleted = false
                },
                new ChecklistTemplate
                {
                    Id = 4,
                    Name = "Open House",
                    Description = "Open house event checklist",
                    Category = "Tour",
                    IsSystemTemplate = true,
                    OrganizationId = "SYSTEM",
                    CreatedOn = systemTimestamp,
                    CreatedBy = "SYSTEM",
                    IsDeleted = false
                }
            );

            // Seed Property Tour Checklist Items
            modelBuilder.Entity<ChecklistTemplateItem>().HasData(
                // Arrival & Introduction (Section 1)
                new ChecklistTemplateItem { Id = 1, ChecklistTemplateId = 1, ItemText = "Greeted prospect and verified appointment", ItemOrder = 1, CategorySection = "Arrival & Introduction", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 2, ChecklistTemplateId = 1, ItemText = "Reviewed property exterior and curb appeal", ItemOrder = 2, CategorySection = "Arrival & Introduction", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 3, ChecklistTemplateId = 1, ItemText = "Showed parking area/garage", ItemOrder = 3, CategorySection = "Arrival & Introduction", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },

                // Interior Tour (Section 2)
                new ChecklistTemplateItem { Id = 4, ChecklistTemplateId = 1, ItemText = "Toured living room/common areas", ItemOrder = 4, CategorySection = "Interior Tour", SectionOrder = 2, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 5, ChecklistTemplateId = 1, ItemText = "Showed all bedrooms", ItemOrder = 5, CategorySection = "Interior Tour", SectionOrder = 2, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 6, ChecklistTemplateId = 1, ItemText = "Showed all bathrooms", ItemOrder = 6, CategorySection = "Interior Tour", SectionOrder = 2, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },

                // Kitchen & Appliances (Section 3)
                new ChecklistTemplateItem { Id = 7, ChecklistTemplateId = 1, ItemText = "Toured kitchen and demonstrated appliances", ItemOrder = 7, CategorySection = "Kitchen & Appliances", SectionOrder = 3, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 8, ChecklistTemplateId = 1, ItemText = "Explained which appliances are included", ItemOrder = 8, CategorySection = "Kitchen & Appliances", SectionOrder = 3, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },

                // Utilities & Systems (Section 4)
                new ChecklistTemplateItem { Id = 9, ChecklistTemplateId = 1, ItemText = "Explained HVAC system and thermostat controls", ItemOrder = 9, CategorySection = "Utilities & Systems", SectionOrder = 4, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 10, ChecklistTemplateId = 1, ItemText = "Reviewed utility responsibilities (tenant vs landlord)", ItemOrder = 10, CategorySection = "Utilities & Systems", SectionOrder = 4, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 11, ChecklistTemplateId = 1, ItemText = "Showed water heater location", ItemOrder = 11, CategorySection = "Utilities & Systems", SectionOrder = 4, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },

                // Storage & Amenities (Section 5)
                new ChecklistTemplateItem { Id = 12, ChecklistTemplateId = 1, ItemText = "Showed storage areas (closets, attic, basement)", ItemOrder = 12, CategorySection = "Storage & Amenities", SectionOrder = 5, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 13, ChecklistTemplateId = 1, ItemText = "Showed laundry facilities", ItemOrder = 13, CategorySection = "Storage & Amenities", SectionOrder = 5, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 14, ChecklistTemplateId = 1, ItemText = "Showed outdoor space (yard, patio, balcony)", ItemOrder = 14, CategorySection = "Storage & Amenities", SectionOrder = 5, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },

                // Lease Terms (Section 6)
                new ChecklistTemplateItem { Id = 15, ChecklistTemplateId = 1, ItemText = "Discussed monthly rent amount", ItemOrder = 15, CategorySection = "Lease Terms", SectionOrder = 6, IsRequired = true, RequiresValue = true, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 16, ChecklistTemplateId = 1, ItemText = "Explained security deposit and move-in costs", ItemOrder = 16, CategorySection = "Lease Terms", SectionOrder = 6, IsRequired = true, RequiresValue = true, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 17, ChecklistTemplateId = 1, ItemText = "Reviewed lease term length and start date", ItemOrder = 17, CategorySection = "Lease Terms", SectionOrder = 6, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 18, ChecklistTemplateId = 1, ItemText = "Explained pet policy", ItemOrder = 18, CategorySection = "Lease Terms", SectionOrder = 6, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },

                // Next Steps (Section 7)
                new ChecklistTemplateItem { Id = 19, ChecklistTemplateId = 1, ItemText = "Explained application process and requirements", ItemOrder = 19, CategorySection = "Next Steps", SectionOrder = 7, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 20, ChecklistTemplateId = 1, ItemText = "Reviewed screening process (background, credit check)", ItemOrder = 20, CategorySection = "Next Steps", SectionOrder = 7, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 21, ChecklistTemplateId = 1, ItemText = "Answered all prospect questions", ItemOrder = 21, CategorySection = "Next Steps", SectionOrder = 7, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },

                // Assessment (Section 8)
                new ChecklistTemplateItem { Id = 22, ChecklistTemplateId = 1, ItemText = "Prospect Interest Level", ItemOrder = 22, CategorySection = "Assessment", SectionOrder = 8, IsRequired = true, RequiresValue = true, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 23, ChecklistTemplateId = 1, ItemText = "Overall showing feedback and notes", ItemOrder = 23, CategorySection = "Assessment", SectionOrder = 8, IsRequired = true, RequiresValue = true, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },

                // Move-In Checklist Items (Placeholders)
                new ChecklistTemplateItem { Id = 24, ChecklistTemplateId = 2, ItemText = "Document property condition", ItemOrder = 1, CategorySection = "General", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 25, ChecklistTemplateId = 2, ItemText = "Collect keys and access codes", ItemOrder = 2, CategorySection = "General", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 26, ChecklistTemplateId = 2, ItemText = "Review lease terms with tenant", ItemOrder = 3, CategorySection = "General", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },

                // Move-Out Checklist Items (Placeholders)
                new ChecklistTemplateItem { Id = 27, ChecklistTemplateId = 3, ItemText = "Inspect property condition", ItemOrder = 1, CategorySection = "General", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 28, ChecklistTemplateId = 3, ItemText = "Collect all keys and access devices", ItemOrder = 2, CategorySection = "General", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 29, ChecklistTemplateId = 3, ItemText = "Document damages and needed repairs", ItemOrder = 3, CategorySection = "General", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },

                // Open House Checklist Items (Placeholders)
                new ChecklistTemplateItem { Id = 30, ChecklistTemplateId = 4, ItemText = "Set up signage and directional markers", ItemOrder = 1, CategorySection = "Preparation", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 31, ChecklistTemplateId = 4, ItemText = "Prepare information packets", ItemOrder = 2, CategorySection = "Preparation", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false },
                new ChecklistTemplateItem { Id = 32, ChecklistTemplateId = 4, ItemText = "Set up visitor sign-in sheet", ItemOrder = 3, CategorySection = "Preparation", SectionOrder = 1, IsRequired = true, RequiresValue = false, AllowsNotes = true, OrganizationId = "SYSTEM", CreatedOn = systemTimestamp, CreatedBy = "SYSTEM", IsDeleted = false }
            );
        }

    }
}