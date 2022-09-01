# aspnetcore-dapper-company-api
ASP.NET Core Web API using Dapper

## Flow
DB - Create Connection -> Context -> Repository -> Controller -> Return model as Json

## Setup
-  Create CompanyDb by running `initCompanyDB.sql` to your DB server.
-  Dont forget to changes the value of `SqlConnection` in `ConnectionStrings` in `appsetings.json` to your local Db's connection string.
 