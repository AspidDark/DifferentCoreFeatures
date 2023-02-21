using FluentValidation;
using Microsoft.Extensions.Options;

namespace RuntimeFluentValidation;

public static class OptionsBuilderFluentValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateFluently<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(s =>
        new FluentVaidationOptions<TOptions>(optionsBuilder.Name, s.GetRequiredService<IValidator<TOptions>>(), s.GetRequiredService<ILogger<FluentVaidationOptions<TOptions>>>()));
        return optionsBuilder;
    }
}

public class FluentVaidationOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
    public string? Name { get; }

    private readonly IValidator<TOptions> _validator;
    private readonly ILogger<FluentVaidationOptions<TOptions>> _logger;
    public FluentVaidationOptions(string? name, IValidator<TOptions> validator, ILogger<FluentVaidationOptions<TOptions>> logger)
    {
        Name = name;
        _validator = validator;
        _logger = logger;
    }

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        if (Name != null && Name != name)
        {
            return ValidateOptionsResult.Skip;
        }
        
        ArgumentNullException.ThrowIfNull(options);

        var validationResult = _validator.Validate(options);

        if (validationResult.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        var errors = validationResult.Errors.Select(x=> $"Property {x.PropertyName}  with error {x.ErrorMessage}");
        var errorMessage = errors.Aggregate((a, b) => a + " | " + b);
        _logger.LogError(errorMessage);

        return ValidateOptionsResult.Fail(errors);
       
    }
}