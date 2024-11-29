using Microsoft.Extensions.Compliance.Redaction;
using Serilog.Core;
using Serilog.Events;
//???????????????????????????????????????????????
public class UserNameEnricher : ILogEventEnricher
{
    private readonly Redactor _redactor;

    public UserNameEnricher(Redactor redactor)
    {
        _redactor = redactor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var userName = "SensitiveUserName";
        var redactedUserName = _redactor.Redact(userName);
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", redactedUserName));
    }
}