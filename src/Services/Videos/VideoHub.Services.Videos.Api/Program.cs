using Micro.Auth;
using Micro.Framework;
using Micro.Handlers;
using VideoHub.Services.Videos.Core;
using VideoHub.Services.Videos.Core.Commands;
using VideoHub.Services.Videos.Core.Queries;
using VideoHub.Services.Videos.Infrastructure;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Services
    .AddCore(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapGet("/videos/{videoId:long}", async (long videoId, IDispatcher dispatcher) =>
{
    var video = await dispatcher.QueryAsync(new GetVideo(videoId));
    return video is null ? Results.NotFound() : Results.Ok(video);
}).WithTags("Videos").WithName("Get video");

app.MapGet("/videos", async (string? title, long? userId, IDispatcher dispatcher)
        => Results.Ok(await dispatcher.QueryAsync(new GetVideos(title, userId))))
    .WithTags("Videos").WithName("Get videos");

app.MapDelete("/videos/{videoId:long}", async (long videoId, HttpContext context, IDispatcher dispatcher) =>
{
    await dispatcher.SendAsync(new DeleteVideo(videoId, context.UserId()));
    return Results.NoContent();
}).RequireAuthorization().WithTags("Videos").WithName("Delete video");

app.UseMicroFramework();

app.Run();
