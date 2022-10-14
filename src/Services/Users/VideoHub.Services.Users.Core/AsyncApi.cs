using Micro.API.AsyncApi;
using Saunter.Attributes;
using VideoHub.Services.Users.Core.Events;

namespace VideoHub.Services.Users.Core;

internal abstract class AsyncApi : IAsyncApi
{
    [Channel(nameof(signed_in), BindingsRef = "users")]
    [SubscribeOperation(typeof(SignedIn), Summary = "User has been authenticated", OperationId = nameof(signed_in))]
    internal abstract void signed_in();
    
    [Channel(nameof(user_subscription_updated), BindingsRef = "users")]
    [SubscribeOperation(typeof(UserSubscriptionUpdated), Summary = "User subscription has been updated", OperationId = nameof(user_subscription_updated))]
    internal abstract void user_subscription_updated();
}