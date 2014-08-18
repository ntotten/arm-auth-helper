using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuthHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var tenantId = ConfigurationManager.AppSettings["TenantId"];
            var clientId = ConfigurationManager.AppSettings["ClientId"];
            var resource = ConfigurationManager.AppSettings["Resource"];
            var authUrl = "https://login.windows.net";
            var header = GetAuthorizationHeader(tenantId, authUrl, clientId, resource);
            Console.Write(header);
        }

        private static string GetAuthorizationHeader(string tenantId, string authUrlHost, string clientId, string resource)
        {
            AuthenticationResult result = null;
            var thread = new Thread(() =>
            {
                try
                {
                    var authUrl = String.Format(authUrlHost + "/{0}", tenantId);
                    var context = new AuthenticationContext(authUrl);

                    result = context.AcquireToken(
                        resource: resource,
                        clientId: clientId,
                        redirectUri: new Uri("urn:ietf:wg:oauth:2.0:oob"),
                        promptBehavior: PromptBehavior.Auto);
                }
                catch (Exception threadEx)
                {
                    Console.WriteLine(threadEx.Message);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "AcquireTokenThread";
            thread.Start();
            thread.Join();

            return result.CreateAuthorizationHeader();
        }
    }
}
