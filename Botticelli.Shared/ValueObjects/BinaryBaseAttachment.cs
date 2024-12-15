using System.Text.Json.Serialization;
using Botticelli.Shared.Constants;

namespace Botticelli.Shared.ValueObjects;

public class BinaryBaseAttachment(
    string uid,
    string name,
    MediaType mediaType,
    string url,
    byte[] data)
    : BaseAttachment(uid)
{
    public string Url { get; } = url;
    public MediaType MediaType { get; } = mediaType;
    public byte[] Data { get; } = data;
    public virtual string Name { get; } = name;
}