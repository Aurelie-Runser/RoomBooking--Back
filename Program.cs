using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Data;
using RoomBookingApi.Middlewares;
using RoomBookingApi.Models;
using RoomBookingApi.Services;
using Serilog;

namespace RoomBookingApi
{

    internal class Program
    {
        private static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            try
            {
                Log.Information("Starting App");

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.Services.AddControllers();
                builder.Services.AddDbContext<RoomApiContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

                var secretKey = builder.Configuration["Jwt:SecretKey"];
                var tokenExpiryDuration = int.Parse(builder.Configuration["Jwt:ExpiryDurationInHours"]);
                builder.Services.AddSingleton(new JwtTokenService(secretKey, tokenExpiryDuration));

                // TODO : A changer lors de la mise en ligne pour restreindre l'accès
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll",
                        policy => policy
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
                });

                builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("ApplicationSettings"));
                
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseCors("AllowAll");

                app.UseHttpsRedirection();
                app.UseMiddleware<ExceptionMiddleware>();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}