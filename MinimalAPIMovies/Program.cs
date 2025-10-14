using MinimalAPIMovies.Entities;

var builder = WebApplication.CreateBuilder(args);


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

app.MapGet("/genres", () =>
{
    var  genres = new List<Genre>()
    {
        new Genre
        {
            Id = 1,
            Name = "Drama"
        },
        new Genre
        {
            Id = 2,
            Name = "Comedy"
        },
        new Genre
        {
            Id = 3,
            Name = "Action"
        },
    };
    return genres;
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));

app.Run();
