using Micro.API.AsyncApi;
using Saunter.Attributes;
using VideoHub.Services.Search.Api.Events.External;

namespace VideoHub.Services.Search.Api;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(signed_up), BindingsRef = "users")]
    [PublishOperation(typeof(SignedUp), Summary = "User has been created", OperationId = nameof(signed_up))]
    internal abstract void signed_up();
    
    [Channel(nameof(video_rendered), BindingsRef = "videos")]
    [PublishOperation(typeof(VideoRendered), Summary = "Video has been rendered", OperationId = nameof(video_rendered))]
    internal abstract void video_rendered();
    
    [Channel(nameof(video_deleted), BindingsRef = "videos")]
    [PublishOperation(typeof(VideoDeleted), Summary = "Video has been deleted", OperationId = nameof(video_deleted))]
    internal abstract void video_deleted();
}