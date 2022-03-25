namespace Api.ApplicationBuilder;

public static class BuildCors
{
    internal static void Prepare(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        });
    }

    internal static void App(WebApplication app)
    {
        app.UseCors("CorsPolicy");
    }
}