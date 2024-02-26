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
