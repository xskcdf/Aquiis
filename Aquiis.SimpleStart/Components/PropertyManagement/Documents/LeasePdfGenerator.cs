using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Aquiis.SimpleStart.Components.PropertyManagement.Leases;

namespace Aquiis.SimpleStart.Components.PropertyManagement.Documents
{
    public class LeasePdfGenerator
    {
        public static byte[] GenerateLeasePdf(Lease lease)
        {
            // Configure QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(content => ComposeContent(content, lease));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("RESIDENTIAL LEASE AGREEMENT").FontSize(18).Bold();
                    column.Item().PaddingTop(5).Text($"Generated: {DateTime.Now:MMMM dd, yyyy}").FontSize(9);
                });
            });
        }

        private static void ComposeContent(IContainer container, Lease lease)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(15);

                // Property Information Section
                column.Item().Element(c => ComposeSectionHeader(c, "PROPERTY INFORMATION"));
                column.Item().Element(c => ComposePropertyInfo(c, lease));

                // Tenant Information Section
                column.Item().Element(c => ComposeSectionHeader(c, "TENANT INFORMATION"));
                column.Item().Element(c => ComposeTenantInfo(c, lease));

                // Lease Terms Section
                column.Item().Element(c => ComposeSectionHeader(c, "LEASE TERMS"));
                column.Item().Element(c => ComposeLeaseTerms(c, lease));

                // Financial Information Section
                column.Item().Element(c => ComposeSectionHeader(c, "FINANCIAL TERMS"));
                column.Item().Element(c => ComposeFinancialInfo(c, lease));

                // Additional Terms Section
                if (!string.IsNullOrWhiteSpace(lease.Terms))
                {
                    column.Item().Element(c => ComposeSectionHeader(c, "ADDITIONAL TERMS AND CONDITIONS"));
                    column.Item().Element(c => ComposeAdditionalTerms(c, lease));
                }

                // Signatures Section
                column.Item().PaddingTop(30).Element(ComposeSignatures);
            });
        }

        private static void ComposeSectionHeader(IContainer container, string title)
        {
            container.Background(Colors.Grey.Lighten3).Padding(8).Text(title).FontSize(12).Bold();
        }

        private static void ComposePropertyInfo(IContainer container, Lease lease)
        {
            container.Padding(10).Column(column =>
            {
                column.Spacing(5);
                
                if (lease.Property != null)
                {
                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Address:").Bold();
                        row.RelativeItem().Text(lease.Property.Address ?? "N/A");
                    });

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("City, State:").Bold();
                        row.RelativeItem().Text($"{lease.Property.City}, {lease.Property.State} {lease.Property.ZipCode}");
                    });

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Property Type:").Bold();
                        row.RelativeItem().Text(lease.Property.PropertyType ?? "N/A");
                    });

                    if (lease.Property.Bedrooms > 0 || lease.Property.Bathrooms > 0)
                    {
                        column.Item().Row(row =>
                        {
                            row.ConstantItem(120).Text("Bedrooms/Baths:").Bold();
                            row.RelativeItem().Text($"{lease.Property.Bedrooms} bed / {lease.Property.Bathrooms} bath");
                        });
                    }
                }
            });
        }

        private static void ComposeTenantInfo(IContainer container, Lease lease)
        {
            container.Padding(10).Column(column =>
            {
                column.Spacing(5);
                
                if (lease.Tenant != null)
                {
                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Name:").Bold();
                        row.RelativeItem().Text(lease.Tenant.FullName ?? "N/A");
                    });

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Email:").Bold();
                        row.RelativeItem().Text(lease.Tenant.Email ?? "N/A");
                    });

                    column.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Phone:").Bold();
                        row.RelativeItem().Text(lease.Tenant.PhoneNumber ?? "N/A");
                    });
                }
            });
        }

        private static void ComposeLeaseTerms(IContainer container, Lease lease)
        {
            container.Padding(10).Column(column =>
            {
                column.Spacing(5);

                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Lease Start Date:").Bold();
                    row.RelativeItem().Text(lease.StartDate.ToString("MMMM dd, yyyy"));
                });

                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Lease End Date:").Bold();
                    row.RelativeItem().Text(lease.EndDate.ToString("MMMM dd, yyyy"));
                });

                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Lease Duration:").Bold();
                    row.RelativeItem().Text($"{(lease.EndDate - lease.StartDate).Days} days");
                });

                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Lease Status:").Bold();
                    row.RelativeItem().Text(lease.Status ?? "N/A");
                });
            });
        }

        private static void ComposeFinancialInfo(IContainer container, Lease lease)
        {
            container.Padding(10).Column(column =>
            {
                column.Spacing(5);

                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Monthly Rent:").Bold();
                    row.RelativeItem().Text(lease.MonthlyRent.ToString("C"));
                });

                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Security Deposit:").Bold();
                    row.RelativeItem().Text(lease.SecurityDeposit.ToString("C"));
                });

                var totalRent = lease.MonthlyRent * ((lease.EndDate - lease.StartDate).Days / 30.0m);
                column.Item().Row(row =>
                {
                    row.ConstantItem(120).Text("Total Rent:").Bold();
                    row.RelativeItem().Text($"{totalRent:C} (approximate)");
                });
            });
        }

        private static void ComposeAdditionalTerms(IContainer container, Lease lease)
        {
            container.Padding(10).Text(lease.Terms).FontSize(10);
        }

        private static void ComposeSignatures(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(30);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(2).Text("");
                        col.Item().PaddingTop(5).Text("Landlord Signature").FontSize(9);
                    });

                    row.ConstantItem(50);

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(2).Text("");
                        col.Item().PaddingTop(5).Text("Date").FontSize(9);
                    });
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(2).Text("");
                        col.Item().PaddingTop(5).Text("Tenant Signature").FontSize(9);
                    });

                    row.ConstantItem(50);

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(2).Text("");
                        col.Item().PaddingTop(5).Text("Date").FontSize(9);
                    });
                });
            });
        }
    }
}
