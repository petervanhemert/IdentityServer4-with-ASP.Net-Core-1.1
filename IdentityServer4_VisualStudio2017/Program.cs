using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer4_VisualStudio2017
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cert = new X509Certificate2("IdentityServer4WithVisualStudio2017.pfx", "123456789");

            var host = new WebHostBuilder()
                .UseKestrel(cfg => cfg.UseHttps(cert))
                .UseUrls("https://localhost:5011")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
