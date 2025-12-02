# Features Directory

This directory contains feature-based organization of Blazor components, with each subdirectory representing a specific business domain.

## Structure

-   **Properties/** - Property management components (list, create, edit, view)
-   **Tenants/** - Tenant management components (list, create, edit, view)
-   **Leases/** - Lease management components (list, create, edit, view)
-   **Invoices/** - Invoice management components (list, create, edit, view)
-   **Payments/** - Payment management components (list, create, edit, view)
-   **Documents/** - Document management components (list, upload, edit, view)
-   **Administration/** - Admin-only components (user management, system admin)

## Naming Convention

Each feature directory contains:

-   `{Entity}.razor` - Main list/index page (e.g., Properties.razor)
-   `Create{Entity}.razor` - Create new entity page
-   `Edit{Entity}.razor` - Edit existing entity page
-   `View{Entity}.razor` - View entity details page

## Benefits

1. **Feature Isolation** - Related components grouped together
2. **Better Navigation** - Easy to find components by business domain
3. **Maintainability** - Changes to one feature contained in one directory
4. **Team Collaboration** - Developers can work on features independently
5. **Scalability** - Easy to add new features without cluttering
