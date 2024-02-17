namespace MessagingContracts;

public record MovieUpdated(
    Guid Id,
    string Title,
    string Slug,
    int YearOfRelease,
    List<string> Genres);
