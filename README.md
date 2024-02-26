# My first sample with .NET8 GraphQL API

For more info about GraphQL see (youtube video):

GraphQL is a query language for APIs and a runtime for executing those queries by using a type system you define for your data

Unlike REST, which uses a fixed set of endpoints to fetch data, GraphQL allows clients to request exactly the data they need and to aggregate data from multiple sources with a single request

This can lead to more efficient network requests and a better development experience

## 1. Create a New .NET 8 project in VSCode 

```
dotnet new web -o GraphQLDemo
cd GraphQLDemo
```

This creates a new web project in a folder named GraphQLDemo and then changes into that directory

## 2. Add Hot Chocolate NuGet Packages

You need to add the Hot Chocolate ASP.NET Core package to your project

```
dotnet add package HotChocolate.AspNetCore
dotnet add package HotChocolate.AspNetCore.Playground
```

## 3. Project Structure Overview

We have to create the following folders and files in our project structure:

```css
GraphQLDemo/
│
├── Models/
│   ├── Author.cs
│   ├── Post.cs
│   └── DataStore.cs
│
├── GraphQL/
│   ├── AuthorType.cs
│   ├── PostType.cs
│   ├── Query.cs
│   └── Mutation.cs
│
├── Services/
│   ├── IAuthorService.cs
│   ├── IPostService.cs
│   ├── AuthorService.cs
│   └── PostService.cs
│
├── Program.cs
└── GraphQLDemo.csproj
```

See the project structure in VSCode

![image](https://github.com/luiscoco/GraphQL_dotNet8_sample1/assets/32194879/3a0a644e-7706-4600-9a74-8cf1fd35c616)


## 4. Create the Models and DataStore

### 4.1. Create the Author model

```csharp
namespace GraphQLDemo.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}
```

### 4.2. Create the Post model

```csharp
namespace GraphQLDemo.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}
```

### 4.3. Create the DataStore

```csharp
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
```

## 5. Create the Services

### 5.1. Create the AuthorService interface


```csharp
using GraphQLDemo.Models;

namespace GraphQLDemo.Services
{
    public interface IAuthorService
    {
        Author GetAuthorById(int id);
        List<Author> GetAllAuthors();
    }
}
```

### 5.2. Create the AuthorService


```csharp
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
```

### 5.3. Create the PostService interfaces



```csharp
using GraphQLDemo.Models;

namespace GraphQLDemo.Services
{
    public interface IPostService
    {
        Post GetPostById(int id);
        List<Post> GetAllPosts();
        Post AddPost(Post post);
    }
}
```

### 5.4. Create the PostService


```csharp
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
```



## 6. Create the Types 

### 6.1. AuthorType

```csharp
using GraphQLDemo.Models;
using HotChocolate.Types;

namespace GraphQLDemo.GraphQL
{
    public class AuthorType : ObjectType<Author>
    {
        protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
        {
            descriptor.Field(a => a.Id).Type<NonNullType<IntType>>();
            descriptor.Field(a => a.Name).Type<NonNullType<StringType>>();
            descriptor.Field(a => a.Posts).Type<NonNullType<ListType<NonNullType<PostType>>>>();
        }
    }
}

```

### 6.2. PostType

```csharp
using GraphQLDemo.Models;
using HotChocolate.Types;

namespace GraphQLDemo.GraphQL
{
    public class PostType : ObjectType<Post>
    {
        protected override void Configure(IObjectTypeDescriptor<Post> descriptor)
        {
            descriptor.Field(p => p.Id).Type<NonNullType<IntType>>();
            descriptor.Field(p => p.Title).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Content).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Author).Type<NonNullType<AuthorType>>();
        }
    }
}
```

### 6.3. Query

```csharp
using GraphQLDemo.Models;
using HotChocolate;
using GraphQLDemo.Services;

namespace GraphQLDemo.GraphQL
{
    public class Query
    {
        public Author GetAuthor([Service] IAuthorService authorService, int id) =>
            authorService.GetAuthorById(id);

        public Post GetPost([Service] IPostService postService, int id) =>
            postService.GetPostById(id);
    }
}
```

### 6.4. Mutation

```csharp
using GraphQLDemo.Models;
using HotChocolate;
using GraphQLDemo.Services;

namespace GraphQLDemo.GraphQL
{
    public class Mutation
    {
        public Post AddPost([Service] IPostService postService, CreatePostInput input) =>
            postService.AddPost(new Post
            {
                Title = input.Title,
                Content = input.Content,
                AuthorId = input.AuthorId
            });
    }

    public record CreatePostInput(string Title, string Content, int AuthorId);
}
```
