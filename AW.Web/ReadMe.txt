== create project .NET Core in cmd ==
1. run "dotnet new webapi -o [project_name]"

== run project .NET Core in cmd ==
1. cd -> Project Path
2. run "dotnet watch run"  

== Extension Helper in VS Code ==
1. C#
2. C# Dev Kit
3. .Net Extension Pack
4. .Net Install Tool
5. Nuget Gallery
6. Prettier
7. Extension Pack By JosKreativ
8. Microsoft.AspNetCore.Mvc.NewtonsoftJs

== Models ==
1. Create Folder "Models" in project API
2. Right Click Folder "Models" -> New C# -> Class

== Install Nuget Packages for use Entity Framework (ORM)==
1. Open Nuget Gallery: CTRL + R -> Nuget Gallery
2. Install Microsoft.EntityFrameworkCore.SqlServer -> Version same with netCore Version
3. Install Microsoft.EntityFrameworkCore.Tools -> Version same with netCore Version
4. Install Microsoft.EntityFrameworkCore.Design -> Version same with netCore Version
5. Create folder "Data" in project API for file DBContext
6. Right Click Folder "Data" -> New C# -> Class -> "ApplicationDBContext"

== Migrate Models ==
1. run "dotnet ef migrations add init" -> add-migration "First Migrate" -Context AppDBContext || dotnet ef migrations add FirstMigration --context UMSDbContext --project UMS.Core --startup-project UMS.Web
2. run "dotnet ef database update" -> update-database -Context AppDBContext || dotnet ef database update --context UMSDbContext --project UMS.Core --startup-project UMS.Web

== Using Identity JWT ==
1. Open Nuget Gallery: CTRL + R -> Nuget Gallery
2. Install Microsoft.EntityFrameworkCore.SqlServer -> Version same with netCore Version
3. Install Microsoft.Extensions.Identity.Core
4. Install Microsoft.AspNetCore.Identity.EntityFrameworkCore
5. Install Microsoft.AspNetCore.Authentication.JwtBearer

== Migrate Identity User | JWT ==
1. dotnet ef migrations add Identity
2. dotnet ef database update

== Versioning Controller ==
1. dotnet add package Microsoft.AspNetCore.Mvc.Versioning

== Filtering ==
{
    "Page": 1,
    "PageSize": 100,
    "Includes": "",
    "FilterParams": [
        {
            "Key": "Description",
            "Option": "startwith",
            "Value": "a"
        },
        {
            "Key": "LogInc",
            "Option": "eq",
            "Value": 1
        },
        {
            "Key": "LogDate",
            "Option": "orEqual",
            "Value": "2024-06-18T09:01:45.2608225,2024-06-18T09:02:20.4805277"
        },
        {
            "Key": "ID",
            "Option": "orEqual",
            "Value": "1,2,4"
        }
    ],
    "SortParams": [
        {
            "Column": "ID",
            "Option": "DESC"
        },
        {
            "Column": "Description",
            "Option": "ASC"
        }
    ]
}

== Setup to use Script EF Core when publish ==
1. dotnet new tool-manifest --force
2. dotnet tool install dotnet-ef --version 7.*