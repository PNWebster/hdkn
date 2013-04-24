using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.ServiceModel;
using System.Text;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Web.Http.Dispatcher;
using System.Web.Http.Tracing;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ITraceWriter = System.Web.Http.Tracing.ITraceWriter;

namespace Hadouken.Http.HttpServer
{
    public class HttpApiServerFactory : IHttpApiServerFactory
    {
        public IHttpApiServer CreateHttpApiServer(Uri baseAddress, params System.Reflection.Assembly[] assemblies)
        {
            var config = new HttpSelfHostConfiguration(baseAddress);

            // Set dependency resolver
            config.DependencyResolver = new ApiDependencyResolver();
            
            // Set basic auth
            config.ClientCredentialType = HttpClientCredentialType.Basic;
            config.UserNamePasswordValidator = new IdentityValidator();

            // Replace services
            config.Services.Replace(typeof (IAssembliesResolver), new CustomAssembliesResolver(assemblies));
            config.Services.Replace(typeof (ITraceWriter), new CustomTraceWriter());

            // Set up formatter
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters.Add(new VersionConverter());
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Replace formatters
            config.Formatters.Clear();
            config.Formatters.Add(formatter);

            config.Routes.MapHttpRoute(
                "API Action route",
                "{controller}/{id}/{action}"
            );

            config.Routes.MapHttpRoute(
                "API Default",
                "{controller}/{id}",
                new { controller = "System", id = RouteParameter.Optional }
            );

            return new HttpApiServer(config);
        }
    }

    public class CustomTraceWriter : ITraceWriter
    {
        public void Trace(HttpRequestMessage request, string category,TraceLevel level, Action<TraceRecord> traceAction)
        {

            TraceRecord traceRecord = new TraceRecord(request, category, level);

            traceAction(traceRecord);

            ShowTrace(traceRecord);

        }

    

       private void ShowTrace(TraceRecord traceRecord)

       {

           Console.WriteLine(

               String.Format(

                   "{0} {1}: Category={2}, Level={3} {4} {5} {6} {7}",

                   traceRecord.Request.Method.ToString(), 

                   traceRecord.Request.RequestUri.ToString(), 

                   traceRecord.Category,
                                    traceRecord.Level,

                   traceRecord.Kind,

                   traceRecord.Operator,

                   traceRecord.Operation,

                   traceRecord.Exception != null

                       ? traceRecord.Exception.GetBaseException().Message

                       : !string.IsNullOrEmpty(traceRecord.Message)

                           ? traceRecord.Message

                           : string.Empty

               ));

       }
    }
}
