using System;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DatingApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Add User Seed to Seed the users in the database, hence a change has been applied to the default Main Method
          var host =  CreateHostBuilder(args).Build();
          using (var scope = host.Services.CreateScope())
          {
              var Services = scope.ServiceProvider;
              try
              {
                  var context = Services.GetRequiredService<DataContext>();
                  // if the db doen't exist, will create it, otherwise will apply the migrations if needed
                  context.Database.Migrate();
                  Seed.SeedUsers(context);

              }
              catch(Exception ex)
              {
                  var logger = Services.GetRequiredService<ILogger<Program>>();
                  logger.LogError(ex, "an error occured during migration");

              }
          }
          host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
