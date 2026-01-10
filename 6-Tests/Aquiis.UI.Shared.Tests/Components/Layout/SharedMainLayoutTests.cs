using Aquiis.UI.Shared.Components.Layout;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Aquiis.UI.Shared.Tests.Components.Layout;

/// <summary>
/// Tests for SharedMainLayout component.
/// Note: SharedMainLayout uses AuthorizeView, so these tests add test authorization context.
/// </summary>
public class SharedMainLayoutTests : TestContext
{
    public SharedMainLayoutTests()
    {
        // Add a test authorization state provider
        var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        var authStateProvider = new TestAuthStateProvider(authState);
        Services.AddSingleton<AuthenticationStateProvider>(authStateProvider);
        Services.AddSingleton<IAuthorizationService>(new TestAuthorizationService());
        Services.AddSingleton<IAuthorizationPolicyProvider>(new TestAuthorizationPolicyProvider());
    }
    
    private class TestAuthStateProvider : AuthenticationStateProvider
    {
        private readonly Task<AuthenticationState> _authState;
        public TestAuthStateProvider(Task<AuthenticationState> authState) => _authState = authState;
        public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authState;
    }
    
    private class TestAuthorizationService : IAuthorizationService
    {
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
            => Task.FromResult(AuthorizationResult.Success());
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
            => Task.FromResult(AuthorizationResult.Success());
    }
    
    private class TestAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => Task.FromResult(new AuthorizationPolicy(new[] { new TestRequirement() }, new string[0]));
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => Task.FromResult<AuthorizationPolicy?>(null);
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) => Task.FromResult<AuthorizationPolicy?>(null);
        
        private class TestRequirement : IAuthorizationRequirement { }
    }
    
    // Helper method to render SharedMainLayout with cascading authentication state
    private IRenderedComponent<SharedMainLayout> RenderLayoutWithAuth(Action<ComponentParameterCollectionBuilder<SharedMainLayout>> parameters)
    {
        return Render<CascadingAuthenticationState>(cascadingParams =>
        {
            cascadingParams.AddChildContent<SharedMainLayout>(parameters);
        }).FindComponent<SharedMainLayout>();
    }
    
    [Fact]
    public void SharedMainLayout_Renders_ChildContent()
    {
        // Arrange & Act
        var cut = RenderLayoutWithAuth(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Main content")))
        );

        // Assert
        cut.Markup.Should().Contain("Main content");
        cut.Markup.Should().Contain("content px-4");
    }

    [Fact]
    public void SharedMainLayout_Renders_SidebarContent()
    {
        // Arrange & Act
        var cut = RenderLayoutWithAuth(parameters => parameters
            .Add(p => p.SidebarContent, (RenderFragment)(builder => builder.AddContent(0, "Sidebar content")))
        );

        // Assert
        cut.Markup.Should().Contain("Sidebar content");
        cut.Markup.Should().Contain("sidebar");
    }

    [Fact]
    public void SharedMainLayout_Renders_AuthorizedHeaderContent()
    {
        // Arrange & Act
        var cut = RenderLayoutWithAuth(parameters => parameters
            .Add(p => p.AuthorizedHeaderContent, (RenderFragment)(builder => builder.AddContent(0, "Authorized Header")))
        );

        // Assert
        cut.Markup.Should().Contain("top-row px-4");
    }

    [Fact]
    public void SharedMainLayout_Renders_NotAuthorizedContent()
    {
        // Arrange & Act
        var cut = RenderLayoutWithAuth(parameters => parameters
            .Add(p => p.NotAuthorizedContent, (RenderFragment)(builder => builder.AddContent(0, "Not Authorized")))
        );

        // Assert
        cut.Markup.Should().Contain("top-row px-4");
    }

    [Fact]
    public void SharedMainLayout_Renders_FooterContent()
    {
        // Arrange & Act
        var cut = RenderLayoutWithAuth(parameters => parameters
            .Add(p => p.FooterContent, (RenderFragment)(builder => builder.AddContent(0, "Footer content")))
        );

        // Assert
        cut.Markup.Should().Contain("Footer content");
    }

    [Fact]
    public void SharedMainLayout_Has_Theme_Attribute()
    {
        // Arrange & Act
        var cut = RenderLayoutWithAuth(parameters => parameters
            .Add(p => p.Theme, "dark")
        );

        // Assert
        cut.Markup.Should().Contain("data-theme=\"dark\"");
    }

    [Fact]
    public void SharedMainLayout_Has_Error_UI()
    {
        // Arrange & Act
        var cut = RenderLayoutWithAuth(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Content")))
        );

        // Assert
        cut.Markup.Should().Contain("blazor-error-ui");
        cut.Markup.Should().Contain("An unhandled error has occurred.");
    }

    [Fact]
    public void SharedMainLayout_Minimal_Configuration()
    {
        // Arrange & Act
        var cut = RenderLayoutWithAuth(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Minimal")))
        );

        // Assert
        cut.Markup.Should().Contain("Minimal");
        cut.Markup.Should().Contain("page");
    }
}
