using System.ComponentModel.DataAnnotations;

namespace Botticelli.Shared.API.Admin.Requests;

public class RegisterRequest
{
    public string? Email { get; set; }

    [DataType(DataType.Password)] public string? Password { get; set; }
}