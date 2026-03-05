using lps_crud_test.Models.LpsDb;
using System;
using System.Globalization;
using System.Linq;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using lps_crud_test.Services.Interfaces;
using lps_crud_test.Services;

namespace lps_crud_test.Helpers
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
            //services.AddNotyf(config => { config.DurationInSeconds = 6; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });

            services.AddScoped<IBookService, BookServices>();
            services.AddScoped<IBookTypeService, BookTypeService>();

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
