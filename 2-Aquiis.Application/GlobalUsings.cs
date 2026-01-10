// Global usings for Application layer
// ApplicationUser removed - now product-specific
// Application layer uses ApplicationDbContext for business data only
global using ApplicationDbContext = Aquiis.Infrastructure.Data.ApplicationDbContext;
global using SendGridEmailService = Aquiis.Infrastructure.Core.Services.SendGridEmailService;
global using TwilioSMSService = Aquiis.Infrastructure.Core.Services.TwilioSMSService;
