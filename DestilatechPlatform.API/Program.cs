using DestilatechPlatform.API.AlertsNotifications.Application.ACL;
using DestilatechPlatform.API.AlertsNotifications.Application.Internal.CommandServices;
using DestilatechPlatform.API.AlertsNotifications.Application.Internal.QueryServices;
using DestilatechPlatform.API.AlertsNotifications.Application.Services;
using DestilatechPlatform.API.AlertsNotifications.Domain.Repositories;
using DestilatechPlatform.API.AlertsNotifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using DestilatechPlatform.API.AlertsNotifications.Interfaces.ACL;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.CommandServices;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.OutboundServices.ACL;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Internal.QueryServices;
using DestilatechPlatform.API.AnalyticsEstimations.Application.Services;
using DestilatechPlatform.API.AnalyticsEstimations.Domain.Repositories;
using DestilatechPlatform.API.AnalyticsEstimations.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using DestilatechPlatform.API.OrdersReplenishment.Application.ACL;
using DestilatechPlatform.API.OrdersReplenishment.Application.Internal.CommandServices;
using DestilatechPlatform.API.OrdersReplenishment.Application.Internal.OutboundServices;
using DestilatechPlatform.API.OrdersReplenishment.Application.Internal.OutboundServices.ACL;
using DestilatechPlatform.API.OrdersReplenishment.Application.Internal.QueryServices;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Repositories;
using DestilatechPlatform.API.OrdersReplenishment.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.ACL;
using DestilatechPlatform.API.Resources;
using DestilatechPlatform.API.Shared.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Mediator.Cortex.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Pipeline.Middleware.Components;
using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Localization Configuration
builder.Services.AddLocalization();

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = false);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization(options =>
        options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource)))
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var localizer = context.HttpContext.RequestServices
                .GetRequiredService<IStringLocalizer<SharedResource>>();
            var problemDetailsFactory = context.HttpContext.RequestServices
                .GetRequiredService<ProblemDetailsFactory>();
            var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                context.HttpContext,
                context.ModelState,
                statusCode: StatusCodes.Status400BadRequest,
                title: localizer["InvalidModelStateTitle"].Value);

            var result = new BadRequestObjectResult(problemDetails);
            result.ContentTypes.Add("application/problem+json");
            return result;
        };
    });

// Register RFC 7807 ProblemDetails payloads for centralized exception handling.
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        if (context.ProblemDetails.Status is null or >= 500)
        {
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
            context.ProblemDetails.Title ??= localizer["UnexpectedServerError"].Value;
            context.ProblemDetails.Detail ??= localizer["UnexpectedErrorProcessingRequest"].Value;
        }
    };
});

// Turn unhandled exceptions into localized ProblemDetails responses.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Localized ProblemDetails for failed application results (bounded context error enums).
builder.Services.AddSingleton<DestilatechPlatform.API.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory>();

// Generate the OpenAPI document with ASP.NET Core's built-in support.
builder.Services.AddOpenApi(options =>
{
    // Alerts & Notifications localized documentation
    options.AddOperationTransformer(
        DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform.LocalizedOpenApiAssembler
            .TransformOperationAsync);
    options.AddSchemaTransformer(
        DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform.LocalizedOpenApiAssembler
            .TransformSchemaAsync);
});

// Configure Database Context and route EF logs through the app logger pipeline.
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .AddInterceptors(serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// Configure Dependency Injection

// Shared Bounded Context Injection Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<DispatchDomainEventsInterceptor>();

// Alerts & Notifications Bounded Context Injection Configuration
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAlertCommandService, AlertCommandService>();
builder.Services.AddScoped<IAlertQueryService, AlertQueryService>();
builder.Services.AddScoped<IAlertsContextFacade, AlertsContextFacade>();

// Orders & Replenishment Bounded Context Injection Configuration
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IReplenishmentOrderRepository, ReplenishmentOrderRepository>();
builder.Services.AddScoped<ICustomerCommandService, CustomerCommandService>();
builder.Services.AddScoped<ICustomerQueryService, CustomerQueryService>();
builder.Services.AddScoped<IOrderCommandService, OrderCommandService>();
builder.Services.AddScoped<IOrderQueryService, OrderQueryService>();
builder.Services.AddScoped<IReplenishmentOrderCommandService, ReplenishmentOrderCommandService>();
builder.Services.AddScoped<IReplenishmentOrderQueryService, ReplenishmentOrderQueryService>();
builder.Services.AddScoped<IInventoryPort, ExternalInventoryService>();
builder.Services.AddScoped<IOrdersContextFacade, OrdersContextFacade>();

// Analytics & Estimations Bounded Context Injection Configuration
builder.Services.AddScoped<IReplenishmentEstimateRepository, ReplenishmentEstimateRepository>();
builder.Services.AddScoped<IReplenishmentEstimateCommandService, ReplenishmentEstimateCommandService>();
builder.Services.AddScoped<IReplenishmentEstimateQueryService, ReplenishmentEstimateQueryService>();
builder.Services.AddScoped<IHistoricalIndicatorQueryService, HistoricalIndicatorQueryService>();
builder.Services.AddScoped<IInventoryHistoryReadPort, ExternalInventoryHistoryService>();
builder.Services.AddScoped<ISalesHistoryReadPort, ExternalSalesHistoryService>();
builder.Services.AddScoped<IProductionHistoryReadPort, EmptyProductionHistoryService>();

// Mediator Configuration
// Command pipeline behaviors (logging, exception logging, unit of work commit)
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(ExceptionHandlingCommandBehavior<>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(TransactionCommandBehavior<>));

// Cortex Mediator publishes domain events to every IEventHandler found in this assembly
builder.Services.AddCortexMediator([typeof(Program)]);

var app = builder.Build();

// Apply pending migrations on startup (safe to call even when schema is up to date)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Localization must run before OpenAPI so generated documentation follows the request culture.
string[] supportedCultures = ["en", "en-US", "es", "es-PE"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

app.UseAuthorization();

// OpenAPI and Scalar are kept available in all environments to preserve the existing API explorer behavior.
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.Run();
