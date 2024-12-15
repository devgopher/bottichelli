using System.Text.Json.Serialization;

namespace Botticelli.Shared.ValueObjects;

[JsonDerivedType(typeof(BaseAttachment), "base")]
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
[JsonDerivedType(typeof(BinaryBaseAttachment), "binaryAttachment")]
[JsonDerivedType(typeof(InvoiceBaseAttachment), "invoiceAttachment")]
public class BaseAttachment(string? uid)
{
    public string? Uid { get; } = uid;
}