using System.ComponentModel.DataAnnotations;
using System.Text;
using Botticelli.Server.Back.Services.Auth.Models;

namespace Botticelli.Server.Back.Services.Auth;

public class ConfirmationCodeGenerator : IConfirmationCodeGenerator
{
    private readonly Random _rand = new(DateTime.Now.Millisecond);

    public ConfirmationCode GenerateCode(int size = 4, TimeSpan lifetime = default)
    {
        var code = new StringBuilder(size);

        code.Append(Enumerable.Range(0, size).Select(x => _rand.Next(0, 9)));

        return new ConfirmationCode()
        {
            Code = code.ToString(),
            Lifetime = lifetime == default ? TimeSpan.FromMinutes(10) : lifetime
        };
    }

}