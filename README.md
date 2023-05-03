# Large Resultset

This sample gives you a couple of options to retrieve a large dataset result from a SQL query through an API.
The API can be anything, like an Azure Function or a Web API. In this sample I use a Web API to explain the concepts.

**Prerequisites to run the sample**

* An Azure SQL Database with the sample database installed (AdventureWorks)
* An Azure Storage Account
* A managed identity (or your local account - when running local) with permissions to SQL: role: db\_datareader
* A managed identity (or your local account - when running local) with RBAC permissions to Blob Storage: Role:Blob Storage Contributor


**Setting permissions for a managed identity on SQL Server**
Make sure your SQL Server has AAD enabled
Run the following query on the database

```
CREATE USER [managed-identity-name] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [managed-identity-name];
```


**Controllers**

* Download: Executes the query once, stores the result in Blob Storage and returns a SAS token for the client to initiate the download as a file
* Paging: Executes the query once, stores the result in Blob Storage and returns a page of 5 record and an Url the fetch the next page. This approach could take a lot of memory depending on the file size. You could also consider pre-paging (storing a file per page).
* SqlPaging: Executes the query paged on SQL Server directly. No intermediate storage (but data can change during the request if database is being edited in the meantime)


Hope this helps
