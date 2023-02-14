using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Logitar.EventSourcing.Demo.Extensions;

internal static class OpenApiExtensions
{
  public static IServiceCollection AddOpenApi(this IServiceCollection services)
  {
    return services.AddSwaggerGen(config =>
    {
      config.AddSecurity();
      config.SwaggerDoc(name: $"v{Constants.Version.Split('.').First()}", new OpenApiInfo
      {
        Contact = new OpenApiContact
        {
          Email = "francispion@hotmail.com",
          Name = "Francis Pion",
          Url = new Uri("https://www.francispion.ca/")
        },
        Description = "Identity management Web API.",
        License = new OpenApiLicense
        {
          Name = "Use under MIT",
          Url = new Uri("https://github.com/Utar94/Logitar.NET/blob/develop/LICENSE")
        },
        Title = "Portal API",
        Version = $"v{Constants.Version}"
      });
    });
  }

  private static void AddSecurity(this SwaggerGenOptions options)
  {
    options.AddSecurityDefinition(Constants.Schemes.UserId, new OpenApiSecurityScheme
    {
      Description = "Enter your user ID in the input below.",
      In = ParameterLocation.Header,
      Name = Constants.Headers.XUser,
      Scheme = Constants.Schemes.UserId,
      Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
      {
        new OpenApiSecurityScheme
        {
          In = ParameterLocation.Header,
          Name = Constants.Headers.XUser,
          Reference = new OpenApiReference
          {
            Id = Constants.Schemes.UserId,
            Type = ReferenceType.SecurityScheme
          },
          Scheme = Constants.Schemes.UserId,
          Type = SecuritySchemeType.ApiKey
        },
        new List<string>()
      }
    });
  }
}
