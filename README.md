# My first sample with .NET8 GraphQL API

See also this example: https://medium.com/@jaydeepvpatil225/graphql-introduction-and-product-application-using-net-core-bd37faf3c585

See also this demo: https://github.com/Jaydeep-007/graphql_demo

For more info about GraphQL see (youtube video list): https://www.youtube.com/playlist?list=PL4cUxeGkcC9gUxtblNUahcsg0WLxmrK_y

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

This is the csproj file

```
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="HotChocolate.AspNetCore" Version="13.9.0" />
    <PackageReference Include="HotChocolate.AspNetCore.Playground" Version="10.5.5" />
  </ItemGroup>

</Project>
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

**Models and DataStore**

**Author Model**: Represents an author with an ID, name, and a list of posts they've written

**Post Model**: Represents a blog post with an ID, title, content, the ID of its author, and a reference back to the Author object

**DataStore**: A static class that acts as an in-memory database to store authors and posts. It's initialized with two authors (Jane Austen and Charles Dickens) and two posts linked to Jane Austen

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

**Services**

**IAuthorService & AuthorService**: Defines and implements functionality to retrieve an author by their ID and to get all authors. It interacts directly with the DataStore

**IPostService & PostService**: Defines and implements functionality to retrieve a post by its ID, get all posts, and add a new post to the DataStore. When adding a post, it ensures the post is linked to an existing author

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

**GraphQL Types, Query, and Mutation**

**AuthorType and PostType**: GraphQL object types that define how Author and Post entities are represented in your GraphQL schema

These types include fields like id, name, title, and content, and they specify the relationships between authors and posts

**Query**: Defines the GraphQL queries that can be made, such as fetching a specific author or post by ID

It uses the services to fetch data from the DataStore

**Mutation**: Defines the GraphQL mutations, specifically adding a new post

It takes a CreatePostInput object containing the title, content, and authorId, creates a new Post object, and adds it to the DataStore via the PostService

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

## 7. Configure the application middleware (program.cs)

**Application Middleware Configuration**

**Program.cs**: Sets up the application, including registering the AuthorService and PostService with the .NET dependency injection system

It also configures the GraphQL server with Hot Chocolate, adding the defined queries, mutations, and types to the GraphQL schema

Finally, it maps the GraphQL endpoint and starts the application

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using GraphQLDemo.GraphQL;
using GraphQLDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IAuthorService, AuthorService>();
builder.Services.AddSingleton<IPostService, PostService>();

// Add GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<AuthorType>()
    .AddType<PostType>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

// Use GraphQL middleware
app.MapGraphQL();

app.Run();
```

## 8. How the application works

**Starting the Application**: When the application starts, it initializes the **DataStore** with predefined authors and posts

**Making Queries and Mutations**: Users can query for authors or posts by ID and create new posts through mutations

The GraphQL server processes these requests, using the services to interact with the DataStore

**Data Retrieval and Modification**: Queries retrieve data without changing it, while mutations can modify the data (e.g., adding a new post)

**GraphQL Schema**: The schema defines the structure of data that can be queried or mutated, including types, queries, and mutations. It acts as a contract between the server and client

## 9. Run and Test the application

**Testing the API**

You can test the API using tools like Banana Cake Pop (integrated with Hot Chocolate), Postman (with GraphQL support), or any GraphQL client

**Querying for Authors and Posts**: Checking if the API correctly returns existing authors and posts

**Creating New Posts**: Using mutations to add new posts and verifying they're correctly associated with authors

To run the application we open it in VSCode and execute this command

```
dotnet run
```

![image](https://github.com/luiscoco/GraphQL_dotNet8_sample1/assets/32194879/76433b30-9c11-43a4-b06b-76550d629160)

**http://localhost:5187/graphql/**

![image](https://github.com/luiscoco/GraphQL_dotNet8_sample1/assets/32194879/a6c081ba-de07-4def-9010-4a9d7b2b2483)

Test the following requests

**Query to Fetch a Single Author by ID**

To retrieve Jane Austen and her posts, you can use the following GraphQL query. This assumes you have an author query set up in your GraphQL API that accepts an id as an argument

This query requests the id, name, and all posts (including their id, title, and content) for the author with id 1, which corresponds to Jane Austen based on your DataStore initialization

```
query GetAuthorById {
  author(id: 1) {
    id
    name
    posts {
      id
      title
      content
    }
  }
}
```

![image](https://github.com/luiscoco/GraphQL_dotNet8_sample1/assets/32194879/5aeefdf7-c853-40bd-bffd-cb5e38af8a85)

**Query to Fetch a Single Post by ID**

To retrieve one of the initialized posts, for example, the post with id 1 titled "Exploring GraphQL", you can use the following GraphQL query

This assumes you have a post query in your GraphQL API that accepts an id as an argument

This query requests the id, title, content, and author information (including the author's id and name), as well as the authorId for the post with id 1

```
query GetPostById {
  post(id: 1) {
    id
    title
    content
    author {
      id
      name
    }
    authorId
  }
}
```

![image](https://github.com/luiscoco/GraphQL_dotNet8_sample1/assets/32194879/a1778582-e64b-4397-98a0-301f6a536dfe)

Now we can add new Post with this request



```
mutation AddNewPost {
  addPost(input: {
    title: "The Benefits of GraphQL Subscriptions",
    content: "GraphQL subscriptions allow clients to receive real-time updates from the server, which is great for dynamic content updates.",
    authorId: 1
  }) {
    id
    title
    content
    author {
      id
      name
    }
  }
}
```

![image](https://github.com/luiscoco/GraphQL_dotNet8_sample1/assets/32194879/cd49bd23-de5d-448a-9ec7-ed44ccb1821b)

