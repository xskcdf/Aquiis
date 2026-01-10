using Aquiis.UI.Shared.Components.Common;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Aquiis.UI.Shared.Tests.Components.Common;

public class DataTableTests : TestContext
{
    private class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    [Fact]
    public void DataTable_Renders_With_Empty_Items()
    {
        // Arrange & Act
        var cut = Render<DataTable<TestItem>>(parameters => parameters
            .Add(p => p.Items, new List<TestItem>())
        );

        // Assert
        cut.Markup.Should().Contain("No data available");
    }

    [Fact]
    public void DataTable_Renders_With_Null_Items()
    {
        // Arrange & Act
        var cut = Render<DataTable<TestItem>>(parameters => parameters
            .Add(p => p.Items, null)
        );

        // Assert
        cut.Markup.Should().Contain("No data available");
    }

    [Fact]
    public void DataTable_Renders_Header_Template()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Id = 1, Name = "Test" }
        };

        // Act
        var cut = Render<DataTable<TestItem>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.HeaderTemplate, (RenderFragment)(builder =>
            {
                builder.OpenElement(0, "th");
                builder.AddContent(1, "ID");
                builder.CloseElement();
                builder.OpenElement(2, "th");
                builder.AddContent(3, "Name");
                builder.CloseElement();
            }))
        );

        // Assert
        cut.Markup.Should().Contain("ID");
        cut.Markup.Should().Contain("Name");
        cut.Markup.Should().Contain("<thead>");
    }

    [Fact]
    public void DataTable_Renders_Row_Template()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Id = 1, Name = "Item 1", Description = "Description 1" },
            new TestItem { Id = 2, Name = "Item 2", Description = "Description 2" }
        };

        // Act
        var cut = Render<DataTable<TestItem>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.RowTemplate, (RenderFragment<TestItem>)(item => builder =>
            {
                builder.OpenElement(0, "td");
                builder.AddContent(1, item.Id);
                builder.CloseElement();
                builder.OpenElement(2, "td");
                builder.AddContent(3, item.Name);
                builder.CloseElement();
            }))
        );

        // Assert
        cut.Markup.Should().Contain("Item 1");
        cut.Markup.Should().Contain("Item 2");
        cut.Markup.Should().Contain("<tbody>");
    }

    [Fact]
    public void DataTable_Renders_Multiple_Rows()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new TestItem { Id = 1, Name = "First" },
            new TestItem { Id = 2, Name = "Second" },
            new TestItem { Id = 3, Name = "Third" }
        };

        // Act
        var cut = Render<DataTable<TestItem>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.RowTemplate, (RenderFragment<TestItem>)(item => builder =>
            {
                builder.OpenElement(0, "td");
                builder.AddContent(1, item.Name);
                builder.CloseElement();
            }))
        );

        // Assert
        var tbody = cut.Find("tbody");
        var rows = tbody.QuerySelectorAll("tr");
        rows.Length.Should().Be(3);
        cut.Markup.Should().Contain("First");
        cut.Markup.Should().Contain("Second");
        cut.Markup.Should().Contain("Third");
    }

    [Fact]
    public void DataTable_Applies_Custom_CssClass()
    {
        // Arrange & Act
        var cut = Render<DataTable<TestItem>>(parameters => parameters
            .Add(p => p.Items, new List<TestItem>())
            .Add(p => p.TableCssClass, "custom-table-class")
        );

        // Assert
        cut.Markup.Should().Contain("custom-table-class");
    }

    [Fact]
    public void DataTable_Renders_With_Custom_EmptyMessage()
    {
        // Arrange & Act
        var cut = Render<DataTable<TestItem>>(parameters => parameters
            .Add(p => p.Items, new List<TestItem>())
            .Add(p => p.EmptyMessage, "Custom empty message")
        );

        // Assert
        cut.Markup.Should().Contain("Custom empty message");
        cut.Markup.Should().NotContain("No data available");
    }

    [Fact]
    public void DataTable_Has_Bootstrap_Table_Classes()
    {
        // Arrange & Act
        var cut = Render<DataTable<TestItem>>(parameters => parameters
            .Add(p => p.Items, new List<TestItem>())
        );

        // Assert
        cut.Markup.Should().Contain("table");
        cut.Markup.Should().Contain("table-striped");
        cut.Markup.Should().Contain("table-hover");
    }
}
