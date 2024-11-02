using Ecommerce.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<DataContext>(o =>
            {
                o.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"));
            });
            builder.Services.AddTransient<SeedDb>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            SeedData(app);
            void SeedData(WebApplication app)
            {
                IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();
                using (IServiceScope scope = scopedFactory!.CreateScope())
                {
                    SeedDb? service = scope.ServiceProvider.GetService<SeedDb>();
                    service!.SeedAsync().Wait();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}