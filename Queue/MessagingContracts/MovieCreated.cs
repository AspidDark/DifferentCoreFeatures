namespace MessagingContracts;

public record MovieCreated(
    Guid Id,
    string Title,
    string Slug,
    int YearOfRelease,
    List<string> Genres);
