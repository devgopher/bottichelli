using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;

namespace Botticelli.Interfaces;

public interface IEventBasedBotClientApi
{
    /// <summary>
    ///     Sends a message
    /// </summary>
    /// <param name="request">Message request</param>
    /// <param name="token">Cancellation token</param>
    /// <returns></returns>
    public Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request, CancellationToken token);

    /// <summary>
    ///     Sends a message
    /// </summary>
    /// <param name="request">Message request</param>
    /// <param name="optionsBuilder">Specific options for a particular messenger (for example, ReplyMarkup for Telegram)</param>
    /// <param name="token">Cancellation token</param>
    /// <returns></returns>
    public Task<SendMessageResponse> SendMessageAsync<TSendOptions>(SendMessageRequest request,
        ISendOptionsBuilder<TSendOptions> optionsBuilder,
        CancellationToken token)
        where TSendOptions : class;

    /// <summary>
    ///     Edits a message
    /// </summary>
    /// <param name="request">Message request</param>
    /// <param name="token">Cancellation token</param>
    /// <returns></returns>
    public Task<SendMessageResponse> UpdateMessageAsync(SendMessageRequest request, CancellationToken token);
    
    /// <summary>
    ///     Edits a message
    /// </summary>
    /// <param name="request">Message request</param>
    /// <param name="optionsBuilder">Specific options for a particular messenger (for example, ReplyMarkup for Telegram)</param>
    /// <param name="token">Cancellation token</param>
    /// <returns></returns>
    public Task<SendMessageResponse> UpdateMessageAsync<TSendOptions>(SendMessageRequest request,
                                                                      ISendOptionsBuilder<TSendOptions> optionsBuilder,
                                                                      CancellationToken token)
            where TSendOptions : class;
    
    public Task<RemoveMessageResponse> DeleteMessageAsync(RemoveMessageRequest request, CancellationToken token);
}