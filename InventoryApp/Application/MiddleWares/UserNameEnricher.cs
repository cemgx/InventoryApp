using Microsoft.Extensions.Compliance.Redaction;
using Serilog.Core;
using Serilog.Events;
using System.Security.Claims;
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
        var userName = ClaimTypes.Email;
        var redactedUserName = _redactor.Redact(userName);
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", redactedUserName));
    }
}