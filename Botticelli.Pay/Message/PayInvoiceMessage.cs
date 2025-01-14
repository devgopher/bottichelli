using Botticelli.Pay.Models;

namespace Botticelli.Pay.Message;

public class PayInvoiceMessage : Shared.ValueObjects.Message
{
    
    
    public Invoice? Invoice { get; set; }    
}