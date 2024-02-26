namespace Kassa.RxUI;

public record struct OkMessage(string Message, string Icon)
{
    public static implicit operator (string, string)(OkMessage value)
    {
        return (value.Message, value.Icon);
    }

    public static implicit operator OkMessage((string, string) value)
    {
        return new OkMessage(value.Item1, value.Item2);
    }
}