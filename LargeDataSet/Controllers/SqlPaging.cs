using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace LargeDataSet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SqlPaging : ControllerBase
{
    [HttpGet]
    [Route("{categoryId}")]
    public async Task<IActionResult> Get(int categoryId)
    {
        int page = 1;
        var requestId = Guid.NewGuid().ToString();

        if (Request.Query.ContainsKey("page"))
        {
            page = int.Parse(Request.Query["page"].ToString());
        }

        var query = new SqlQuery(HttpContext.RequestServices);

        string totalRecords = "SELECT COUNT(*) as Total FROM [SalesLT].[Product]" +
                            "WHERE ProductCategoryID = @product_category_id";

        var total = query.Execute(totalRecords, categoryId);

        var parsedTotal = double.Parse(JArray.Parse(total)[0]["Total"].ToString());

        string queryText = "SELECT ProductId, Name, ProductNumber, Color FROM [SalesLT].[Product] " +
                            "WHERE ProductCategoryID = @product_category_id " +
                            "ORDER BY ProductId " +
                            $"OFFSET {(page - 1) * 5} ROWS " +
                            "FETCH NEXT 5 ROWS ONLY";

        var json = query.Execute(queryText, categoryId);

            
        DataTable table = JsonConvert.DeserializeObject<DataTable>(json);

        var pageCount = (int)Math.Ceiling((double)(parsedTotal / 5));

        var pageResult = new PageResult();
        pageResult.PageNumber = page;
        pageResult.PageSize = 5;
        pageResult.PageCount = pageCount;
        pageResult.PageData = table;

        string currentUri = Request.Host.Value;

        if (page < pageCount)
           pageResult.NextPage = $"https://{currentUri}/api/sqlpaging/{categoryId}?page={page + 1}";


        return Ok(JsonConvert.SerializeObject(pageResult));

        

    }
}
