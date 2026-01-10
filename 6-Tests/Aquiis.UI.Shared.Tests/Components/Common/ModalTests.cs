using Aquiis.UI.Shared.Components.Common;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Aquiis.UI.Shared.Tests.Components.Common;

public class ModalTests : TestContext
{
    [Fact]
    public void Modal_Renders_When_IsVisible_Is_True()
    {
        // Arrange & Act
        var cut = Render<Modal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, "Test Modal")
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Test Content")))
        );

        // Assert
        cut.Markup.Should().Contain("Test Modal");
        cut.Markup.Should().Contain("Test Content");
        cut.Markup.Should().Contain("modal fade show d-block");
    }

    [Fact]
    public void Modal_DoesNotRender_When_IsOpen_Is_False()
    {
        // Arrange & Act
        var cut = Render<Modal>(parameters => parameters
            .Add(p => p.IsVisible, false)
            .Add(p => p.Title, "Test Modal")
        );

        // Assert
        cut.Markup.Should().BeNullOrWhiteSpace();
    }

    [Fact]
    public void Modal_Renders_With_Small_Size()
    {
        // Arrange & Act
        var cut = Render<Modal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, "Small Modal")
            .Add(p => p.Size, ModalSize.Small)
        );

        // Assert
        cut.Markup.Should().Contain("modal-sm");
    }

    [Fact]
    public void Modal_Renders_With_Large_Size()
    {
        // Arrange & Act
        var cut = Render<Modal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, "Large Modal")
            .Add(p => p.Size, ModalSize.Large)
        );

        // Assert
        cut.Markup.Should().Contain("modal-lg");
    }

    [Fact]
    public void Modal_Renders_With_ExtraLarge_Size()
    {
        // Arrange & Act
        var cut = Render<Modal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, "Extra Large Modal")
            .Add(p => p.Size, ModalSize.ExtraLarge)
        );

        // Assert
        cut.Markup.Should().Contain("modal-xl");
    }

    [Fact]
    public void Modal_Renders_Centered()
    {
        // Arrange & Act
        var cut = Render<Modal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, "Centered Modal")
            .Add(p => p.Position, ModalPosition.Centered)
        );

        // Assert
        cut.Markup.Should().Contain("modal-dialog-centered");
    }

    [Fact]
    public void Modal_Renders_With_Backdrop()
    {
        // Arrange & Act
        var cut = Render<Modal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, "Modal with Backdrop")
            .Add(p => p.CloseOnBackdropClick, true)
        );

        // Assert
        cut.Markup.Should().Contain("rgba(0,0,0,0.5)");
    }

    [Fact]
    public void Modal_Calls_OnClose_When_Close_Button_Clicked()
    {
        // Arrange
        var closeCalled = false;
        var cut = Render<Modal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, "Test Modal")
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => closeCalled = true))
        );

        // Act
        var closeButton = cut.Find(".btn-close");
        closeButton.Click();

        // Assert
        closeCalled.Should().BeTrue();
    }

    [Fact]
    public void Modal_Renders_Footer_Content()
    {
        // Arrange & Act
        var cut = Render<Modal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, "Modal with Footer")
            .Add(p => p.FooterContent, (RenderFragment)(builder => builder.AddContent(0, "Footer Content")))
        );

        // Assert
        cut.Markup.Should().Contain("Footer Content");
        cut.Markup.Should().Contain("modal-footer");
    }

    [Fact]
    public void Modal_DoesNotRender_Footer_When_No_Content()
    {
        // Arrange & Act
        var cut = Render<Modal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Title, "Modal without Footer")
        );

        // Assert
        cut.Markup.Should().NotContain("modal-footer");
    }
}
