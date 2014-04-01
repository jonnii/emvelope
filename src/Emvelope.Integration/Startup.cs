using System.Web.Http;
using Owin;

namespace Emvelope.Integration
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            var formatter = new EmvelopeMediaTypeFormatter();
            formatter.AddMetaProvider(new PagingMetaProvider());

            config.Formatters.Insert(0, formatter);

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
