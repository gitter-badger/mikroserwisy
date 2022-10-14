using Micro.Abstractions;

namespace VideoHub.Services.Videos.Core.Commands;

public sealed record UploadVideo(long VideoId, long UserId, string Title) : ICommand;