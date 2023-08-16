using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using NorthLake.SmartSearch.Repository;
using NorthLake.SmartSearch.Request;
using NorthLake.SmartSearch.Response;
using NorthLake.SmartSearch.Service;

namespace NorthLake.SmartSearch.Controllers;

[ApiController]
//[Route("[controller]")]
public class SmartSearchController : ControllerBase
{

    private readonly ILogger<SmartSearchController> _logger;

    public SmartSearchController(ILogger<SmartSearchController> logger)
    {
        _logger = logger;
    }

    
   
    [HttpGet, Route("api/SmartSearch/Get")]
    public List<SearchResponse> Get()
    {
        return new List<SearchResponse> { new SearchResponse { CompanyName = "abc", FormType = "10-K" } };
    }

    [HttpPost, Route("api/SmartSearch/Search")]
    public List<SearchResponse> Search(SearchRequest request)
    {
        try
        {
            SearchService secService = new();
            return secService.GetDataForSearch(request);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
        return new List<SearchResponse>();

    }

    [HttpGet, Route("api/SmartSearch/Suggestion")]
    public List<SearchResponse> Suggestion(string companyName)
    {
        // should be handled by the repository
        try
        {

            SearchService suggestionService = new();
            List<SearchResponse> response = suggestionService.GetDataForSuggestions(companyName);
            return response;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
        return new List<SearchResponse>();

    }

}

