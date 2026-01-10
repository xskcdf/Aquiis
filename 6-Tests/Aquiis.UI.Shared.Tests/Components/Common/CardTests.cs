using Aquiis.UI.Shared.Components.Common;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Aquiis.UI.Shared.Tests.Components.Common;

public class CardTests : TestContext
{
    [Fact]
    public void Card_Renders_With_Title()
    {
        // Arrange & Act
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.Title, "Test Card")
        );

        // Assert
        cut.Markup.Should().Contain("Test Card");
        cut.Markup.Should().Contain("card-header");
    }

    [Fact]
    public void Card_Renders_Body_Content()
    {
        // Arrange & Act
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Body Content")))
        );

        // Assert
        cut.Markup.Should().Contain("Body Content");
        cut.Markup.Should().Contain("card-body");
    }

    [Fact]
    public void Card_Renders_Header_Content()
    {
        // Arrange & Act
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.HeaderContent, (RenderFragment)(builder => builder.AddContent(0, "Custom Header")))
        );

        // Assert
        cut.Markup.Should().Contain("Custom Header");
        cut.Markup.Should().Contain("card-header");
    }

    [Fact]
    public void Card_Renders_Footer_Content()
    {
        // Arrange & Act
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.FooterContent, (RenderFragment)(builder => builder.AddContent(0, "Footer Content")))
        );

        // Assert
        cut.Markup.Should().Contain("Footer Content");
        cut.Markup.Should().Contain("card-footer");
    }

    [Fact]
    public void Card_DoesNotRender_Header_When_No_Title_Or_HeaderContent()
    {
        // Arrange & Act
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Just Body")))
        );

        // Assert
        cut.Markup.Should().NotContain("card-header");
    }

    [Fact]
    public void Card_DoesNotRender_Footer_When_No_FooterContent()
    {
        // Arrange & Act
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Just Body")))
        );

        // Assert
        cut.Markup.Should().NotContain("card-footer");
    }

    [Fact]
    public void Card_Prefers_HeaderContent_Over_Title()
    {
        // Arrange & Act
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.Title, "Title Text")
            .Add(p => p.HeaderContent, (RenderFragment)(builder => builder.AddContent(0, "Custom Header")))
        );

        // Assert
        cut.Markup.Should().Contain("Custom Header");
        cut.Markup.Should().NotContain("Title Text");
    }

    [Fact]
    public void Card_Renders_With_Custom_CssClass()
    {
        // Arrange & Act
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.CssClass, "custom-card-class")
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Content")))
        );

        // Assert
        cut.Markup.Should().Contain("custom-card-class");
    }

    [Fact]
    public void Card_Renders_All_Sections_Together()
    {
        // Arrange & Act
        var cut = Render<Card>(parameters => parameters
            .Add(p => p.Title, "Card Title")
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Body")))
            .Add(p => p.FooterContent, (RenderFragment)(builder => builder.AddContent(0, "Footer")))
        );

        // Assert
        cut.Markup.Should().Contain("Card Title");
        cut.Markup.Should().Contain("Body");
        cut.Markup.Should().Contain("Footer");
        cut.Markup.Should().Contain("card-header");
        cut.Markup.Should().Contain("card-body");
        cut.Markup.Should().Contain("card-footer");
    }
}
