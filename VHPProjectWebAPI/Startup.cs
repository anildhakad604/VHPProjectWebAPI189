//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Logging;
//using Microsoft.OpenApi.Models;
//using NLog;
//using STEIWebAPI.Helper;
//using VHPProjectBAL;
//using VHPProjectCommonUtility.Configuration;
//using VHPProjectDAL.AutoMapperProfile;
//using VHPProjectDAL.Helper;
//using VHPProjectDAL.MemberRepo;

//namespace VHPProjectWebAPI
//{
//    public class Startup
//    {
//        private AppsettingsConfig _appSettings;
//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//            LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
//        }

//        public IConfiguration Configuration { get; }

//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddCors();
//            services.AddControllers(

//            ).AddNewtonsoftJson();
//            //services.AddAutoMapper(typeof(MapperProfileDeclaration));
//            var serviceRegistry = new ServiceRegistry();
//            services.AddMvc();
//            services.AddAutoMapper(typeof(MapperProfileDeclaration));
//            services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

//                // Add the custom operation filter
//                //c.OperationFilter<EncryptedRequestOperationFilter>();
//            });
//            IdentityModelEventSource.ShowPII = true;
//            AppsettingsConfig appSettings = LoadConfiguration(services);
//            _appSettings = appSettings;
//            serviceRegistry.ConfigureDataContext(services, appSettings);

//            serviceRegistry.ConfigureDependencies(services, appSettings);







//            //    services.AddAuthentication(
//            //        options =>
//            //        {
//            //            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//            //        })
//            //        .AddJwtBearer("JwtBearer", options =>
//            //        {
//            //            options.TokenValidationParameters = new TokenValidationParameters
//            //            {
//            //                ValidateIssuer = true,
//            //                ValidIssuer = Configuration["Jwt:Issuer"],
//            //                ValidateAudience = true,
//            //                ValidAudience = Configuration["Jwt:Audience"],
//            //                ValidateLifetime = true,
//            //                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
//            //                ValidateIssuerSigningKey = true
//            //            };
//            //        });
//            //    services.AddAuthorization(options =>
//            //    {
//            //        options.AddPolicy("view_all_tabs", Policies.Create("view_all_tabs"));
//            //        options.AddPolicy("create_entry", Policies.Create("create_entry"));
//            //        options.AddPolicy("approve_entry", Policies.Create("approve_entry"));
//            //        options.AddPolicy("final_approve", Policies.Create("final_approve"));
//            //        options.AddPolicy("create_users", Policies.Create("create_users"));
//            //});

//        }

//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {

//            var envr = Configuration["MasterProjData:Environment"];


//            //app.Use(async (context, next) =>
//            //{
//            //    var url = context.Request.GetTypedHeaders().Referer;
//            //    if (url != null)
//            //    {
//            //        if (!url.ToString().StartsWith("https") && envr != "DEV")
//            //        {
//            //            byte[] data = Encoding.ASCII.GetBytes("not recognized request");
//            //            context.Response.StatusCode = 403;
//            //            await context.Response.Body.WriteAsync(data);
//            //            return;
//            //        }
//            //        await next();
//            //    }
//            //    else
//            //    {
//            //        await next();
//            //    }

//            //});

//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }

//            app.UseAuthentication();

//            app.UseHttpsRedirection();

//            app.UseStaticFiles();

//            app.UseRouting();

//            //app.UseMiddleware<EncryptionMiddleware>();
//            app.UseMiddleware<GlobalExceptionMiddleware>();

//            app.UseAuthorization();

//            app.UseEndpoints(endpoints =>
//            {
//                endpoints.MapControllers();
//                endpoints.MapGet("/", async context =>
//                {
//                    await context.Response.WriteAsync("My Running Enviroment:" + env.EnvironmentName + "");
//                });
//            });
//            app.UseSwagger();
//            app.UseSwaggerUI(s =>
//            {
//                s.SwaggerEndpoint("/swagger/v1/swagger.json", "VHPProjectAPI");
//            });
//        }

//        private AppsettingsConfig LoadConfiguration(IServiceCollection services)
//        {
//            AppsettingsConfig appSettings = new AppsettingsConfig();
//            Configuration.Bind(appSettings);
//            services.AddSingleton(appSettings);
//            return appSettings;
//        }

//    }
//}


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using STEIWebAPI.Helper;
using System.Text;
using VHPProjectCommonUtility.Configuration;
using VHPProjectDAL.Authentication;
using VHPProjectDAL.AutoMapperProfile;
using VHPProjectDAL.DataModel;
using VHPProjectDAL.Helper;
using VHPProjectWebAPI.Helper.Authentication;
using VHPProjectWebAPI.Helper.Authorization;

namespace VHPProjectWebAPI
{
    public class Startup
    {
        private AppsettingsConfig _appSettings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers().AddNewtonsoftJson();

            services.AddMvc();
            services.AddAutoMapper(typeof(MapperProfileDeclaration));


            //        services.AddSwaggerGen(c =>
            //        {
            //            c.SwaggerDoc("v1", new OpenApiInfo { Title = "VHP Project API", Version = "v1" });


            //            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //            {
            //                Name = "Authorization",
            //                Type = SecuritySchemeType.ApiKey,
            //                Scheme = "Bearer",
            //                BearerFormat = "JWT",
            //                In = ParameterLocation.Header,
            //                Description = "Enter your JWT token here. Example: Bearer {your token}"
            //            });


            //            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //            {
            //                new OpenApiSecurityScheme
            //                {
            //                    Reference = new OpenApiReference
            //                    {
            //                        Type = ReferenceType.SecurityScheme,
            //                        Id = "Bearer"
            //                    }
            //                },
            //                new string[] {}
            //            }
            //});
            //        });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1"
                });
                //jwt token
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Enter your JWT Access Token",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
                {jwtSecurityScheme, new List<string>() }
    });
                //custom token
                var customSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "SHA256",
                    Name = "CustomAuthorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "CustomTokenScheme",
                    Description = "Enter the static token here",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "CustomToken"
                    }
                };
                options.AddSecurityDefinition("CustomToken", customSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
                {customSecurityScheme, new List<string>() }
    });
            });





            IdentityModelEventSource.ShowPII = true;

           
            AppsettingsConfig appSettings = LoadConfiguration(services);
            _appSettings = appSettings;

            var serviceRegistry = new ServiceRegistry();

            
            serviceRegistry.ConfigureDataContext(services, appSettings);
            serviceRegistry.ConfigureDependencies(services, appSettings);

           


            services.AddAuthorization(options =>
            {
                // Admin Policies
                options.AddPolicy(Policies.AddSatsang, policy =>
                    policy.Requirements.Add(new PermissionRequirement(Policies.AddSatsang)));

                options.AddPolicy(Policies.DeleteMember, policy =>
                    policy.Requirements.Add(new PermissionRequirement(Policies.DeleteMember)));

                options.AddPolicy(Policies.AddMember, policy =>
                    policy.Requirements.Add(new PermissionRequirement(Policies.AddMember)));

                // Member Policies
                options.AddPolicy(Policies.UpdateMember, policy =>
                    policy.Requirements.Add(new PermissionRequirement(Policies.UpdateMember)));
            });

            // Register the custom permission handler
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();



            // Add Swagger (optional)
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            services.AddAuthentication(
                    options =>
                    {
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = Configuration["Jwt:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = Configuration["Jwt:Audience"],
                            ValidateLifetime = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                            ValidateIssuerSigningKey = true
                        };
                    }).AddScheme<CustomTokenAuthenticationOptions, CustomTokenAuthenticationHandler>("CustomTokenScheme", options => { })
;


            services.AddDbContext<MasterProjContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("DefaultConnection");
                options.UseMySql(connectionString, ServerVersion.Parse("8.0.44-mysql"));
            });


            //var connectionString = Configuration.GetConnectionString("DefaultConnection");
            //services.AddDbContext<MasterProjContext>(options =>
            //    options.UseSqlServer(connectionString));

           


            // Add other repositories/services here as needed
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var envr = Configuration["MasterProjData:Environment"];

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseStatusCodePages(context =>
            {
                var response = context.HttpContext.Response;

                if (response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    return response.WriteAsJsonAsync(new
                    {
                        message = "Forbidden: You do not have access to this resource."
                    });
                }
                else if (response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    return response.WriteAsJsonAsync(new
                    {
                        message = "Unauthorized: Authentication is required."
                    });
                }

                return Task.CompletedTask;
            });
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("My Running Environment: " + env.EnvironmentName);
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "VHPProjectAPI");
            });
        }

        private AppsettingsConfig LoadConfiguration(IServiceCollection services)
        {
            AppsettingsConfig appSettings = new AppsettingsConfig();
            Configuration.Bind(appSettings);
            services.AddSingleton(appSettings);
            return appSettings;
        }
    }
}



