namespace Kassa.RxUI;

public record struct OkMessage(string Message, /* TODO Should replace with enum */ string Icon, string Description = "")
{
    public static implicit operator (string, string, string)(OkMessage value)
    {
        return (value.Message, value.Icon, value.Description);
    }

    public static implicit operator OkMessage((string, string, string) value)
    {
        return new OkMessage(value.Item1, value.Item2, value.Item3);
    }
}