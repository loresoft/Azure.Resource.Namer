using AzureNamer.Core.Options;

using MediatR.CommandQuery.Endpoints;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureNamer.Server.Endpoints;

[RegisterTransient<IFeatureEndpoint>(Duplicate = DuplicateStrategy.Append)]
public class LoggingEndpoints : IFeatureEndpoint
{
    private readonly IOptions<LoggingOptions> _options;

    public LoggingEndpoints(IOptions<LoggingOptions> options)
    {
        _options = options;
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/administrative/logging")
            .RequireAuthorization();

        MapGroup(group);
    }

    private void MapGroup(RouteGroupBuilder group)
    {
        group
            .MapGet("", GetLogFiles)
            .Produces<IReadOnlyCollection<string>>()
            .WithTags("Logging")
            .WithName($"GetLogFiles")
            .WithSummary("Get list of log files")
            .WithDescription("Get list of log files");

        group
            .MapGet("{file}", GetLogFile)
            .Produces(200, contentType: "application/vnd.serilog.clef")
            .WithTags("Logging")
            .WithName($"GetLogFile")
            .WithSummary("Get log file context")
            .WithDescription("Get log file context");
    }

    private IResult GetLogFile([FromRoute] string file)
    {
        var path = Path.Combine(_options.Value.Path, file) + ".clef";
        path = Path.GetFullPath(path);

        if (!File.Exists(path))
            return Results.NotFound("File Not found");

        var readStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        return Results.File(readStream, "application/vnd.serilog.clef");
    }

    private IResult GetLogFiles()
    {
        var results = Directory
            .EnumerateFiles(_options.Value.Path, _options.Value.Filter)
            .Select(Path.GetFileNameWithoutExtension)
            .OrderByDescending(s => s)
            .ToList();

        return Results.Ok(results);
    }
}
