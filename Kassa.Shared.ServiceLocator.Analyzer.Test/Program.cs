// See https://aka.ms/new-console-template for more information

using Kassa.Shared;
using Kassa.Shared.ServiceLocator;

Console.WriteLine("Hello, World!");

RcsLocatorBuilder.AddSingleton<IA, A>();
RcsLocatorBuilder.AddScoped<IB, B>();
RcsLocatorBuilder.AddTransient<C>();
RcsLocatorBuilder.AddScoped<D>();

RcsLocatorBuilder.AddToBuilder();

ServiceLocatorBuilder.SetLocator();

await RcsLocator.ActivateScope();

var d = RcsLocator.GetRequiredService<C>();

Console.WriteLine(d.GetName());


static class Ac
{
    static int? a = 2;

    public static int? GetA()
    {
        return a ??= 1;
    }
}

public interface IA
{
    string Name { get; }
}

public class A: IA
{
    public string Name => "as";
}

public interface IB: IInitializable
{
    public int Value { get; set; }
}

public class B: EmptyInitializable, IB
{
    public int Value { get; set; }

    public B(IA a)
    {
    }
}

public class C
{
    public C(IA a, [ScopeInject] IB b)
    {
    }

    public string GetName() => "c";
}

public class D : EmptyInitializable
{
    private readonly IA _a;
    private readonly C _c;

    public D(IA a, [ScopeInject] C c)
    {
        _a = a;
    }

    public string GetName() => _a.Name + _c.GetName();
}