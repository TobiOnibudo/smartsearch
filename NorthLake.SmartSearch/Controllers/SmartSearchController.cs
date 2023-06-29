using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using NorthLake.SmartSearch.Request;
using NorthLake.SmartSearch.Response;

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
        return new List<SearchResponse> { new SearchResponse {CompanyName = "abc",FormType = "10-K"}};
    }

    [HttpPost, Route("api/SmartSearch/Search")]
    public List<SearchResponse> Search(SearchRequest request)
    {
        List<SearchResponse> list = new List<SearchResponse> { };

        int CIK = string.IsNullOrEmpty(request.CompanyCIK) ? 0 : int.Parse(request.CompanyCIK);
        var Company = request.Company.ToLower();
        string FormType = request.FormType.ToLower();

        if (CIK == 0 && Company == "" && FormType == "") { return list; }

        var conn = new MySqlConnection("Server = database-1.cvt3rfrvkrjd.us-east-1.rds.amazonaws.com; UserID = admin;Password = 12345678;Database = web_scraper");

        conn.Open();

        var query = "SELECT Company_name, DATE_FORMAT(Filing_Date, '%M %d %Y'), Form_Type, SEC_number FROM CompanyFilings JOIN Company USING(CIK) WHERE CIK = " + CIK + " OR Company_name LIKE '%" + Company + "%' AND Form_Type LIKE '%" + FormType + "%'";

        MySqlCommand command = new MySqlCommand(query, conn);

        MySqlDataReader reader = command.ExecuteReader();
        try
        {
            while (reader.Read())
            {
                SearchResponse response = new SearchResponse();
                response.CompanyName = (string)reader["Company_name"];
                response.FilingDate = (string)reader["DATE_FORMAT(Filing_Date, '%M %d %Y')"];
                response.FormType = (string)reader["Form_Type"];
                response.SEC = (string)reader["SEC_number"];
                list.Add(response);
            }

        }
        finally
        {
            reader.Close();
            conn.Close();
        }

        return list;

    }
}

