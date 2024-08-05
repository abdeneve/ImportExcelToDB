using Autofac;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;
public static class DependencyInjection
{
    public static void RegisterInfrastructureServices(this ContainerBuilder builder, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("App");

        builder.RegisterType<ApplicationDbContext>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.Register(c =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return optionsBuilder.Options;

        })
        .As<DbContextOptions<ApplicationDbContext>>();
    }
}
