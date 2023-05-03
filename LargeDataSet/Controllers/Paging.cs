using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace LargeDataSet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class Paging : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Paging(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    [HttpGet]
    [Route("{categoryId}")]
    public async Task<IActionResult> Get(int categoryId)
    {
        if (!Request.Query.ContainsKey("req"))
        {
            var requestId = Guid.NewGuid().ToString();

            var query = new SqlQuery(HttpContext.RequestServices);

            string queryText = "SELECT ProductId, Name, ProductNumber, Color FROM [SalesLT].[Product]" +
                                "WHERE ProductCategoryID = @product_category_id";

            var json = query.Execute(queryText, categoryId);

            var blob = new Blob(_configuration, _environment);
            blob.UpLoad(requestId, json, false);

            DataTable table = JsonConvert.DeserializeObject<DataTable>(json);

            var pageCount = (int)Math.Ceiling((double)((double)table.Rows.Count / 5));


            var pageResult = new PageResult();
            pageResult.PageNumber = 1;
            pageResult.PageSize = 5;
            pageResult.PageCount = pageCount;
            pageResult.PageData = table.Select().Skip(0).Take(5).CopyToDataTable();

            string currentUri = Request.Host.Value;


            pageResult.NextPage = $"https://{currentUri}/api/paging/{categoryId}?req={requestId}&page=2";

            return Ok(JsonConvert.SerializeObject(pageResult));

        }
        else
        {
            var requestId = Request.Query["req"].ToString();
            var page = int.Parse(Request.Query["page"].ToString());
            var blob = new Blob(_configuration, _environment);
            var json = await blob.Download(requestId);
            DataTable table = JsonConvert.DeserializeObject<DataTable>(json);

            var pageCount = (int)Math.Ceiling((double)((double)table.Rows.Count / 5));


            var pageResult = new PageResult();
            pageResult.PageNumber = page;
            pageResult.PageSize = 5;
            pageResult.PageCount = pageCount;
            pageResult.PageData = table.Select().Skip((page - 1) * 5).Take(5).CopyToDataTable();
            string currentUri = Request.Host.Value;
            if (page < pageResult.PageCount)
            {
                pageResult.NextPage = $"https://{currentUri}/api/paging/{categoryId}?req={requestId}&page={page + 1}";
            }
            return Ok(JsonConvert.SerializeObject(pageResult));
        }

    }
}
