using Aquiis.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace Aquiis.Core.Tests.Validation;

public class RequiredGuidAttributeTests
{
    [Fact]
    public void RequiredGuid_GuidEmpty_ReturnsFalse()
    {
        // Arrange
        var attribute = new RequiredGuidAttribute();
        var value = Guid.Empty;

        // Act
        var result = attribute.IsValid(value);

        // Assert   
        Assert.False(result);
    }

    [Fact]
    public void RequiredGuid_ValidGuid_ReturnsTrue()
    {
        // Arrange
        var attribute = new RequiredGuidAttribute();
        var value = Guid.NewGuid();

        // Act
        var result = attribute.IsValid(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void RequiredGuid_Null_ReturnsFalse()
    {
        // Arrange
        var attribute = new RequiredGuidAttribute();
        object? value = null;

        // Act
        var result = attribute.IsValid(value);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RequiredGuid_WithContext_GuidEmpty_ReturnsValidationError()
    {
        // Arrange
        var model = new TestModel { Id = Guid.Empty };
        var context = new ValidationContext(model) { MemberName = nameof(TestModel.Id) };
        var attribute = new RequiredGuidAttribute();

        // Act
        var result = attribute.GetValidationResult(Guid.Empty, context);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(ValidationResult.Success, result);
    }

    [Fact]
    public void RequiredGuid_WithContext_ValidGuid_ReturnsSuccess()
    {
        // Arrange
        var model = new TestModel { Id = Guid.NewGuid() };
        var context = new ValidationContext(model) { MemberName = nameof(TestModel.Id) };
        var attribute = new RequiredGuidAttribute();

        // Act
        var result = attribute.GetValidationResult(model.Id, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void RequiredGuid_CustomErrorMessage_UsesCustomMessage()
    {
        // Arrange
        var customMessage = "Custom error message";
        var attribute = new RequiredGuidAttribute(customMessage);
        var model = new TestModel { Id = Guid.Empty };
        var context = new ValidationContext(model) { MemberName = nameof(TestModel.Id) };

        // Act
        var result = attribute.GetValidationResult(Guid.Empty, context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customMessage, result!.ErrorMessage);
    }

    private class TestModel
    {
        public Guid Id { get; set; }
    }
}

public class OptionalGuidAttributeTests
{
    [Fact]
    public void OptionalGuid_Null_ReturnsTrue()
    {
        // Arrange
        var attribute = new OptionalGuidAttribute();
        object? value = null;

        // Act
        var result = attribute.IsValid(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OptionalGuid_GuidEmpty_ReturnsFalse()
    {
        // Arrange
        var attribute = new OptionalGuidAttribute();
        var value = Guid.Empty;

        // Act
        var result = attribute.IsValid(value);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void OptionalGuid_ValidGuid_ReturnsTrue()
    {
        // Arrange
        var attribute = new OptionalGuidAttribute();
        var value = Guid.NewGuid();

        // Act
        var result = attribute.IsValid(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OptionalGuid_WithContext_Null_ReturnsSuccess()
    {
        // Arrange
        var model = new TestModel { OptionalId = null };
        var context = new ValidationContext(model) { MemberName = nameof(TestModel.OptionalId) };
        var attribute = new OptionalGuidAttribute();

        // Act
        var result = attribute.GetValidationResult(null, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void OptionalGuid_WithContext_GuidEmpty_ReturnsValidationError()
    {
        // Arrange
        var model = new TestModel { OptionalId = Guid.Empty };
        var context = new ValidationContext(model) { MemberName = nameof(TestModel.OptionalId) };
        var attribute = new OptionalGuidAttribute();

        // Act
        var result = attribute.GetValidationResult(Guid.Empty, context);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(ValidationResult.Success, result);
    }

    [Fact]
    public void OptionalGuid_WithContext_ValidGuid_ReturnsSuccess()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var model = new TestModel { OptionalId = validGuid };
        var context = new ValidationContext(model) { MemberName = nameof(TestModel.OptionalId) };
        var attribute = new OptionalGuidAttribute();

        // Act
        var result = attribute.GetValidationResult(validGuid, context);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    private class TestModel
    {
        public Guid? OptionalId { get; set; }
    }
}
