using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSource.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;

namespace WebApiSource
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
            SigningConfigurations signingConfigurations = new SigningConfigurations();
            TokenConfigurations tokenConfigurations = new TokenConfigurations(Configuration);
            services.AddSingleton(signingConfigurations);
            services.AddSingleton(tokenConfigurations);
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearerOptions =>
            {                
                bearerOptions.TokenValidationParameters.IssuerSigningKey = signingConfigurations.Key;
                bearerOptions.TokenValidationParameters.ValidAudience = tokenConfigurations.Audience;
                bearerOptions.TokenValidationParameters.ValidIssuer = tokenConfigurations.Issuer;
                bearerOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
                bearerOptions.TokenValidationParameters.ValidateLifetime = true;
                bearerOptions.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
            });
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser()
                    .Build());
            });
            
            services.AddDbContext<ApplicationDbContext>(a => a.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>().AddDefaultUI(UIFramework.Bootstrap4).AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddCors().AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);            
        }
                
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");                
                app.UseHsts();
            }
                                    
            app.UseCors(x =>
            {
                x.AllowAnyHeader();
                x.AllowAnyMethod();
                x.AllowAnyOrigin();
                x.AllowCredentials();                
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
