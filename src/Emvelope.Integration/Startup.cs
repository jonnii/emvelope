using System.Web.Http;
using Owin;

namespace Emvelope.Integration
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Formatters.Insert(0, new EmvelopeMediaTypeFormatter());
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new
                {
                    id = RouteParameter.Optional
                });

            app.UseWebApi(config);
        }
    }
}
