using ReSR.Application.Core;
using ReSR.Infrastructure.Core;
using ReSR.Presentation.Api.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.InitInfrastructure();
builder.InitApplication();
builder.InitPresentation();

var app = builder.Build();

app.FinalizePresentation(args);

app.Run();