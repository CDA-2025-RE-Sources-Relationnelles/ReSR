using FluentResponse;
using ReSR.Application.Core;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Core;
using ReSR.Infrastructure.Core;

var builder = WebApplication.CreateBuilder(args);

builder.InitInfrastructure();
builder.InitApplication();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/", async (IRepository<QuizResource> repo, IRepository<Category> cat) => {
    var category = await cat.TryGetAsync(6).UnwrapAsync();
    await repo.TryAddAsync(QuizResource.TryCreate("hello", category, [], "", [
        new QuizQuestion {
            Score = 10,
            Content = "Vrai ou faux",
            Answers = [
                new QuizAnswer {
                    IsCorrect = true,
                    Content = "Vrai"
                },
                new QuizAnswer {
                    IsCorrect = false,
                    Content = "Faux"
                }
            ]
        },
        new QuizQuestion {
            Score = 20,
            Content = "Vrai ou vrai",
            Answers = [
                new QuizAnswer {
                    IsCorrect = true,
                    Content = "Vrai"
                },
                new QuizAnswer {
                    IsCorrect = false,
                    Content = "Vrai"
                },
                new QuizAnswer {
                    IsCorrect = false,
                    Content = "Faux"
                }
            ]
        }
    ]).Unwrap()).UnwrapAsync();
});

app.Run();
