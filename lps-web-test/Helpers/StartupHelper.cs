using lps_web_test.Application.Interfaces;
using lps_web_test.Application.Services;
using lps_web_test.Domain.Interface;
using lps_web_test.Infrastructure.Data;
using lps_web_test.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace lps_web_test.Helpers
{
    public static class StartupHelper
    {
        public static IServiceCollection AddAndConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LpsDbContext>(options => options.UseNpgsql(configuration.GetConnectionStringLpspDb()));
            return services;
        }

        public static IServiceCollection AddAndConfigureScoped(this IServiceCollection services)
        {
            services.AddScoped<IBookTypeRepository, BookTypeRepository>();
            services.AddScoped<IBookTypeService, BookTypeService>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookService, BookService>();

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
