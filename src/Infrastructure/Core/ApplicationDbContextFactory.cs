using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ReSR.Infrastructure.Core;
public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>  {
    public ApplicationDbContext CreateDbContext(string[] args) {

        var configurationBuilder = new ConfigurationBuilder();
        var assemblyLocation     = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        
        if (assemblyLocation is not null) {

            var directory = new DirectoryInfo(assemblyLocation);
            while (directory is not null && directory.GetFiles("*.slnx").Length == 0)
                directory = directory.Parent;

            if (directory is not null)
                configurationBuilder
                    .SetBasePath(directory.FullName)
                    .AddEnvironmentVariables()
                    .AddJsonFile(
                        path     : args.FirstOrDefault() ?? "appsettings.shared.Development.json",
                        optional : false
                    );
        }

        var configuration = configurationBuilder.Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString());

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
