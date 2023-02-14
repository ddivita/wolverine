using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace WolverineWebApi;

public class TraceEndpoint
{
    [WolverineGet("/trace")]
    public string Hey() => "hey";
}