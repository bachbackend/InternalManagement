using BachBinHoangManagement.Models;
using BachBinHoangManagement.Service;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace BachBinHoangManagement
{
    public class Program
    {
        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            //builder.EntitySet<Product>("ProductOdata");
            //builder.EntitySet<Prize>("PrizeOdata");
            //builder.EntitySet<User>("UserOdata");
            //builder.EntitySet<Article>("ArticleOdata");
            //builder.EntitySet<Category>("CategoryOdata");
            //builder.EntitySet<Certificate>("CertificateOdata");
            //builder.EntitySet<Constructionproject>("ConstructionprojectOdata");
            return builder.GetEdmModel();
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.WriteIndented = false; // Optional for formatting
                    options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.Services.AddSwaggerGen(options =>
            //{
            //    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    options.IncludeXmlComments(xmlPath);
            //    options.SwaggerDoc("v1", new OpenApiInfo { Title = "server", Version = "v1" });

            //    // Register the custom enum schema filter
            //    //options.SchemaFilter<EnumSchemaFilter>();
            //    // Add JWT Authentication
            //    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    {
            //        In = ParameterLocation.Header,
            //        Description = "Please enter JWT with Bearer prefix",
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.ApiKey,
            //        Scheme = "Bearer"
            //    });

            //    options.MapType<DateTime>(() => new OpenApiSchema
            //    {
            //        Type = "string",
            //        Format = "date-time", // Defines DateTime format in Swagger UI
            //        Example = new OpenApiString("2024-01-01T00:00:00Z") // Example value
            //    });

            //    // Apply this JWT scheme globally to all endpoints
            //    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //        {
            //            new OpenApiSecurityScheme
            //            {
            //                Reference = new OpenApiReference
            //                {
            //                    Type = ReferenceType.SecurityScheme,
            //                    Id = "Bearer"
            //                },
            //                Scheme = "oauth2",
            //                Name = "Bearer",
            //                In = ParameterLocation.Header
            //            },
            //            new List<string>()
            //        }
            //});
            //});

            // session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn của session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("CORSPolicy", builder =>
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials()
                           .SetIsOriginAllowed((host) => true));
            });
            builder.Services.AddScoped<MailService>();

            // Đăng ký DbContext với MySQL sử dụng Pomelo.EntityFrameworkCore.MySql
            builder.Services.AddDbContext<InternalManagementContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("MyDatabase"),
                    // Sử dụng Pomelo để tự động phát hiện phiên bản MySQL
                    new MySqlServerVersion(new Version(8, 0, 0))  // Thay bằng phiên bản MySQL của bạn
                )
            );
            // Bind PaginationSettings to configuration
            builder.Services.Configure<PaginationSettings>(builder.Configuration.GetSection("PaginationSettings"));
            //builder.Services.AddScoped<IImageService, ImageService>();
            //builder.Services.AddMemoryCache();
            //builder.Services.AddScoped<CacheService>();

            //Jwt
            //if (!builder.Environment.IsEnvironment("Testing"))
            //{
            //    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //        ValidAudience = builder.Configuration["Jwt:Audience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            //    };
            //});
            //}

            // ODATA
            builder.Services.AddControllers().AddOData(option => option.Select().Filter().Count().OrderBy().Expand().SetMaxTop(100)
            .AddRouteComponents("odata", GetEdmModel()));

            builder.Services.AddAuthorization(options =>
            {
                // Permission-based policies
                options.AddPolicy("ReadPolicy", policy => policy.RequireClaim("Permission", "ReadData"));
                options.AddPolicy("WritePolicy", policy => policy.RequireClaim("Permission", "WriteData"));
                options.AddPolicy("DeletePolicy", policy => policy.RequireClaim("Permission", "DeleteData"));

                // Role-based policies
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
                options.AddPolicy("UserPolicy", policy => policy.RequireRole("Customer"));
                options.AddPolicy("GuestPolicy", policy => policy.RequireRole("Guest"));
            });

            //builder.Services.AddScoped<IAttributeRepository, AttributeRepository>();
            //builder.Services.AddScoped<IAttributeService, AttributeService>();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            // Register the custom WebSocket server
            /*builder.Services.AddSingleton<WebSocketHandler>();
            builder.Services.AddHostedService<WebSocketHostedService>();*/
            //builder.Services.AddHostedService<WebSocketHostedService>();
            var app = builder.Build();
            app.UseMiddleware<SwaggerAuthMiddleware>();
            app.UseWebSockets();
            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            // cân nhắc sửa
            if (!app.Environment.IsEnvironment("Testing"))
            {
                app.UseStaticFiles();
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseHttpsRedirection();
            app.UseCors("CORSPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            // cái này nữa
            //app.UseStaticFiles();
            app.MapControllers();
            app.UseODataBatching();
            app.UseSession();
            app.Run();
        }
    }
}
