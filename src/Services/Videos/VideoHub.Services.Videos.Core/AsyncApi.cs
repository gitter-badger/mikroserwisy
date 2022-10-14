using Micro.API.AsyncApi;
using Saunter.Attributes;
using VideoHub.Services.Videos.Core.Events;

namespace VideoHub.Services.Videos.Core;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(video_deleted), BindingsRef = "videos")]
    [SubscribeOperation(typeof(VideoDeleted), Summary = "Video has been deleted", OperationId = nameof(video_deleted))]
    internal abstract void video_deleted();
    
    [Channel(nameof(video_render_progressed), BindingsRef = "videos")]
    [SubscribeOperation(typeof(VideoRendered), Summary = "Video has been rendered", OperationId = nameof(video_rendered))]
    internal abstract void video_rendered();
    
    [Channel(nameof(video_received), BindingsRef = "videos")]
    [SubscribeOperation(typeof(VideoReceived), Summary = "Video has been received", OperationId = nameof(video_received))]
    internal abstract void video_received();
    
    [Channel(nameof(VideoRenderProgressed), BindingsRef = "videos")]
    [SubscribeOperation(typeof(VideoRenderProgressed), Summary = "Video render has progressed", OperationId = nameof(video_render_progressed))]
    internal abstract void video_render_progressed();
}