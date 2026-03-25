using System.Configuration;

namespace Uranus.Suite.Helpers
{
    public static class AppSettingsHelper
    {
        public static string AuthApiUrl => ConfigurationManager.AppSettings["AuthApiUrl"];
    }
}