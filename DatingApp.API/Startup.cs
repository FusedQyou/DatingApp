using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // When in development mode
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // Inject our context as a service so we can use this to communicate with the database.
            services.AddDbContext<DataContext>(x => {
                x.UseLazyLoadingProxies();
                x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });

            ConfigureServices(services);
        }

        // When in production mode
        public void ConfigureProductionServices(IServiceCollection services)
        {
            // Inject our context as a service so we can use this to communicate with the database.
            services.AddDbContext<DataContext>(x => {
                x.UseLazyLoadingProxies();
                x.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure the identity service
            IdentityBuilder builder = services.AddIdentityCore<User>(opt => {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            // Add the authentication service including in the app
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Configure controllers.
            // Add a policy here we require authorized identityusers unless specified anonymous.
            // Also configure NewtonsoftJson to be included and handle self referencing data properly, just in case lazy loading doesn't fix it.
            services.AddControllers(options => {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                
                options.Filters.Add(new AuthorizeFilter(policy));

            }).AddNewtonsoftJson(opt => {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            // Add corse, which will allow our Angular app to access the API from a different source.
            services.AddCors();

            // Cloudinary information
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            // Automapper to map data between DTOs
            services.AddAutoMapper(typeof(DatingRepository).Assembly);

            // Add dependency injection support for our classes.
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();

            // Filters
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null) {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            // Do not use this yet. We stick to http.
            //app.UseHttpsRedirection();

            app.UseRouting();

            // Enforce [Authorize] and similar attributes to verify the token the user should send with each request.
            app.UseAuthentication();
            app.UseAuthorization();

            // This allows our Angular app to use the API.
            // Obviously, allowing all allows more than Angular, but this will work for now.
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            // Indicates the API can serve Angular's build-ready files supplied in wwwroot, starting at index.html
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();

                // This API does not support routing, because it's an API. Duh.
                // Due to this reason refreshing on a routed page results in an error.
                // This fallback workaround makes sure that any routes are redirected to the base HTML, so angular can take care of the routing like it should.
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
