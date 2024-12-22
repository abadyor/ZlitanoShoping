
using APPL.Extations;
using Microsoft.Extensions.DependencyInjection;




namespace APPL.DepandecyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services) {

            services.AddScoped<HttpClientService>();

            services.AddHttpClient("ClientUI", client =>

            {

                client.BaseAddress = new Uri("https://localhost:7173/");

            });

            return services;
        } }
}
