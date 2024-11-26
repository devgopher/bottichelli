using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Botticelli.Framework.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Security;

public static class Certificates
{
    public static IHttpClientBuilder AddServerCertificates(this IHttpClientBuilder builder, BotSettings? settings) =>
        builder.ConfigurePrimaryHttpMessageHandler(() =>
        {
            if (settings?.SecuritySettings?.DisableSecurity is true)
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (_, _, _, policyErrors) => true
                };

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var certificate = store.Certificates
                .FirstOrDefault(c => c.FriendlyName == settings!.SecuritySettings?.BotCertificateName);

            if (certificate == null) throw new NullReferenceException("Can't find a client certificate!");

            return new HttpClientHandler
            {
                ClientCertificates = { certificate },
                ServerCertificateCustomValidationCallback =
                    (_, _, _, policyErrors) =>
                    {
#if DEBUG
                        return true;
#endif
                        if (settings.SecuritySettings?.AllowSelfSignedServerCertificate is true)
                            return true;

                        return policyErrors == SslPolicyErrors.None;
                    }
            };
        });

}