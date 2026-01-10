using Aquiis.Application.Services;
using Aquiis.Application.Services.Workflows;
using Aquiis.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Aquiis.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Register Application layer services and Infrastructure internally.
    /// This is the ONLY method products should call for dependency registration.
    /// Note: IDatabaseService must be registered by the product layer since it requires
    /// the product-specific Identity context (e.g., SimpleStartDbContext).
    /// </summary>
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        string connectionString)
    {
        // Call Infrastructure registration internally
        services.AddInfrastructure(connectionString);
        
        // Register all Application services
        services.AddScoped<AccountWorkflowService>();
        services.AddScoped<ApplicationService>();
        services.AddScoped<ApplicationWorkflowService>();
        services.AddScoped<CalendarEventService>();
        services.AddScoped<CalendarSettingsService>();
        services.AddScoped<ChecklistService>();
        services.AddScoped<DocumentService>();
        services.AddScoped<EmailService>();
        services.AddScoped<EmailSettingsService>();
        services.AddScoped<FinancialReportService>();
        services.AddScoped<InspectionService>();
        services.AddScoped<InvoiceService>();
        services.AddScoped<LeaseOfferService>();
        services.AddScoped<LeaseService>();
        services.AddScoped<LeaseWorkflowService>();
        services.AddScoped<MaintenanceService>();
        services.AddScoped<NoteService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<OrganizationService>();
        services.AddScoped<PaymentService>();
        services.AddScoped<PropertyManagementService>();
        services.AddScoped<PropertyService>();
        services.AddScoped<ProspectiveTenantService>();
        services.AddScoped<RentalApplicationService>();
        services.AddScoped<ScheduledTaskService>();
        services.AddScoped<SchemaValidationService>();
        services.AddScoped<ScreeningService>();
        services.AddScoped<SecurityDepositService>();
        services.AddScoped<SMSService>();
        services.AddScoped<SMSSettingsService>();
        services.AddScoped<TenantConversionService>();
        services.AddScoped<TenantService>();
        services.AddScoped<TourService>();
        
        return services;
    }
}
