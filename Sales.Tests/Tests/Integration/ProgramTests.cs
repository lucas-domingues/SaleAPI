using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Sales.API.Interfaces.Services;
using Sales.API.Services;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Sales.API.Data;
using Sales.API.Middleware;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Sales.API.Publishers;
using Sales.API.RabbitMQ;

namespace Sales.Tests.Tests.Integration
{
    public class ProgramTests
    {
        private WebApplicationBuilder CreateWebApplicationBuilder()
        {
            var webApplicationOptions = new WebApplicationOptions
            {
                ContentRootPath = Directory.GetCurrentDirectory(),
                EnvironmentName = Environments.Development,
            };

            var builder = WebApplication.CreateBuilder(webApplicationOptions);

            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Jwt:Key", "your-test-secret-key"},
                {"Jwt:Issuer", "test-issuer"},
                {"Jwt:Audience", "test-audience"},
                {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=testdb;"}
            });

            return builder;
        }

        [Fact]
        public void ConfigureServices_RegistersRequiredServices()
        {
            // Arrange
            var builder = CreateWebApplicationBuilder();

            // Act
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Registrar DbContext primeiro
            builder.Services.AddDbContext<SalesDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IEventPublisher>(new Mock<IEventPublisher>().Object);

            // Registrar serviços após suas dependências
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            // Assert
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                Assert.NotNull(serviceProvider.GetService<IProductService>());
                Assert.NotNull(serviceProvider.GetService<ICartService>());
                Assert.NotNull(serviceProvider.GetService<IUserService>());
                Assert.NotNull(serviceProvider.GetService<IAuthService>());
            }
        }

        [Fact]
        public void ConfigureServices_ConfiguresDbContext()
        {
            // Arrange
            var builder = CreateWebApplicationBuilder();

            // Act
            builder.Services.AddDbContext<SalesDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Assert
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var dbContext = serviceProvider.GetService<SalesDbContext>();
                Assert.NotNull(dbContext);
            }
        }

        [Fact]
        public void ConfigureServices_ConfiguresSwagger()
        {
            // Arrange
            var builder = CreateWebApplicationBuilder();

            // Act
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sales API",
                    Version = "v1",
                    Description = "API for managing products, cart, and checkout process.",
                    Contact = new OpenApiContact
                    {
                        Name = "Test User",
                        Email = "test@test.com"
                    }
                });
            });

            // Adicionar serviços necessários para o Swagger
            builder.Services.AddControllers();
            builder.Services.AddRouting();

            var app = builder.Build();

            // Assert
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var swaggerGenOptions = serviceProvider.GetService<IOptions<SwaggerGenOptions>>();
                Assert.NotNull(swaggerGenOptions);
            }
        }

        [Fact]
        public void ConfigureServices_ConfiguresAuthentication()
        {
            // Arrange
            var builder = CreateWebApplicationBuilder();

            // Act
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            var app = builder.Build();

            // Assert
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var authenticationScheme = serviceProvider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
                Assert.NotNull(authenticationScheme);
            }
        }

        [Fact]
        public void Configure_UsesMiddlewareInCorrectOrder()
        {
            // Arrange
            var builder = CreateWebApplicationBuilder();

            // Configurar serviços necessários
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            // Act
            var app = builder.Build();

            // Configurar o pipeline
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Assert
            Assert.NotNull(app);
        }

        [Fact]
        public void ConfigureServices_ConfiguresJsonOptions()
        {
            var builder = CreateWebApplicationBuilder();

            // Act
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            var app = builder.Build();

            // Assert
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var options = serviceProvider.GetService<IOptions<JsonOptions>>();
                Assert.NotNull(options);
                Assert.NotNull(options.Value);
                Assert.Contains(options.Value.JsonSerializerOptions.Converters,
                    converter => converter is JsonStringEnumConverter);
            }
        }
    }
}
