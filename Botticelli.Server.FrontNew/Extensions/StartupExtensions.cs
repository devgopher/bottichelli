using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Botticelli.Server.FrontNew.Extensions;

public static class StartupExtensions
{
    public static IHttpClientBuilder AddCertificates(this IHttpClientBuilder builder) =>
        builder.ConfigurePrimaryHttpMessageHandler(() =>
        {
#if DEBUG
            return null;
#endif

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var certificate = store.Certificates
                .FirstOrDefault(c => c.FriendlyName == "BotticelliBotsServerBack");

            if (certificate == null) throw new NullReferenceException("Can't find a client certificate!");

            return new HttpClientHandler
            {
                ClientCertificates = { certificate },
                // ClientCertificateOptions = ClientCertificateOption.Automatic,
                ServerCertificateCustomValidationCallback =
                    (httpRequestMessage,
                        cert,
                        cetChain,
                        policyErrors) =>
                    {
                        return policyErrors == SslPolicyErrors.None;
                        // TODO: cert checking
                    }
            };
        });
}