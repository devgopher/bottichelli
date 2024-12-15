// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

using System.Text.Json.Serialization;

namespace Botticelli.AI.DeepSeekGpt.Message.DeepSeek;

public class Choice
{
    [JsonPropertyName("finish_reason")] public string FinishReason { get; set; }

    [JsonPropertyName("index")] public int Index { get; set; }

    [JsonPropertyName("logprobs")] public object Logprobs { get; set; }

    [JsonPropertyName("message")] public DeepSeekInnerOutputMessage DeepSeekMessage { get; set; }
}

public class DeepSeekInnerOutputMessage
{
    [JsonPropertyName("content")] public string Content { get; set; }

    [JsonPropertyName("role")] public string Role { get; set; }

    [JsonPropertyName("function_call")] public object FunctionCall { get; set; }

    [JsonPropertyName("tool_calls")] public object ToolCalls { get; set; }
}

public class DeepSeekOutputMessage
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("choices")] public List<Choice> Choices { get; set; }

    [JsonPropertyName("created")] public int Created { get; set; }

    [JsonPropertyName("model")] public string Model { get; set; }

    [JsonPropertyName("object")] public string Object { get; set; }

    [JsonPropertyName("system_fingerprint")]
    public object SystemFingerprint { get; set; }

    [JsonPropertyName("usage")] public Usage Usage { get; set; }
}

public class Usage
{
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("prompt_tokens")] public int PromptTokens { get; set; }

    [JsonPropertyName("total_tokens")] public int TotalTokens { get; set; }
}