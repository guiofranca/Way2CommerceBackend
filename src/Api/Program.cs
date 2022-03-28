using Api.ApplicationBuilder;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddFluentValidation();

builder.BuildSwagger();

builder.InjectDependencies();

builder.BuildCorsPolicy();

builder.BuildAuth();

var app = builder.Build();

app.UseDevSwagger();

app.UseHttpsRedirection();

app.UseAuth();

app.MapControllers();

app.UseCorsPolicy();

app.Run();
