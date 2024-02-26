using GraphQLDemo.Models;

namespace GraphQLDemo.Services
{
    public class AuthorService : IAuthorService
    {
        public Author GetAuthorById(int id)
        {
            return DataStore.Authors.First(a => a.Id == id);
        }

        public List<Author> GetAllAuthors()
        {
            return DataStore.Authors;
        }
    }
}
