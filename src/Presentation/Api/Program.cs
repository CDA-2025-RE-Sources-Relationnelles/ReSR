using Microsoft.EntityFrameworkCore;
using ReSR.Application.Core;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Ports;
using ReSR.Infrastructure.Core;
using ReSR.Presentation.Api.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.InitInfrastructure();
builder.InitApplication();
builder.InitPresentation();

var app = builder.Build();

await app.FinalizePresentationAsync();

app.Run();