namespace RuntimeFluentValidation
{
    public class Options
    {
        public const string sectionName = nameof(Options); 

        public required string SomeValue { get; init; }
        public required int IntData { get; set; }
    }
}
