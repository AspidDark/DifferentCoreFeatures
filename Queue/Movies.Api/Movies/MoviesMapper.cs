

using MessagingContracts;

namespace Movies.Api.Movies;

public static class MoviesMapper
{
    public static MovieCreated MapToCreated(this Movie movie)
    {
        return new MovieCreated(
            movie.Id,
            movie.Title,
            movie.Slug,
            movie.YearOfRelease,
            movie.Genres);
    }
    
    public static MovieUpdated MapToUpdated(this Movie movie)
    {
        return new MovieUpdated(
            movie.Id,
            movie.Title,
            movie.Slug,
            movie.YearOfRelease,
            movie.Genres);
    }
    
    public static MovieDeleted MapToDeleted(this Movie movie)
    {
        return new MovieDeleted(
            movie.Id,
            movie.Title,
            movie.Slug,
            movie.YearOfRelease,
            movie.Genres);
    }
}
