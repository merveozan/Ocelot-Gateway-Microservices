using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

namespace ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure authentication parameters
            var identityUrl = Configuration.GetValue<string>("IdentityUrl");
            var secretKey = Configuration["JwtSettings:SecretKey"];
            var issuer = Configuration["JwtSettings:Issuer"];
            var audience = Configuration["JwtSettings:Audience"];


            var key = Encoding.ASCII.GetBytes(secretKey);

            services.AddAuthentication(options =>
             {
                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
             }).AddJwtBearer("IdentityApiKey", options =>  {
                    //options.Authority = identityUrl;
                    options.RequireHttpsMetadata = false; 
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,

                    };
                });
            services.AddSignalR();
            services.AddOcelot()
                .AddCacheManager(settings => settings.WithDictionaryHandle()); // Memory-based caching

            services.AddControllers();
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOcelot(); 
        }
    }
}
