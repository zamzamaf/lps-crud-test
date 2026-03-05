using lps_crud_test.Configuration;
using lps_crud_test.Configuration.Constants;

namespace lps_crud_test.Helpers
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