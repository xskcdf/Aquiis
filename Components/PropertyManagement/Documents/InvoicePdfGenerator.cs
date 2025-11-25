using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Aquiis.SimpleStart.Components.PropertyManagement.Invoices;

namespace Aquiis.SimpleStart.Components.PropertyManagement.Documents
{
    public class InvoicePdfGenerator
    {
        public static byte[] GenerateInvoicePdf(Invoice invoice)
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

                    page.Header().Element(content => ComposeHeader(content, invoice));
                    page.Content().Element(content => ComposeContent(content, invoice));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeHeader(IContainer container, Invoice invoice)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("INVOICE").FontSize(24).Bold();
                        col.Item().PaddingTop(5).Text($"Invoice #: {invoice.InvoiceNumber}").FontSize(12).Bold();
                    });

                    row.ConstantItem(150).Column(col =>
                    {
                        col.Item().AlignRight().Text($"Date: {invoice.InvoicedOn:MMMM dd, yyyy}").FontSize(10);
                        col.Item().AlignRight().Text($"Due Date: {invoice.DueOn:MMMM dd, yyyy}").FontSize(10);
                        col.Item().PaddingTop(5).AlignRight()
                            .Background(GetStatusColor(invoice.Status))
                            .Padding(5)
                            .Text(invoice.Status).FontColor(Colors.White).Bold();
                    });
                });

                column.Item().PaddingTop(10).LineHorizontal(2).LineColor(Colors.Grey.Darken2);
            });
        }

        private static void ComposeContent(IContainer container, Invoice invoice)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(15);

                // Bill To Section
                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeBillTo(c, invoice));
                    row.ConstantItem(20);
                    row.RelativeItem().Element(c => ComposePropertyInfo(c, invoice));
                });

                // Invoice Details
                column.Item().PaddingTop(10).Element(c => ComposeInvoiceDetails(c, invoice));

                // Payments Section (if any)
                if (invoice.Payments != null && invoice.Payments.Any(p => !p.IsDeleted))
                {
                    column.Item().PaddingTop(15).Element(c => ComposePaymentsSection(c, invoice));
                }

                // Total Section
                column.Item().PaddingTop(20).Element(c => ComposeTotalSection(c, invoice));

                // Notes Section
                if (!string.IsNullOrWhiteSpace(invoice.Notes))
                {
                    column.Item().PaddingTop(20).Element(c => ComposeNotes(c, invoice));
                }
            });
        }

        private static void ComposeBillTo(IContainer container, Invoice invoice)
        {
            container.Column(column =>
            {
                column.Item().Text("BILL TO:").FontSize(10).Bold().FontColor(Colors.Grey.Darken1);
                column.Item().PaddingTop(5).Column(col =>
                {
                    if (invoice.Lease?.Tenant != null)
                    {
                        col.Item().Text(invoice.Lease.Tenant.FullName ?? "N/A").FontSize(12).Bold();
                        col.Item().Text(invoice.Lease.Tenant.Email ?? "").FontSize(10);
                        col.Item().Text(invoice.Lease.Tenant.PhoneNumber ?? "").FontSize(10);
                    }
                });
            });
        }

        private static void ComposePropertyInfo(IContainer container, Invoice invoice)
        {
            container.Column(column =>
            {
                column.Item().Text("PROPERTY:").FontSize(10).Bold().FontColor(Colors.Grey.Darken1);
                column.Item().PaddingTop(5).Column(col =>
                {
                    if (invoice.Lease?.Property != null)
                    {
                        col.Item().Text(invoice.Lease.Property.Address ?? "N/A").FontSize(12).Bold();
                        col.Item().Text($"{invoice.Lease.Property.City}, {invoice.Lease.Property.State} {invoice.Lease.Property.ZipCode}").FontSize(10);
                    }
                });
            });
        }

        private static void ComposeInvoiceDetails(IContainer container, Invoice invoice)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(8).Text("Description").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(8).AlignRight().Text("Amount").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(8).AlignRight().Text("Status").Bold();
                });

                // Row
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(8)
                    .Text($"{invoice.Description}\nPeriod: {invoice.InvoicedOn:MMM dd, yyyy} - {invoice.DueOn:MMM dd, yyyy}");
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(8)
                    .AlignRight().Text(invoice.Amount.ToString("C")).FontSize(12);
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(8)
                    .AlignRight().Text(invoice.Status);
            });
        }

        private static void ComposePaymentsSection(IContainer container, Invoice invoice)
        {
            container.Column(column =>
            {
                column.Item().Text("PAYMENTS RECEIVED:").FontSize(12).Bold();
                column.Item().PaddingTop(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1);
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Date").Bold().FontSize(9);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Method").Bold().FontSize(9);
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Amount").Bold().FontSize(9);
                    });

                    // Rows
                    foreach (var payment in invoice.Payments.Where(p => !p.IsDeleted).OrderBy(p => p.PaidOn))
                    {
                        table.Cell().Padding(5).Text(payment.PaidOn.ToString("MMM dd, yyyy")).FontSize(9);
                        table.Cell().Padding(5).Text(payment.PaymentMethod ?? "N/A").FontSize(9);
                        table.Cell().Padding(5).AlignRight().Text(payment.Amount.ToString("C")).FontSize(9);
                    }
                });
            });
        }

        private static void ComposeTotalSection(IContainer container, Invoice invoice)
        {
            container.AlignRight().Column(column =>
            {
                column.Spacing(5);

                column.Item().BorderTop(1).BorderColor(Colors.Grey.Darken1).PaddingTop(10).Row(row =>
                {
                    row.ConstantItem(150).Text("Invoice Total:").FontSize(12);
                    row.ConstantItem(100).AlignRight().Text(invoice.Amount.ToString("C")).FontSize(12).Bold();
                });

                column.Item().Row(row =>
                {
                    row.ConstantItem(150).Text("Paid Amount:").FontSize(12);
                    row.ConstantItem(100).AlignRight().Text(invoice.AmountPaid.ToString("C")).FontSize(12).FontColor(Colors.Green.Darken2);
                });

                column.Item().BorderTop(2).BorderColor(Colors.Grey.Darken2).PaddingTop(5).Row(row =>
                {
                    row.ConstantItem(150).Text("Balance Due:").FontSize(14).Bold();
                    row.ConstantItem(100).AlignRight().Text((invoice.Amount - invoice.AmountPaid).ToString("C"))
                        .FontSize(14).Bold().FontColor(invoice.Status == "Paid" ? Colors.Green.Darken2 : Colors.Red.Darken2);
                });
            });
        }

        private static void ComposeNotes(IContainer container, Invoice invoice)
        {
            container.Column(column =>
            {
                column.Item().Text("NOTES:").FontSize(10).Bold().FontColor(Colors.Grey.Darken1);
                column.Item().PaddingTop(5).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10)
                    .Text(invoice.Notes).FontSize(9);
            });
        }

        private static string GetStatusColor(string status)
        {
            return status switch
            {
                "Paid" => Colors.Green.Darken2,
                "Overdue" => Colors.Red.Darken2,
                "Pending" => Colors.Orange.Darken1,
                "Partially Paid" => Colors.Blue.Darken1,
                _ => Colors.Grey.Darken1
            };
        }
    }
}
