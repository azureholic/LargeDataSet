using Azure.Core;
using Azure.Core.Diagnostics;
using Azure.Identity;
using LargeDataSet;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);


// managed identity credentials used to access the database
// Run this in your Db to create the user and grant permissions:
//------------------------
//  CREATE USER [managed-identity-name] FROM EXTERNAL PROVIDER;
//  ALTER ROLE db_datareader ADD MEMBER [managed-identity-name] ;
//------------------------

//diagnostics for troubleshooting
using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger();



builder.Services.AddTransient<IDbConnection>(provider =>
{
    SqlConnection conn = new SqlConnection();
    conn.ConnectionString = builder.Configuration["Sql:ConnectionString"];
    string token = ManagedIdentityCredentialHelper.GetCredential(builder.Environment)
            .GetToken(new TokenRequestContext(new[] { TokenContext.SqlContext }), new CancellationToken()).Token;

    conn.AccessToken = token;
    return conn;
});


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
