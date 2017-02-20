# IdentityServer4-with-ASP.Net-Core-1.1

- [Setup IdentityServer](#setup-identityserver)
- [Debugging in IIS Project(Kestrel)](#debugging-in-project)

##Setup IdentityServer
 [*Based on this repository.*](https://github.com/petervanhemert/ASP.NET-CORE-1.1-Development-with-SSL/blob/master/README.md)
 
 Project Directory.
 
 In Data/Migrations Add Folders,
 - Configuration
 - Identity
 - PersistedGrants
 
 Put files of ApplicationDbContext Migration to Identity.
 ```
 ApplicationDbContextModelSnapshot.cs
 00000000000000_CreateIdentitySchema.cs
 ```
 
 Nuget Package Manager
 
 Update Packages
 
 Add to project:
 ```
 IdentityServer4
 IdentityServer4.AspNetIdentity
 IdentityServer4.EntityFramework
 ```
 Change DefaultConnection in ConnectionStrings in the file appsettings.json
  ```
 {
  "ConnectionStrings": {
    "DefaultConnection": "Server=<Server name>;Database=<Database name>;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
  ```
  
  
## Debugging in IIS Express



```
PM> Add-Migration -Context ApplicationDbContext -OutputDir <ProjectName>\Data\Migrations\<DestinationFolder> SetMigrationName

PM> Update-Database -Context ApplicationDbContext
```
