using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Recurly.Configuration
{
    public static class Recurly
    {
        public static void UseRecurly(IConfigurationRoot config)
        {
            var apiKey = config["Recurly:ApiKey"];
            var subDomain = config["Recurly:Subdomain"];
            var privateKey = config["Recurly:PrivateKey"];//Not used anyway
            var pageSizeString = config["Recurly:PageSize"];
            int pageSize = 50;

            if (!string.IsNullOrWhiteSpace(pageSizeString))
            {
                Int32.TryParse(pageSizeString, out pageSize);
            }

            Settings.Instance.Initialize(apiKey, subDomain, privateKey, pageSize);
        }
    }
}
