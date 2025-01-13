using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Data;
using RoomBookingApi.Middlewares;
using RoomBookingApi.Services;

namespace RoomBookingApi
{

    internal class Program
    {
        private static void Main(string[] args)
        {
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

            // A changer lors de la mise en ligne pour restreindre l'accès
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionMiddleware>(); // utilisation de Middleware pour traiter les erreurs
            app.MapControllers();

            app.Run();
        }
    }
}