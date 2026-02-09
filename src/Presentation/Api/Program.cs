using ReSR.Application.Core;
using ReSR.Infrastructure.Core;
using ReSR.Presentation.Api.Core;

var builder = WebApplication.CreateBuilder(args);

builder.InitInfrastructure();
builder.InitApplication();
builder.InitPresentation();

var app = builder.Build();

app.FinalizePresentation();

app.Run();
