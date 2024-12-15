using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;

namespace Botticelli.Server.Back.Extensions;

public static class StartupExtensions
{
    public static void ApplyMigrations<TContext>(this WebApplicationBuilder webApplicationBuilder)
        where TContext : DbContext
    {
        using var scope = webApplicationBuilder.Services
            .BuildServiceProvider()
            .CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<TContext>();
        var pendingMigrations = db.Database.GetPendingMigrations();

        if (pendingMigrations.Any()) db.Database.Migrate();
    }

    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@";
            options.User.RequireUniqueEmail = true;
        });

        services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
        });

        return services;
    }

    public static IWebHostBuilder AddSsl(this IWebHostBuilder builder, IConfiguration config)
    {
        // in Linux put here: ~/.dotnet/corefx/cryptography/x509stores/
        if (!OperatingSystem.IsWindows()) return builder;

        var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        var port = int.Parse(config["ServerSettings:httpsPort"]!);
        var thumbprint = config["ServerSettings:thumbprint"]!;

        var certificate = store.Certificates
            .FirstOrDefault(c => c.FriendlyName == "BotticelliBotsServerBack" && c.Thumbprint == thumbprint);

        if (certificate == null) throw new KeyNotFoundException("Can't find SSL certificate!");

        return builder
            .UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback,
                    port,
                    listenOptions =>
                    {
                        var connectionOptions = new HttpsConnectionAdapterOptions
                        {
                            ServerCertificate = certificate,
                            ClientCertificateMode = ClientCertificateMode.AllowCertificate,
                            ClientCertificateValidation = (cert, chain, errors) =>
                            {
                                if (errors != SslPolicyErrors.None) return false;


                                return true;
                            }
                        };

                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        listenOptions.UseHttps(connectionOptions);
                    });
            });
    }
}