using Microsoft.Extensions.Compliance.Redaction;

namespace DataProtection;

public class StarRedactor : Redactor
{
    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        destination.Fill('*');
        return destination.Length;
    }

    public override int GetRedactedLength(ReadOnlySpan<char> input)
    {
        return input.Length;
    }
}
