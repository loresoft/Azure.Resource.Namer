using System.Text.Json;
using System.Text.Json.Serialization;

using AzureNamer.Core.Options;
using AzureNamer.Shared;

using Blazone.Authentication;

using MediatR.CommandQuery.Endpoints;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace AzureNamer.Server;

public static class Program
{
    private const string OutputTemplate = "{Timestamp:HH:mm:ss.fff} [{Level:u1}] {Message:lj}{NewLine}{Exception}";

    public static int Main(string[] args)
    {
        string logDirectory = GetLoggingPath();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: OutputTemplate)
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting Web Host Version {version}", ThisAssembly.InformationalVersion);

            var builder = WebApplication.CreateBuilder(args);
            builder.Host
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Debug)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName)
                    .Enrich.WithProperty("ApplicationVersion", ThisAssembly.InformationalVersion)
                    .Enrich.WithProperty("EnvironmentName", builder.Environment.EnvironmentName)
                    .WriteTo.Console(outputTemplate: OutputTemplate)
                    .WriteTo.File(
                        formatter: new RenderedCompactJsonFormatter(),
                        path: $"{logDirectory}/log.clef",
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 10
                    ));

            ConfigureServices(builder);

            var app = builder.Build();
            ConfigureMiddleware(app);

            app.Run();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services
            .AddMicrosoftIdentityWebAppAuthentication(configuration);

        services
            .AddMemoryCache();

        services
            .AddAuthorization()
            .AddBlazoneServer()
            .AddAntiforgery();

        services
            .AddHttpContextAccessor();

        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        services
            .AddProblemDetails();

        services
            .AddAzureNamerShared()
            .AddAzureNamerCore()
            .AddAzureNamerServer();

        services
            .AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["image/svg+xml", "application/vnd.serilog.clef"]);
            });

        services
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.SerializerOptions.TypeInfoResolverChain.Add(DomainJsonContext.Default);
            });

        services
            .TryAddSingleton(sp => sp.GetRequiredService<IOptions<JsonOptions>>().Value.SerializerOptions);

        // change authentication cookie name
        builder.Services
            .Configure<CookieAuthenticationOptions>(
                name: CookieAuthenticationDefaults.AuthenticationScheme,
                configureOptions: options => options.Cookie.Name = ".AzureNamer.Authentication"
            );

        string logDirectory = GetLoggingPath();
        services.Configure<LoggingOptions>(options => options.Path = logDirectory);
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseWebAssemblyDebugging();
        else
            app.UseHsts();

        app.UseResponseCompression();

        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseBlazorFrameworkFiles();
        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapBlazoneEndpoints();
        app.MapFeatureEndpoints();

        // fallback for api endpoints
        app.MapFallback("api/{**slug}", () => Results.NotFound());

        app.MapFallbackToFile("index.html");
    }

    private static string GetLoggingPath()
    {
        // azure home directory
        var homeDirectory = Environment.GetEnvironmentVariable("HOME") ?? ".";
        var logDirectory = Path.Combine(homeDirectory, "LogFiles");

        return Path.GetFullPath(logDirectory);
    }
}
