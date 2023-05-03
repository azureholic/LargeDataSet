using Newtonsoft.Json;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.Json;

namespace LargeDataSet;

public class SqlQuery
{
    private readonly IServiceProvider _provider;

    public SqlQuery(IServiceProvider provider)
    {
        _provider = provider;
    }

    public string Execute(string query, int productCategory)
    {
        using (var dbConnection = _provider.GetRequiredService<IDbConnection>())
        {
            dbConnection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbConnection as SqlConnection;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@product_category_id", productCategory);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = cmd;
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            string json = JsonConvert.SerializeObject(dataSet.Tables[0]);

            return json;
        }

    }
}
