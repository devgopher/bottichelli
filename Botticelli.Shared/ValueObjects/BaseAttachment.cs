using System.Text.Json.Serialization;

namespace Botticelli.Shared.ValueObjects;

[JsonDerivedType(typeof(BaseAttachment), typeDiscriminator: "base")]
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
[JsonDerivedType(typeof(BinaryBaseAttachment), typeDiscriminator: "binaryAttachment")]
[JsonDerivedType(typeof(InvoiceBaseAttachment), typeDiscriminator: "invoiceAttachment")]
public class BaseAttachment(string? uid)
{
    public string? Uid { get; } = uid;
}