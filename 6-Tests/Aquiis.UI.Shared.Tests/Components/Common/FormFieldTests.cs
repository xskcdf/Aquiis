using Aquiis.UI.Shared.Components.Common;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Aquiis.UI.Shared.Tests.Components.Common;

public class FormFieldTests : TestContext
{
    [Fact]
    public void FormField_Renders_Label()
    {
        // Arrange & Act
        var cut = Render<FormField>(parameters => parameters
            .Add(p => p.Label, "Test Label")
        );

        // Assert
        cut.Markup.Should().Contain("Test Label");
        cut.Markup.Should().Contain("form-label");
    }

    [Fact]
    public void FormField_Renders_ChildContent()
    {
        // Arrange & Act
        var cut = Render<FormField>(parameters => parameters
            .Add(p => p.Label, "Test Field")
            .AddChildContent("<input class='form-control' />")
        );

        // Assert
        cut.Markup.Should().Contain("input");
        cut.Markup.Should().Contain("form-control");
    }

    [Fact]
    public void FormField_Shows_Required_Indicator_When_Required_Is_True()
    {
        // Arrange & Act
        var cut = Render<FormField>(parameters => parameters
            .Add(p => p.Label, "Required Field")
            .Add(p => p.Required, true)
        );

        // Assert
        cut.Markup.Should().Contain("*");
        cut.Markup.Should().Contain("text-danger");
    }

    [Fact]
    public void FormField_Does_Not_Show_Required_Indicator_When_Required_Is_False()
    {
        // Arrange & Act
        var cut = Render<FormField>(parameters => parameters
            .Add(p => p.Label, "Optional Field")
            .Add(p => p.Required, false)
        );

        // Assert
        cut.Markup.Should().Contain("Optional Field");
        cut.Markup.Should().NotContain("text-danger");
    }

    [Fact]
    public void FormField_Renders_HelpText()
    {
        // Arrange & Act
        var cut = Render<FormField>(parameters => parameters
            .Add(p => p.Label, "Test Field")
            .Add(p => p.HelpText, "This is help text")
        );

        // Assert
        cut.Markup.Should().Contain("This is help text");
        cut.Markup.Should().Contain("form-text");
    }

    [Fact]
    public void FormField_Does_Not_Render_HelpText_When_Null()
    {
        // Arrange & Act
        var cut = Render<FormField>(parameters => parameters
            .Add(p => p.Label, "Test Field")
        );

        // Assert
        cut.Markup.Should().NotContain("form-text");
    }

    [Fact]
    public void FormField_Has_Form_Group_Class()
    {
        // Arrange & Act
        var cut = Render<FormField>(parameters => parameters
            .Add(p => p.Label, "Test Field")
        );

        // Assert
        cut.Markup.Should().Contain("mb-3");
    }

    [Fact]
    public void FormField_Applies_Custom_CssClass()
    {
        // Arrange & Act
        var cut = Render<FormField>(parameters => parameters
            .Add(p => p.Label, "Test Field")
            .Add(p => p.CssClass, "custom-field")
        );

        // Assert
        cut.Markup.Should().Contain("custom-field");
    }

    [Fact]
    public void FormField_Renders_Complete_Structure()
    {
        // Arrange & Act
        var cut = Render<FormField>(parameters => parameters
            .Add(p => p.Label, "Complete Field")
            .Add(p => p.Required, true)
            .Add(p => p.HelpText, "Help text")
            .AddChildContent("<input class='form-control' />")
        );

        // Assert
        cut.Markup.Should().Contain("Complete Field");
        cut.Markup.Should().Contain("*");
        cut.Markup.Should().Contain("Help text");
        cut.Markup.Should().Contain("input");
    }
}
