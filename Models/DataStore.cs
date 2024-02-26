using GraphQLDemo.Models;
using System.Collections.Generic;

namespace GraphQLDemo.Models
{
    public static class DataStore
    {
        public static List<Author> Authors { get; } = new List<Author>
        {
            new Author { Id = 1, Name = "Jane Austen", Posts = new List<Post>() },
            new Author { Id = 2, Name = "Charles Dickens", Posts = new List<Post>() }
        };

        public static List<Post> Posts { get; } = new List<Post>();

        static DataStore()
        {
            // Initialize with some posts
            var post1 = new Post
            {
                Id = 1,
                Title = "Exploring GraphQL",
                Content = "GraphQL offers a more efficient way to design web APIs.",
                AuthorId = 1,
                Author = Authors[0] // Linking the post to Jane Austen
            };

            var post2 = new Post
            {
                Id = 2,
                Title = "Advantages of GraphQL",
                Content = "One major advantage of GraphQL is it allows clients to request exactly what they need.",
                AuthorId = 1, // Assigning to the same author for simplicity
                Author = Authors[0] // Linking the post to Jane Austen
            };

            // Add the posts to the authors' posts collection
            Authors[0].Posts.Add(post1);
            Authors[0].Posts.Add(post2);

            // Add the posts to the global posts list
            Posts.Add(post1);
            Posts.Add(post2);
        }
    }
}
