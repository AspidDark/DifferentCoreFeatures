using FluentValidation;

namespace RuntimeFluentValidation
{
    public class ExampleValidation :  AbstractValidator<Options>
    {

        public ExampleValidation()
        {
            RuleFor(x => x.SomeValue).NotNull();
            RuleFor(x => x.IntData).GreaterThan(0);
        }
    }
}
