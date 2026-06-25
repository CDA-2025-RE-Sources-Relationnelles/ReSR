using Microsoft.Extensions.Configuration;

namespace ReSR.Infrastructure.Core;
public static partial class Extensions {
    public static string GetConnectionString(this IConfiguration configuration) =>
        $"Host={configuration["DB:Host"]}:{configuration["DB:Port"]};Database={configuration["DB:Database"]};Username={configuration["DB:Username"]};Password={configuration["DB:Password"]};GSS Encryption Mode=Disable";

}