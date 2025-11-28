using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Aquiis.SimpleStart.Components.PropertyManagement.Inspections;

namespace Aquiis.SimpleStart.Components.PropertyManagement.Documents;

public class InspectionPdfGenerator
{
    public byte[] GenerateInspectionPdf(Inspection inspection)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header()
                    .Height(100)
                    .Background(Colors.Blue.Darken3)
                    .Padding(20)
                    .Column(column =>
                    {
                        column.Item().Text("PROPERTY INSPECTION REPORT")
                            .FontSize(20)
                            .Bold()
                            .FontColor(Colors.White);
                        
                        column.Item().PaddingTop(5).Text(text =>
                        {
                            text.Span("Inspection Date: ").FontColor(Colors.White);
                            text.Span(inspection.CompletedOn.ToString("MMMM dd, yyyy"))
                                .Bold()
                                .FontColor(Colors.White);
                        });
                    });

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        // Property Information
                        column.Item().Element(c => PropertySection(c, inspection));
                        
                        // Inspection Details
                        column.Item().PaddingTop(15).Element(c => InspectionDetailsSection(c, inspection));
                        
                        // Exterior
                        column.Item().PageBreak();
                        column.Item().Element(c => SectionHeader(c, "EXTERIOR INSPECTION"));
                        column.Item().Element(c => ChecklistTable(c, GetExteriorItems(inspection)));
                        
                        // Interior
                        column.Item().PaddingTop(15).Element(c => SectionHeader(c, "INTERIOR INSPECTION"));
                        column.Item().Element(c => ChecklistTable(c, GetInteriorItems(inspection)));
                        
                        // Kitchen
                        column.Item().PaddingTop(15).Element(c => SectionHeader(c, "KITCHEN"));
                        column.Item().Element(c => ChecklistTable(c, GetKitchenItems(inspection)));
                        
                        // Bathroom
                        column.Item().PageBreak();
                        column.Item().Element(c => SectionHeader(c, "BATHROOM"));
                        column.Item().Element(c => ChecklistTable(c, GetBathroomItems(inspection)));
                        
                        // Systems & Safety
                        column.Item().PaddingTop(15).Element(c => SectionHeader(c, "SYSTEMS & SAFETY"));
                        column.Item().Element(c => ChecklistTable(c, GetSystemsItems(inspection)));
                        
                        // Overall Assessment
                        column.Item().PageBreak();
                        column.Item().Element(c => OverallAssessmentSection(c, inspection));
                    });

                page.Footer()
                    .Height(30)
                    .AlignCenter()
                    .DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Medium))
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                        text.Span($" • Generated on {DateTime.Now:MMM dd, yyyy}");
                    });
            });
        });

        return document.GeneratePdf();
    }

    private void PropertySection(IContainer container, Inspection inspection)
    {
        container.Background(Colors.Grey.Lighten3)
            .Padding(15)
            .Column(column =>
            {
                column.Item().Text("PROPERTY INFORMATION")
                    .FontSize(14)
                    .Bold()
                    .FontColor(Colors.Blue.Darken3);
                
                column.Item().PaddingTop(10).Text(text =>
                {
                    text.Span("Address: ").Bold();
                    text.Span(inspection.Property?.Address ?? "N/A");
                });
                
                column.Item().PaddingTop(5).Text(text =>
                {
                    text.Span("Location: ").Bold();
                    text.Span($"{inspection.Property?.City}, {inspection.Property?.State} {inspection.Property?.ZipCode}");
                });
                
                if (inspection.Property != null)
                {
                    column.Item().PaddingTop(5).Text(text =>
                    {
                        text.Span("Type: ").Bold();
                        text.Span($"{inspection.Property.PropertyType} • ");
                        text.Span($"{inspection.Property.Bedrooms} bed • ");
                        text.Span($"{inspection.Property.Bathrooms} bath");
                    });
                }
            });
    }

    private void InspectionDetailsSection(IContainer container, Inspection inspection)
    {
        container.Border(1)
            .BorderColor(Colors.Grey.Lighten1)
            .Padding(15)
            .Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Inspection Type").FontSize(9).FontColor(Colors.Grey.Medium);
                    column.Item().PaddingTop(3).Text(inspection.InspectionType).Bold();
                });
                
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Overall Condition").FontSize(9).FontColor(Colors.Grey.Medium);
                    column.Item().PaddingTop(3).Text(inspection.OverallCondition)
                        .Bold()
                        .FontColor(GetConditionColor(inspection.OverallCondition));
                });
                
                if (!string.IsNullOrEmpty(inspection.InspectedBy))
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("Inspected By").FontSize(9).FontColor(Colors.Grey.Medium);
                        column.Item().PaddingTop(3).Text(inspection.InspectedBy).Bold();
                    });
                }
            });
    }

    private void SectionHeader(IContainer container, string title)
    {
        container.Background(Colors.Blue.Lighten4)
            .Padding(10)
            .Text(title)
            .FontSize(12)
            .Bold()
            .FontColor(Colors.Blue.Darken3);
    }

    private void ChecklistTable(IContainer container, List<(string Label, bool IsGood, string? Notes)> items)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2);
                columns.RelativeColumn(1);
                columns.RelativeColumn(3);
            });

            table.Header(header =>
            {
                header.Cell().Background(Colors.Grey.Lighten2).Padding(8)
                    .Text("Item").Bold().FontSize(9);
                header.Cell().Background(Colors.Grey.Lighten2).Padding(8)
                    .Text("Status").Bold().FontSize(9);
                header.Cell().Background(Colors.Grey.Lighten2).Padding(8)
                    .Text("Notes").Bold().FontSize(9);
            });

            foreach (var item in items)
            {
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(item.Label);
                
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(item.IsGood ? "✓ Good" : "✗ Issue")
                    .FontColor(item.IsGood ? Colors.Green.Darken2 : Colors.Red.Darken1)
                    .Bold();
                
                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                    .Text(item.Notes ?? "-")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);
            }
        });
    }

    private void OverallAssessmentSection(IContainer container, Inspection inspection)
    {
        container.Column(column =>
        {
            column.Item().Element(c => SectionHeader(c, "OVERALL ASSESSMENT"));
            
            column.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(15)
                .Column(innerColumn =>
                {
                    innerColumn.Item().Text(text =>
                    {
                        text.Span("Overall Condition: ").Bold();
                        text.Span(inspection.OverallCondition)
                            .Bold()
                            .FontColor(GetConditionColor(inspection.OverallCondition));
                    });
                    
                    if (!string.IsNullOrEmpty(inspection.GeneralNotes))
                    {
                        innerColumn.Item().PaddingTop(10).Text("General Notes:").Bold();
                        innerColumn.Item().PaddingTop(5).Text(inspection.GeneralNotes);
                    }
                    
                    if (!string.IsNullOrEmpty(inspection.ActionItemsRequired))
                    {
                        innerColumn.Item().PaddingTop(15)
                            .Background(Colors.Orange.Lighten4)
                            .Padding(10)
                            .Column(actionColumn =>
                            {
                                actionColumn.Item().Text("⚠ ACTION ITEMS REQUIRED")
                                    .Bold()
                                    .FontColor(Colors.Orange.Darken2);
                                actionColumn.Item().PaddingTop(5)
                                    .Text(inspection.ActionItemsRequired);
                            });
                    }
                });
            
            // Summary Statistics
            column.Item().PaddingTop(15).Background(Colors.Grey.Lighten4).Padding(15)
                .Row(row =>
                {
                    var stats = GetInspectionStats(inspection);
                    
                    row.RelativeItem().Column(statColumn =>
                    {
                        statColumn.Item().Text("Items Checked").FontSize(9).FontColor(Colors.Grey.Medium);
                        statColumn.Item().PaddingTop(3).Text(stats.TotalItems.ToString()).Bold().FontSize(16);
                    });
                    
                    row.RelativeItem().Column(statColumn =>
                    {
                        statColumn.Item().Text("Issues Found").FontSize(9).FontColor(Colors.Grey.Medium);
                        statColumn.Item().PaddingTop(3).Text(stats.IssuesCount.ToString())
                            .Bold()
                            .FontSize(16)
                            .FontColor(Colors.Red.Darken1);
                    });
                    
                    row.RelativeItem().Column(statColumn =>
                    {
                        statColumn.Item().Text("Pass Rate").FontSize(9).FontColor(Colors.Grey.Medium);
                        statColumn.Item().PaddingTop(3).Text($"{stats.PassRate:F0}%")
                            .Bold()
                            .FontSize(16)
                            .FontColor(Colors.Green.Darken2);
                    });
                });
        });
    }

    private string GetConditionColor(string condition) => condition switch
    {
        "Excellent" => "#28a745",
        "Good" => "#17a2b8",
        "Fair" => "#ffc107",
        "Poor" => "#dc3545",
        _ => "#6c757d"
    };

    private (int TotalItems, int IssuesCount, double PassRate) GetInspectionStats(Inspection inspection)
    {
        var allItems = new List<bool>
        {
            inspection.ExteriorRoofGood, inspection.ExteriorGuttersGood, inspection.ExteriorSidingGood,
            inspection.ExteriorWindowsGood, inspection.ExteriorDoorsGood, inspection.ExteriorFoundationGood,
            inspection.LandscapingGood, inspection.InteriorWallsGood, inspection.InteriorCeilingsGood,
            inspection.InteriorFloorsGood, inspection.InteriorDoorsGood, inspection.InteriorWindowsGood,
            inspection.KitchenAppliancesGood, inspection.KitchenCabinetsGood, inspection.KitchenCountersGood,
            inspection.KitchenSinkPlumbingGood, inspection.BathroomToiletGood, inspection.BathroomSinkGood,
            inspection.BathroomTubShowerGood, inspection.BathroomVentilationGood, inspection.HvacSystemGood,
            inspection.ElectricalSystemGood, inspection.PlumbingSystemGood, inspection.SmokeDetectorsGood,
            inspection.CarbonMonoxideDetectorsGood
        };
        
        int total = allItems.Count;
        int issues = allItems.Count(x => !x);
        double passRate = ((total - issues) / (double)total) * 100;
        
        return (total, issues, passRate);
    }

    private List<(string Label, bool IsGood, string? Notes)> GetExteriorItems(Inspection i) => new()
    {
        ("Roof", i.ExteriorRoofGood, i.ExteriorRoofNotes),
        ("Gutters & Downspouts", i.ExteriorGuttersGood, i.ExteriorGuttersNotes),
        ("Siding/Paint", i.ExteriorSidingGood, i.ExteriorSidingNotes),
        ("Windows", i.ExteriorWindowsGood, i.ExteriorWindowsNotes),
        ("Doors", i.ExteriorDoorsGood, i.ExteriorDoorsNotes),
        ("Foundation", i.ExteriorFoundationGood, i.ExteriorFoundationNotes),
        ("Landscaping & Drainage", i.LandscapingGood, i.LandscapingNotes)
    };

    private List<(string Label, bool IsGood, string? Notes)> GetInteriorItems(Inspection i) => new()
    {
        ("Walls", i.InteriorWallsGood, i.InteriorWallsNotes),
        ("Ceilings", i.InteriorCeilingsGood, i.InteriorCeilingsNotes),
        ("Floors", i.InteriorFloorsGood, i.InteriorFloorsNotes),
        ("Doors", i.InteriorDoorsGood, i.InteriorDoorsNotes),
        ("Windows", i.InteriorWindowsGood, i.InteriorWindowsNotes)
    };

    private List<(string Label, bool IsGood, string? Notes)> GetKitchenItems(Inspection i) => new()
    {
        ("Appliances", i.KitchenAppliancesGood, i.KitchenAppliancesNotes),
        ("Cabinets & Drawers", i.KitchenCabinetsGood, i.KitchenCabinetsNotes),
        ("Countertops", i.KitchenCountersGood, i.KitchenCountersNotes),
        ("Sink & Plumbing", i.KitchenSinkPlumbingGood, i.KitchenSinkPlumbingNotes)
    };

    private List<(string Label, bool IsGood, string? Notes)> GetBathroomItems(Inspection i) => new()
    {
        ("Toilet", i.BathroomToiletGood, i.BathroomToiletNotes),
        ("Sink & Vanity", i.BathroomSinkGood, i.BathroomSinkNotes),
        ("Tub/Shower", i.BathroomTubShowerGood, i.BathroomTubShowerNotes),
        ("Ventilation/Exhaust Fan", i.BathroomVentilationGood, i.BathroomVentilationNotes)
    };

    private List<(string Label, bool IsGood, string? Notes)> GetSystemsItems(Inspection i) => new()
    {
        ("HVAC System", i.HvacSystemGood, i.HvacSystemNotes),
        ("Electrical System", i.ElectricalSystemGood, i.ElectricalSystemNotes),
        ("Plumbing System", i.PlumbingSystemGood, i.PlumbingSystemNotes),
        ("Smoke Detectors", i.SmokeDetectorsGood, i.SmokeDetectorsNotes),
        ("Carbon Monoxide Detectors", i.CarbonMonoxideDetectorsGood, i.CarbonMonoxideDetectorsNotes)
    };
}
