using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Identity;

namespace Play.Common.Extensions
{
    public static class AuthenticationExtension
    {
        public static AuthenticationBuilder AddJwtBearerAuthentication(this IServiceCollection services)
        {
            return services.ConfigureOptions<ConfigureJwtBearerOptions>()
                        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer();
        }
    }
}
