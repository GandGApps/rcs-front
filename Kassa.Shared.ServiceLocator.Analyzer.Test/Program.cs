// See https://aka.ms/new-console-template for more information

using Kassa.Shared.Locator;

Console.WriteLine("Hello, World!");

RcsLocatorBuilder.AddSingleton<IA, A>();
RcsLocatorBuilder.AddSingleton<IB, B>();

public interface IA
{
    string Name { get; }
}

public class A: IA
{
    public string Name => "as";
}

public interface IB
{
    public int Value { get; set; }
}

public class B: IB
{
    public int Value { get; set; }

    public B(IA a)
    {
    }
}