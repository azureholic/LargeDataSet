using Microsoft.AspNetCore.Mvc;

namespace LargeDataSet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Download : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Download(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    [HttpGet]
    [Route("{categoryId}")]
    
    public IActionResult Get(int categoryId)
    {
        var requestId = Guid.NewGuid().ToString();

        var query = new SqlQuery(HttpContext.RequestServices);

        string queryText = "SELECT ProductId, Name, ProductNumber, Color FROM [SalesLT].[Product]" +
                            "WHERE ProductCategoryID = @product_category_id";

        var json = query.Execute(queryText, categoryId);

        var blob = new Blob(_configuration, _environment);
        var sas = blob.UpLoad(requestId, json, true);

        //return a SAS token to the client, so it can download the file
        return Ok(sas);
    }

}
