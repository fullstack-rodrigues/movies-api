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
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));

app.MapGet("/genres/{id:int}", async (IGenresRepository genresRepository, int id) =>
{
    var genre = await genresRepository.GetById(id);
    return TypedResults.Ok(genre);
});

app.MapPost("/genres", async (Genre genre, IGenresRepository genresRepository) =>
{
    await genresRepository.Create(genre);
    return TypedResults.Created($"genres/{genre.Id}");
});

app.Run();
