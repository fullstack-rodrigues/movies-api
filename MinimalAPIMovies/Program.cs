using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMovies.Endpoints;
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

var genresEndpoints = app.MapGroup("/genres").MapGenres();

app.Run();

