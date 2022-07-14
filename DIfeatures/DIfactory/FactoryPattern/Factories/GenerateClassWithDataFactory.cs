using FactoryPattern.Samples;

namespace FactoryPattern.Factories;

public static class GenerateClassWithDataFactoryExtension
{
    public static void AddGenericClassWithDataFactory(this IServiceCollection services)
    {
        services.AddTransient<IUserData, UserData>();
        services.AddSingleton<Func<IUserData>>(x => () => x.GetService<IUserData>()!);
        services.AddSingleton<IUserDataFactory, UserDataFactory>();
    }
}

public interface IUserDataFactory
{
    IUserData Create(string name);
}

public class UserDataFactory : IUserDataFactory
{
    private readonly Func<IUserData> _factory;

    public UserDataFactory(Func<IUserData> factory)
    {
        _factory = factory;
    }

    public IUserData Create(string name)
    {
        var output = _factory();
        output.Name = name;
        return output;
    }
}