using Serilog.Core;
using Serilog.Events;

namespace InventoryApp.Application.MiddleWares
{
    public class UserNameEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var userName = "dynamicUser";
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", userName));
        }
    }
}
