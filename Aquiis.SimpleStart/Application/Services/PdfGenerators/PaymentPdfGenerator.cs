using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Aquiis.SimpleStart.Core.Entities;

namespace Aquiis.SimpleStart.Application.Services.PdfGenerators
{
    public class PaymentPdfGenerator
    {
        public static byte[] GeneratePaymentReceipt(Payment payment)
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

                    page.Header().Element(content => ComposeHeader(content, payment));
                    page.Content().Element(content => ComposeContent(content, payment));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated: ");
                        x.Span(DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"));
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeHeader(IContainer container, Payment payment)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("PAYMENT RECEIPT").FontSize(24).Bold();
                        col.Item().PaddingTop(5).Text($"Receipt Date: {payment.PaidOn:MMMM dd, yyyy}").FontSize(12);
                    });

                    row.ConstantItem(150).Column(col =>
                    {
                        col.Item().AlignRight()
                            .Background(Colors.Green.Darken2)
                            .Padding(10)
                            .Text("PAID").FontColor(Colors.White).FontSize(16).Bold();
                    });
                });

                column.Item().PaddingTop(10).LineHorizontal(2).LineColor(Colors.Grey.Darken2);
            });
        }

        private static void ComposeContent(IContainer container, Payment payment)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(20);

                // Payment Amount (Prominent)
                column.Item().Background(Colors.Grey.Lighten3).Padding(20).Column(col =>
                {
                    col.Item().AlignCenter().Text("AMOUNT PAID").FontSize(14).FontColor(Colors.Grey.Darken1);
                    col.Item().AlignCenter().Text(payment.Amount.ToString("C")).FontSize(32).Bold().FontColor(Colors.Green.Darken2);
                });

                // Payment Information
                column.Item().Element(c => ComposePaymentInfo(c, payment));

                // Invoice Information
                if (payment.Invoice != null)
                {
                    column.Item().Element(c => ComposeInvoiceInfo(c, payment));
                }

                // Tenant and Property Information
                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => ComposeTenantInfo(c, payment));
                    row.ConstantItem(20);
                    row.RelativeItem().Element(c => ComposePropertyInfo(c, payment));
                });

                // Additional Information
                if (!string.IsNullOrWhiteSpace(payment.Notes))
                {
                    column.Item().Element(c => ComposeNotes(c, payment));
                }

                // Footer Message
                column.Item().PaddingTop(30).AlignCenter().Text("Thank you for your payment!")
                    .FontSize(14).Italic().FontColor(Colors.Grey.Darken1);
            });
        }

        private static void ComposePaymentInfo(IContainer container, Payment payment)
        {
            container.Column(column =>
            {
                column.Item().Background(Colors.Blue.Lighten4).Padding(10).Text("PAYMENT DETAILS").FontSize(12).Bold();
                column.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(15).Column(col =>
                {
                    col.Spacing(8);

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Payment Date:").Bold();
                        row.RelativeItem().Text(payment.PaidOn.ToString("MMMM dd, yyyy"));
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Payment Method:").Bold();
                        row.RelativeItem().Text(payment.PaymentMethod ?? "N/A");
                    });

                    if (!string.IsNullOrWhiteSpace(payment.Invoice.InvoiceNumber))
                    {
                        col.Item().Row(row =>
                        {
                            row.ConstantItem(150).Text("Transaction Reference:").Bold();
                            row.RelativeItem().Text(payment.Invoice.InvoiceNumber);
                        });
                    }

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Amount Paid:").Bold();
                        row.RelativeItem().Text(payment.Amount.ToString("C")).FontSize(14).FontColor(Colors.Green.Darken2).Bold();
                    });
                });
            });
        }

        private static void ComposeInvoiceInfo(IContainer container, Payment payment)
        {
            container.Column(column =>
            {
                column.Item().Background(Colors.Grey.Lighten3).Padding(10).Text("INVOICE INFORMATION").FontSize(12).Bold();
                column.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(15).Column(col =>
                {
                    col.Spacing(8);

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Invoice Number:").Bold();
                        row.RelativeItem().Text(payment.Invoice!.InvoiceNumber ?? "N/A");
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Invoice Date:").Bold();
                        row.RelativeItem().Text(payment.Invoice.InvoicedOn.ToString("MMMM dd, yyyy"));
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Due Date:").Bold();
                        row.RelativeItem().Text(payment.Invoice.DueOn.ToString("MMMM dd, yyyy"));
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Invoice Total:").Bold();
                        row.RelativeItem().Text(payment.Invoice.Amount.ToString("C"));
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Total Paid:").Bold();
                        row.RelativeItem().Text(payment.Invoice.AmountPaid.ToString("C"));
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Balance Remaining:").Bold();
                        row.RelativeItem().Text((payment.Invoice.Amount - payment.Invoice.AmountPaid).ToString("C"))
                            .FontColor(payment.Invoice.Status == "Paid" ? Colors.Green.Darken2 : Colors.Orange.Darken1);
                    });

                    col.Item().Row(row =>
                    {
                        row.ConstantItem(150).Text("Invoice Status:").Bold();
                        row.RelativeItem().Text(payment.Invoice.Status ?? "N/A")
                            .FontColor(payment.Invoice.Status == "Paid" ? Colors.Green.Darken2 : Colors.Grey.Darken1);
                    });
                });
            });
        }

        private static void ComposeTenantInfo(IContainer container, Payment payment)
        {
            container.Column(column =>
            {
                column.Item().Text("TENANT INFORMATION").FontSize(10).Bold().FontColor(Colors.Grey.Darken1);
                column.Item().PaddingTop(5).Column(col =>
                {
                    if (payment.Invoice?.Lease?.Tenant != null)
                    {
                        var tenant = payment.Invoice.Lease.Tenant;
                        col.Item().Text(tenant.FullName ?? "N/A").FontSize(12).Bold();
                        col.Item().Text(tenant.Email ?? "").FontSize(10);
                        col.Item().Text(tenant.PhoneNumber ?? "").FontSize(10);
                    }
                    else
                    {
                        col.Item().Text("N/A").FontSize(10);
                    }
                });
            });
        }

        private static void ComposePropertyInfo(IContainer container, Payment payment)
        {
            container.Column(column =>
            {
                column.Item().Text("PROPERTY INFORMATION").FontSize(10).Bold().FontColor(Colors.Grey.Darken1);
                column.Item().PaddingTop(5).Column(col =>
                {
                    if (payment.Invoice?.Lease?.Property != null)
                    {
                        var property = payment.Invoice.Lease.Property;
                        col.Item().Text(property.Address ?? "N/A").FontSize(12).Bold();
                        col.Item().Text($"{property.City}, {property.State} {property.ZipCode}").FontSize(10);
                        if (!string.IsNullOrWhiteSpace(property.PropertyType))
                        {
                            col.Item().Text($"Type: {property.PropertyType}").FontSize(10);
                        }
                    }
                    else
                    {
                        col.Item().Text("N/A").FontSize(10);
                    }
                });
            });
        }

        private static void ComposeNotes(IContainer container, Payment payment)
        {
            container.Column(column =>
            {
                column.Item().Text("NOTES:").FontSize(10).Bold().FontColor(Colors.Grey.Darken1);
                column.Item().PaddingTop(5).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10)
                    .Text(payment.Notes).FontSize(9);
            });
        }
    }
}
