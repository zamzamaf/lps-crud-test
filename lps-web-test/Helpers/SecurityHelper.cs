using lps_web_test.Infrastructure.Configuration.Constants;

namespace lps_web_test.Helpers
{
    public static class SecurityHelper
    {
        public static string GetConnectionStringLpspDb(this IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(ConfigurationConsts.LpsDbConnectionStringKey);
            return connectionString;
        }
    }
}
