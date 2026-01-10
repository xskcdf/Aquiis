using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Aquiis.Core.Entities;
using PdfDocument = QuestPDF.Fluent.Document;

namespace Aquiis.Application.Services.PdfGenerators
{
    public class LeaseRenewalPdfGenerator
    {
        public byte[] GenerateRenewalOfferLetter(Lease lease, Property property, Tenant tenant)
        {
             // Configure QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;
            
            var document = PdfDocument.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(50);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Height(100)
                        .Background(Colors.Grey.Lighten3)
                        .Padding(20)
                        .Column(column =>
                        {
                            column.Item().Text("LEASE RENEWAL OFFER")
                                .FontSize(20)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);
                            
                            column.Item().PaddingTop(5).Text(DateTime.Now.ToString("MMMM dd, yyyy"))
                                .FontSize(10)
                                .FontColor(Colors.Grey.Darken1);
                        });

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            // Tenant Information
                            column.Item().PaddingBottom(20).Column(c =>
                            {
                                c.Item().Text("Dear " + tenant.FullName + ",")
                                    .FontSize(12)
                                    .Bold();
                                
                                c.Item().PaddingTop(10).Text(text =>
                                {
                                    text.Line("RE: Lease Renewal Offer");
                                    text.Span("Property Address: ").Bold();
                                    text.Span(property.Address);
                                    text.Line("");
                                    text.Span(property.City + ", " + property.State + " " + property.ZipCode);
                                });
                            });

                            // Introduction
                            column.Item().PaddingBottom(15).Text(text =>
                            {
                                text.Span("We hope you have enjoyed living at ");
                                text.Span(property.Address).Bold();
                                text.Span(". As your current lease is approaching its expiration date on ");
                                text.Span(lease.EndDate.ToString("MMMM dd, yyyy")).Bold();
                                text.Span(", we would like to offer you the opportunity to renew your lease.");
                            });

                            // Current Lease Details
                            column.Item().PaddingBottom(20).Column(c =>
                            {
                                c.Item().Text("Current Lease Information:")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken2);
                                
                                c.Item().PaddingTop(10).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(3);
                                    });

                                    // Header
                                    table.Cell().Background(Colors.Grey.Lighten2)
                                        .Padding(8).Text("Detail").Bold();
                                    table.Cell().Background(Colors.Grey.Lighten2)
                                        .Padding(8).Text("Information").Bold();

                                    // Rows
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text("Lease Start Date");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text(lease.StartDate.ToString("MMMM dd, yyyy"));

                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text("Lease End Date");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text(lease.EndDate.ToString("MMMM dd, yyyy"));

                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text("Current Monthly Rent");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text(lease.MonthlyRent.ToString("C"));

                                    table.Cell().Padding(8).Text("Security Deposit");
                                    table.Cell().Padding(8).Text(lease.SecurityDeposit.ToString("C"));
                                });
                            });

                            // Renewal Offer Details
                            column.Item().PaddingBottom(20).Column(c =>
                            {
                                c.Item().Text("Renewal Offer Details:")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken2);
                                
                                c.Item().PaddingTop(10).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(3);
                                    });

                                    // Header
                                    table.Cell().Background(Colors.Grey.Lighten2)
                                        .Padding(8).Text("Detail").Bold();
                                    table.Cell().Background(Colors.Grey.Lighten2)
                                        .Padding(8).Text("Proposed Terms").Bold();

                                    // Rows
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text("New Lease Start Date");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text(lease.EndDate.AddDays(1).ToString("MMMM dd, yyyy"));

                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text("New Lease End Date");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text(lease.EndDate.AddYears(1).ToString("MMMM dd, yyyy"));

                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text("Proposed Monthly Rent");
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8).Text(text =>
                                        {
                                            text.Span((lease.ProposedRenewalRent ?? lease.MonthlyRent).ToString("C")).Bold();
                                            
                                            if (lease.ProposedRenewalRent.HasValue && lease.ProposedRenewalRent != lease.MonthlyRent)
                                            {
                                                var increase = lease.ProposedRenewalRent.Value - lease.MonthlyRent;
                                                var percentage = (increase / lease.MonthlyRent) * 100;
                                                text.Span(" (");
                                                text.Span(increase > 0 ? "+" : "");
                                                text.Span(increase.ToString("C") + ", ");
                                                text.Span(percentage.ToString("F1") + "%");
                                                text.Span(")").FontSize(9).Italic();
                                            }
                                        });

                                    table.Cell().Padding(8).Text("Lease Term");
                                    table.Cell().Padding(8).Text("12 months");
                                });
                            });

                            // Renewal Notes
                            if (!string.IsNullOrEmpty(lease.RenewalNotes))
                            {
                                column.Item().PaddingBottom(15).Column(c =>
                                {
                                    c.Item().Text("Additional Information:")
                                        .FontSize(12)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken2);
                                    
                                    c.Item().PaddingTop(8)
                                        .PaddingLeft(10)
                                        .Text(lease.RenewalNotes)
                                        .Italic();
                                });
                            }

                            // Response Instructions
                            column.Item().PaddingBottom(15).Column(c =>
                            {
                                c.Item().Text("Next Steps:")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken2);
                                
                                c.Item().PaddingTop(8).Text(text =>
                                {
                                    text.Line("Please review this renewal offer carefully. We would appreciate your response by " + 
                                             lease.EndDate.AddDays(-45).ToString("MMMM dd, yyyy") + ".");
                                    text.Line("");
                                    text.Line("To accept this renewal offer, please:");
                                    text.Line("  • Contact our office at your earliest convenience");
                                    text.Line("  • Sign and return the new lease agreement");
                                    text.Line("  • Continue to maintain the property in excellent condition");
                                });
                            });

                            // Closing
                            column.Item().PaddingTop(20).Column(c =>
                            {
                                c.Item().Text("We value you as a tenant and hope you will choose to renew your lease. " +
                                            "If you have any questions or concerns, please do not hesitate to contact us.");
                                
                                c.Item().PaddingTop(15).Text("Sincerely,");
                                c.Item().PaddingTop(30).Text("Property Management")
                                    .Bold();
                            });
                        });

                    page.Footer()
                        .Height(50)
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("This is an official lease renewal offer. Please retain this document for your records.");
                            text.Line("");
                            text.Span("Generated on " + DateTime.Now.ToString("MMMM dd, yyyy 'at' h:mm tt"))
                                .FontSize(8)
                                .FontColor(Colors.Grey.Darken1);
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}
