namespace Botticelli.AI.Message;

public class AiMessage : Shared.ValueObjects.Message
{
    public AiMessage()
    {
    }

    public AiMessage(string uid) : base(uid)
    {
    }

    public string Instruction { get; set; }

    public List<AiMessage> AdditionalMessages { get; set; }
}