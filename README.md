# My first sample with .NET8 GraphQL

For more info about GraphQL see (youtube video):

GraphQL is a query language for APIs and a runtime for executing those queries by using a type system you define for your data

Unlike REST, which uses a fixed set of endpoints to fetch data, GraphQL allows clients to request exactly the data they need and to aggregate data from multiple sources with a single request

This can lead to more efficient network requests and a better development experience

## 1. Create a New .NET 8 Project in VSCode 

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
