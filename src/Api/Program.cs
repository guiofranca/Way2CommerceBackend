using Api.ApplicationBuilder;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddFluentValidation();

BuildSwagger.Prepare(builder);

BuildDependencyInjection.Prepare(builder);

BuildCors.Prepare(builder);

BuildAuth.Prepare(builder);

var app = builder.Build();

BuildSwagger.App(app);

app.UseHttpsRedirection();

BuildAuth.App(app);

app.MapControllers();

BuildCors.App(app);

app.Run();
