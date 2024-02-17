namespace MessagingContracts;

public record MovieDeleted(
    Guid Id,
    string Title,
    string Slug,
    int YearOfRelease,
    List<string> Genres);
