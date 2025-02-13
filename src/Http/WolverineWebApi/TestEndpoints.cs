using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;

namespace WolverineWebApi;

[Special]
public static class TestEndpoints
{
    [WolverineGet("/hello")]
    public static string Speak()
    {
        return "Hello";
    }

    [WolverineGet("/results/static")]
    public static Results FetchStaticResults()
    {
        return new Results
        {
            Sum = 3,
            Product = 4
        };
    }

    #region sample_using_string_route_parameter

    [WolverineGet("/name/{name}")]
    public static string SimpleStringRouteArgument(string name)
    {
        return $"Name is {name}";
    }

    #endregion

    #region sample_using_numeric_route_parameter

    [WolverineGet("/age/{age}")]
    public static string IntRouteArgument(int age)
    {
        return $"Age is {age}";
    }

    #endregion

    #region sample_using_string_value_as_query_string

    [WolverineGet("/querystring/string")]
    public static string UsingQueryString(string name) // name is from the query string
    {
        return name.IsEmpty() ? "Name is missing" : $"Name is {name}";
    }

    #endregion
    
    [WolverineGet("/querystring/int")]
    public static string UsingQueryStringParsing(Recorder recorder, int? age)
    {
        recorder.Actions.Add("got through query string usage");
        return $"Age is {age}";
    }
    
    [WolverineGet("/querystring/int/nullable")]
    public static string UsingQueryStringParsingNullable(int? age)
    {
        if (!age.HasValue) return "Age is missing";
        return $"Age is {age}";
    }

    #region sample_simple_wolverine_http_endpoint

    [WolverinePost("/question")]
    public static Results PostJson(Question question)
    {
        return new Results
        {
            Sum = question.One + question.Two,
            Product = question.One * question.Two
        };
    }

    #endregion
    
    #region sample_simple_wolverine_http_endpoint_async

    [WolverinePost("/question2")]
    public static Task<Results> PostJsonAsync(Question question)
    { 
        var results = new Results
        {
            Sum = question.One + question.Two,
            Product = question.One * question.Two
        };

        return Task.FromResult(results);
    }

    #endregion
}

public class Results
{
    public int Sum { get; set; }
    public int Product { get; set; }
}

public class Question
{
    public int One { get; set; }
    public int Two { get; set; }
}