using PlayingWithRefit.Model;
using Refit;

namespace PlayingWithRefit.Refit;

public interface IJsonPlaceholderClient
{
    [Get("/Posts")]
    Task<IEnumerable<Post>> GetAllPostAsync();

    [Get("/Posts/{id}")]
    Task<Post> GetPostAsync(int id);

    [Post("/Posts")]
    Task<Post> CreatePostAsync([Body] Post post);

    [Put("/Posts/{id}")]
    Task<Post> UpdatePostAsync(int id, [Body] Post post);

    [Delete("/Posts/{id}")]
    Task DeletePostAsync(int id);
}
