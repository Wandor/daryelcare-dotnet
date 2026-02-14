using DaryelCare.Data;
using DaryelCare.Services;
using DaryelCare.Tests.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DaryelCare.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DatabaseContext and ApplicationService
            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DatabaseContext));
            if (dbDescriptor != null)
                services.Remove(dbDescriptor);

            var appServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ApplicationService));
            if (appServiceDescriptor != null)
                services.Remove(appServiceDescriptor);

            // Add mock services
            services.AddSingleton<DatabaseContext, MockDatabaseContext>();
            services.AddSingleton<ApplicationService, MockApplicationService>();
        });

        builder.UseEnvironment("Testing");
    }
}
