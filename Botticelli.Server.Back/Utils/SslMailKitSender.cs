using System.Text;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using FluentEmail.MailKitSmtp;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Botticelli.Server.Back.Utils;

/// <summary>
///     Send emails with the MailKit Library.
/// </summary>
public class SslMailKitSender : ISender
{
    private readonly IOptionsMonitor<SmtpClientOptions> _smtpClientOptions;

    /// <summary>
    ///     Creates a sender that uses the given SmtpClientOptions when sending with MailKit. Since the client is internal this
    ///     will dispose of the client.
    /// </summary>
    /// <param name="smtpClientOptions">The SmtpClientOptions to use to create the MailKit client</param>
    public SslMailKitSender(IOptionsMonitor<SmtpClientOptions> smtpClientOptions)
    {
        _smtpClientOptions = smtpClientOptions;
    }

    /// <summary>
    ///     Send the specified email.
    /// </summary>
    /// <returns>A response with any errors and a success boolean.</returns>
    /// <param name="email">Email.</param>
    /// <param name="token">Cancellation Token.</param>
    public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
    {
        var response = new SendResponse();
        var message = CreateMailMessage(email);

        if (token?.IsCancellationRequested ?? false)
        {
            response.ErrorMessages.Add("Message was cancelled by cancellation token.");
            return response;
        }

        try
        {
            if (_smtpClientOptions.CurrentValue.UsePickupDirectory)
            {
                SaveToPickupDirectory(message, _smtpClientOptions.CurrentValue.MailPickupDirectory).Wait();
                return response;
            }


            using var client = new SmtpClient();
            SslCertCheckOff(client);

            if (_smtpClientOptions.CurrentValue.SocketOptions.HasValue)
                client.Connect(
                    _smtpClientOptions.CurrentValue.Server,
                    _smtpClientOptions.CurrentValue.Port,
                    _smtpClientOptions.CurrentValue.SocketOptions.Value,
                    token.GetValueOrDefault());
            else
                client.Connect(
                    _smtpClientOptions.CurrentValue.Server,
                    _smtpClientOptions.CurrentValue.Port,
                    _smtpClientOptions.CurrentValue.UseSsl,
                    token.GetValueOrDefault());

            // Note: only needed if the SMTP server requires authentication
            if (_smtpClientOptions.CurrentValue.RequiresAuthentication)
                client.Authenticate(_smtpClientOptions.CurrentValue.User, _smtpClientOptions.CurrentValue.Password,
                    token.GetValueOrDefault());

            client.Send(message, token.GetValueOrDefault());
            client.Disconnect(true, token.GetValueOrDefault());
        }
        catch (Exception ex)
        {
            response.ErrorMessages.Add(ex.Message);
        }

        return response;
    }

    /// <summary>
    ///     Send the specified email.
    /// </summary>
    /// <returns>A response with any errors and a success boolean.</returns>
    /// <param name="email">Email.</param>
    /// <param name="token">Cancellation Token.</param>
    public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
    {
        var response = new SendResponse();
        var message = CreateMailMessage(email);

        if (token?.IsCancellationRequested ?? false)
        {
            response.ErrorMessages.Add("Message was cancelled by cancellation token.");
            return response;
        }

        try
        {
            if (_smtpClientOptions.CurrentValue.UsePickupDirectory)
            {
                await SaveToPickupDirectory(message, _smtpClientOptions.CurrentValue.MailPickupDirectory);
                return response;
            }

            using var client = new SmtpClient();
            SslCertCheckOff(client);
            if (_smtpClientOptions.CurrentValue.SocketOptions.HasValue)
                await client.ConnectAsync(
                    _smtpClientOptions.CurrentValue.Server,
                    _smtpClientOptions.CurrentValue.Port,
                    _smtpClientOptions.CurrentValue.SocketOptions.Value,
                    token.GetValueOrDefault());
            else
                await client.ConnectAsync(
                    _smtpClientOptions.CurrentValue.Server,
                    _smtpClientOptions.CurrentValue.Port,
                    _smtpClientOptions.CurrentValue.UseSsl,
                    token.GetValueOrDefault());

            // Note: only needed if the SMTP server requires authentication
            if (_smtpClientOptions.CurrentValue.RequiresAuthentication)
                await client.AuthenticateAsync(_smtpClientOptions.CurrentValue.User,
                    _smtpClientOptions.CurrentValue.Password, token.GetValueOrDefault());

            await client.SendAsync(message, token.GetValueOrDefault());
            await client.DisconnectAsync(true, token.GetValueOrDefault());
        }
        catch (Exception ex)
        {
            response.ErrorMessages.Add(ex.Message);
        }

        return response;
    }

    private static void SslCertCheckOff(SmtpClient client)
    {
        // Temporary!
        client.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
    }

    /// <summary>
    ///     Saves email to a pickup directory.
    /// </summary>
    /// <param name="message">Message to save for pickup.</param>
    /// <param name="pickupDirectory">Pickup directory.</param>
    private async Task SaveToPickupDirectory(MimeMessage message, string pickupDirectory)
    {
        // Note: this will require that you know where the specified pickup directory is.
        var path = Path.Combine(pickupDirectory, $"{Guid.NewGuid()}.eml");

        if (File.Exists(path))
            return;

        await using var stream = new FileStream(path, FileMode.CreateNew);
        await message.WriteToAsync(stream);
    }

    /// <summary>
    ///     Create a MimMessage so MailKit can send it
    /// </summary>
    /// <returns>The mail message.</returns>
    /// <param name="email">Email data.</param>
    private MimeMessage CreateMailMessage(IFluentEmail email)
    {
        var data = email.Data;

        var message = new MimeMessage();

        // fixes https://github.com/lukencode/FluentEmail/issues/228
        // if for any reason, subject header is not added, add it else update it.
        if (!message.Headers.Contains(HeaderId.Subject))
            message.Headers.Add(HeaderId.Subject, Encoding.UTF8, data.Subject ?? string.Empty);
        else
            message.Headers[HeaderId.Subject] = data.Subject ?? string.Empty;

        message.Headers.Add(HeaderId.Encoding, Encoding.UTF8.EncodingName);

        message.From.Add(new MailboxAddress(data.FromAddress.Name, data.FromAddress.EmailAddress));

        var builder = new BodyBuilder();
        if (!string.IsNullOrEmpty(data.PlaintextAlternativeBody))
        {
            builder.TextBody = data.PlaintextAlternativeBody;
            builder.HtmlBody = data.Body;
        }
        else if (!data.IsHtml)
        {
            builder.TextBody = data.Body;
        }
        else
        {
            builder.HtmlBody = data.Body;
        }

        data.Attachments.ForEach(x =>
        {
            var attachment = builder.Attachments.Add(x.Filename, x.Data, ContentType.Parse(x.ContentType));
            attachment.ContentId = x.ContentId;
        });


        message.Body = builder.ToMessageBody();

        foreach (var header in data.Headers) message.Headers.Add(header.Key, header.Value);

        data.ToAddresses.ForEach(x => { message.To.Add(new MailboxAddress(x.Name, x.EmailAddress)); });

        data.CcAddresses.ForEach(x => { message.Cc.Add(new MailboxAddress(x.Name, x.EmailAddress)); });

        data.BccAddresses.ForEach(x => { message.Bcc.Add(new MailboxAddress(x.Name, x.EmailAddress)); });

        data.ReplyToAddresses.ForEach(x => { message.ReplyTo.Add(new MailboxAddress(x.Name, x.EmailAddress)); });

        message.Priority = data.Priority switch
        {
            Priority.Low => MessagePriority.NonUrgent,
            Priority.Normal => MessagePriority.Normal,
            Priority.High => MessagePriority.Urgent,
            _ => message.Priority
        };

        return message;
    }
}