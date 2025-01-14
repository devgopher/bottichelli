using Botticelli.Pay.Models;

namespace Botticelli.Pay.Message;

public class PayPreCheckoutMessage : Shared.ValueObjects.Message
{
    public required PreCheckoutQuery PreCheckoutQuery { get; set; }    
}