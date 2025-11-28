# UserContextService Usage Examples

The `UserContextService` provides cached access to the current user's OrganizationId and other user context information throughout your Blazor application.

## Benefits

1. **Performance**: Database queried only once per user session (Blazor circuit)
2. **Simplicity**: Single line to get OrganizationId instead of 5-10 lines
3. **Consistency**: Centralized user context access
4. **Type Safety**: Strongly typed properties
5. **Scoped Lifetime**: Automatically manages lifecycle with Blazor circuit

## Basic Usage

### Inject the Service

```razor
@inject UserContextService UserContext
```

### Get OrganizationId

```csharp
protected override async Task OnInitializedAsync()
{
    var organizationId = await UserContext.GetOrganizationIdAsync();

    // Use organizationId in your queries
    if (!string.IsNullOrEmpty(organizationId))
    {
        var properties = await dbContext.Properties
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync();
    }
}
```

## Common Scenarios

### 1. Filter Data by Organization

**Before (Old Way):**

```csharp
var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
var userId = authState.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

if (string.IsNullOrEmpty(userId))
{
    errorMessage = "User not authenticated.";
    return;
}

var currentUser = await UserManager.FindByIdAsync(userId);
if (currentUser == null)
{
    errorMessage = "Current user not found.";
    return;
}

var organizationId = currentUser.OrganizationId;
var tenants = await dbContext.Tenants
    .Where(t => t.OrganizationId == organizationId)
    .ToListAsync();
```

**After (New Way):**

```csharp
var organizationId = await UserContext.GetOrganizationIdAsync();
if (string.IsNullOrEmpty(organizationId))
{
    errorMessage = "User not authenticated.";
    return;
}

var tenants = await dbContext.Tenants
    .Where(t => t.OrganizationId == organizationId)
    .ToListAsync();
```

### 2. Check User Authentication

```csharp
if (!await UserContext.IsAuthenticatedAsync())
{
    Navigation.NavigateTo("/Account/Login");
    return;
}
```

### 3. Get Current User Information

```csharp
var user = await UserContext.GetCurrentUserAsync();
var email = await UserContext.GetUserEmailAsync();
var fullName = await UserContext.GetUserNameAsync();
var userId = await UserContext.GetUserIdAsync();
```

### 4. Check User Role

```csharp
if (await UserContext.IsInRoleAsync("Administrator"))
{
    // Show admin features
}
```

### 5. Creating New Records with OrganizationId

```csharp
private async Task CreateProperty()
{
    var organizationId = await UserContext.GetOrganizationIdAsync();

    var property = new Property
    {
        OrganizationId = organizationId,
        Address = model.Address,
        // ... other fields
    };

    await dbContext.Properties.AddAsync(property);
    await dbContext.SaveChangesAsync();
}
```

### 6. Refresh User Context (After User Update)

```csharp
// After updating user profile
await UserManager.UpdateAsync(user);

// Refresh the cached user context
await UserContext.RefreshAsync();
```

## Complete Component Example

```razor
@page "/tenants"
@inject UserContextService UserContext
@inject ApplicationDbContext DbContext
@rendermode InteractiveServer

<h3>My Tenants</h3>

@if (isLoading)
{
    <p>Loading...</p>
}
else if (tenants.Any())
{
    <table class="table">
        @foreach (var tenant in tenants)
        {
            <tr>
                <td>@tenant.FirstName @tenant.LastName</td>
                <td>@tenant.Email</td>
            </tr>
        }
    </table>
}

@code {
    private List<Tenant> tenants = new();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        // Simple one-liner to get OrganizationId
        var organizationId = await UserContext.GetOrganizationIdAsync();

        if (!string.IsNullOrEmpty(organizationId))
        {
            tenants = await DbContext.Tenants
                .Where(t => t.OrganizationId == organizationId)
                .OrderBy(t => t.LastName)
                .ToListAsync();
        }

        isLoading = false;
    }
}
```

## PropertyManagementService Integration

You can also inject `UserContextService` into your services:

```csharp
public class PropertyManagementService
{
    private readonly ApplicationDbContext _context;
    private readonly UserContextService _userContext;

    public PropertyManagementService(
        ApplicationDbContext context,
        UserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<List<Property>> GetPropertiesAsync()
    {
        var organizationId = await _userContext.GetOrganizationIdAsync();

        return await _context.Properties
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync();
    }
}
```

## Performance Notes

- The service is **scoped** to the Blazor circuit (user session)
- User data is loaded **only once** on first access
- Subsequent calls return **cached data** (no database queries)
- Call `RefreshAsync()` if user data changes during the session
- Service automatically cleans up when circuit disconnects

## Migration Guide

To update existing code:

1. Add injection: `@inject UserContextService UserContext`
2. Replace authentication state code with: `var organizationId = await UserContext.GetOrganizationIdAsync();`
3. Remove unused injections: `AuthenticationStateProvider`, `UserManager` (if only used for getting user)
4. Test the page to ensure functionality remains the same

## Error Handling

```csharp
var organizationId = await UserContext.GetOrganizationIdAsync();

if (string.IsNullOrEmpty(organizationId))
{
    // User not authenticated or no organization assigned
    errorMessage = "Unable to determine your organization. Please contact support.";
    return;
}

// Proceed with organization-scoped operations
```
