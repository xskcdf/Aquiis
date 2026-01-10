using Aquiis.Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;

namespace Aquiis.Application.Services.PdfGenerators;

public class ChecklistPdfGenerator
{
    private static bool _fontsRegistered = false;

    public ChecklistPdfGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        // Register fonts once
        if (!_fontsRegistered)
        {
            try
            {
                // Register Lato fonts (from QuestPDF package)
                var latoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LatoFont");
                if (Directory.Exists(latoPath))
                {
                    FontManager.RegisterFont(File.OpenRead(Path.Combine(latoPath, "Lato-Regular.ttf")));
                    FontManager.RegisterFont(File.OpenRead(Path.Combine(latoPath, "Lato-Bold.ttf")));
                    FontManager.RegisterFont(File.OpenRead(Path.Combine(latoPath, "Lato-Italic.ttf")));
                    FontManager.RegisterFont(File.OpenRead(Path.Combine(latoPath, "Lato-BoldItalic.ttf")));
                }
                
                // Register DejaVu fonts (custom fonts for Unicode support)
                var dejaVuPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "DejaVu");
                if (Directory.Exists(dejaVuPath))
                {
                    FontManager.RegisterFont(File.OpenRead(Path.Combine(dejaVuPath, "DejaVuSans.ttf")));
                    FontManager.RegisterFont(File.OpenRead(Path.Combine(dejaVuPath, "DejaVuSans-Bold.ttf")));
                    FontManager.RegisterFont(File.OpenRead(Path.Combine(dejaVuPath, "DejaVuSans-Oblique.ttf")));
                    FontManager.RegisterFont(File.OpenRead(Path.Combine(dejaVuPath, "DejaVuSans-BoldOblique.ttf")));
                }
                
                _fontsRegistered = true;
            }
            catch
            {
                // If fonts aren't available, QuestPDF will fall back to default fonts
            }
        }
    }

    public byte[] GenerateChecklistPdf(Checklist checklist)
    {
        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("DejaVu Sans"));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Text(text =>
                        {
                            text.Span("CHECKLIST REPORT\n").FontSize(20).Bold();
                            text.Span($"{checklist.Name}\n").FontSize(14).SemiBold();
                        });

                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Type: {checklist.ChecklistType}").FontSize(10);
                                col.Item().Text($"Status: {checklist.Status}").FontSize(10);
                                col.Item().Text($"Created: {checklist.CreatedOn:MMM dd, yyyy}").FontSize(10);
                                if (checklist.CompletedOn.HasValue)
                                {
                                    col.Item().Text($"Completed: {checklist.CompletedOn:MMM dd, yyyy}").FontSize(10);
                                }
                            });

                            row.RelativeItem().Column(col =>
                            {
                                if (checklist.Property != null)
                                {
                                    col.Item().Text("Property:").FontSize(10).Bold();
                                    col.Item().Text($"{checklist.Property.Address ?? "N/A"}").FontSize(10);
                                    col.Item().Text($"{checklist.Property.City ?? ""}, {checklist.Property.State ?? ""} {checklist.Property.ZipCode ?? ""}").FontSize(10);
                                }
                                if (checklist.Lease?.Tenant != null)
                                {
                                    col.Item().Text($"Tenant: {checklist.Lease.Tenant.FirstName ?? ""} {checklist.Lease.Tenant.LastName ?? ""}").FontSize(10);
                                }
                            });
                        });

                        column.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        if (checklist.Items == null || !checklist.Items.Any())
                        {
                            column.Item().Text("No items in this checklist.").Italic().FontSize(10);
                            return;
                        }

                        // Group items by section
                        var groupedItems = checklist.Items
                            .OrderBy(i => i.ItemOrder)
                            .GroupBy(i => i.CategorySection ?? "General");

                        foreach (var group in groupedItems)
                        {
                            column.Item().PaddingBottom(5).Text(group.Key)
                                .FontSize(13)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(25); // Checkbox
                                    columns.RelativeColumn(3); // Item text
                                    columns.RelativeColumn(1); // Value
                                    columns.RelativeColumn(2); // Notes
                                });

                                // Header
                                table.Cell().Element(HeaderStyle).Text("✓");
                                table.Cell().Element(HeaderStyle).Text("Item");
                                table.Cell().Element(HeaderStyle).Text("Value");
                                table.Cell().Element(HeaderStyle).Text("Notes");

                                // Items
                                foreach (var item in group)
                                {
                                    table.Cell()
                                        .Element(CellStyle)
                                        .AlignCenter()
                                        .Text(item.IsChecked ? "☑" : "☐")
                                        .FontSize(12);

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(item.ItemText);

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(item.Value ?? "-")
                                        .FontSize(10);

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(item.Notes ?? "-")
                                        .FontSize(9)
                                        .Italic();
                                }
                            });

                            column.Item().PaddingBottom(10);
                        }

                        // General Notes Section
                        if (!string.IsNullOrWhiteSpace(checklist.GeneralNotes))
                        {
                            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                            
                            column.Item().PaddingTop(10).Column(col =>
                            {
                                col.Item().Text("General Notes").FontSize(12).Bold();
                                col.Item().PaddingTop(5).Border(1).BorderColor(Colors.Grey.Lighten2)
                                    .Padding(10).Background(Colors.Grey.Lighten4)
                                    .Text(checklist.GeneralNotes).FontSize(10);
                            });
                        }

                        // Summary
                        column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            var totalItems = checklist.Items.Count;
                            var checkedItems = checklist.Items.Count(i => i.IsChecked);
                            var itemsWithValues = checklist.Items.Count(i => !string.IsNullOrEmpty(i.Value));
                            var itemsWithNotes = checklist.Items.Count(i => !string.IsNullOrEmpty(i.Notes));
                            var progressPercent = totalItems > 0 ? (int)((checkedItems * 100.0) / totalItems) : 0;

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("Summary").FontSize(12).Bold();
                                col.Item().Text($"Total Items: {totalItems}").FontSize(10);
                                col.Item().Text($"Checked: {checkedItems} ({progressPercent}%)").FontSize(10);
                                col.Item().Text($"Unchecked: {totalItems - checkedItems}").FontSize(10);
                            });

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Items with Values: {itemsWithValues}").FontSize(10);
                                col.Item().Text($"Items with Notes: {itemsWithNotes}").FontSize(10);
                                if (checklist.CompletedBy != null)
                                {
                                    col.Item().PaddingTop(5).Text($"Completed By: {checklist.CompletedBy}").FontSize(10);
                                }
                            });
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .DefaultTextStyle(x => x.FontSize(9))
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                        text.Span($" • Generated on {DateTime.Now:MMM dd, yyyy h:mm tt}");
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5);
    }

    private static IContainer HeaderStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Medium)
            .Background(Colors.Grey.Lighten3)
            .Padding(5)
            .DefaultTextStyle(x => x.FontSize(10).Bold());
    }
}
