namespace Api.ApplicationBuilder;

public static class ExtensionBuildCors
{
    internal static void BuildCorsPolicy(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        });
    }

    internal static void UseCorsPolicy(this WebApplication app)
    {
        app.UseCors("CorsPolicy");
    }
}