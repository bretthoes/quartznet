#if NETFRAMEWORK
using Microsoft.Extensions.Logging;
#endif

namespace Quartz.Util;

internal static class Configuration
{
    internal static System.Collections.Specialized.NameValueCollection? GetSection(string sectionName)
    {
#if NETFRAMEWORK
        try
        {
            return (System.Collections.Specialized.NameValueCollection) System.Configuration.ConfigurationManager.GetSection(sectionName);
        }
        catch (Exception e)
        {
            var logger = Diagnostics.LogProvider.CreateLogger(nameof(Configuration));
            logger.LogWarning(e, "could not read configuration using ConfigurationManager.GetSection: {ExceptionMessage}", e.Message);
            return null;
        }
#else
        return null;
#endif
    }
}