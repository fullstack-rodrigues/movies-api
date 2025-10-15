using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMovies.Entities;
using MinimalAPIMovies.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGenresRepository, GenresRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");

app.UseOutputCache();

app.MapGet("/", () => "Hello World!");

app.MapGet("/genres", async (IGenresRepository genresRepository) =>
{
    var genres = await genresRepository.GetAll();
    return TypedResults.Ok(genres);
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(120)).Tag("genres-get"));

app.MapGet("/genres/{id:int}", async (IGenresRepository genresRepository, int id) =>
{
    var genre = await genresRepository.GetById(id);
    return TypedResults.Ok(genre);
});

app.MapPost("/genres", async (Genre genre, IGenresRepository genresRepository, IOutputCacheStore cacheStore) =>
{
    await cacheStore.EvictByTagAsync("genres-get", default);
    await genresRepository.Create(genre);
    return TypedResults.Created($"genres/{genre.Id}");
});

app.MapPut("/genres", async (Genre genre, IGenresRepository genresRepository, IOutputCacheStore cacheStore) =>
{
    var exist = await genresRepository.Exists(genre.Id);
    if (!exist)
    {
        return Results.NotFound();
    }

    await cacheStore.EvictByTagAsync("genres-get", default);
    await genresRepository.Update(genre);
    return Results.NoContent();

});

app.MapDelete("/genres/{id}", async(int id, IGenresRepository genresRepository, IOutputCacheStore cacheStore) =>
{
    var exist = await genresRepository.Exists(id);
    if (!exist)
    {
        return Results.NotFound();
    }
    await cacheStore.EvictByTagAsync("genres-get", default);
    await genresRepository.Delete(id);
    return Results.NoContent();
});

app.Run();
