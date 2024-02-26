using GraphQLDemo.Models;

namespace GraphQLDemo.Services
{
    public class PostService : IPostService
    {
        public Post GetPostById(int id)
        {
            return DataStore.Posts.First(p => p.Id == id);
        }

        public List<Post> GetAllPosts()
        {
            return DataStore.Posts;
        }

        public Post AddPost(Post post)
        {
            post.Id = DataStore.Posts.Any() ? DataStore.Posts.Max(p => p.Id) + 1 : 1;
            DataStore.Posts.Add(post);

            var author = DataStore.Authors.FirstOrDefault(a => a.Id == post.AuthorId);
            if (author != null)
            {
                if (author.Posts == null)
                {
                    author.Posts = new List<Post>();
                }
                author.Posts.Add(post);
                post.Author = author; // Ensure this line is present to correctly associate the post with its author
            }
            else
            {
                throw new Exception($"Author with ID {post.AuthorId} not found.");
            }

            return post;
        }
    }
}
