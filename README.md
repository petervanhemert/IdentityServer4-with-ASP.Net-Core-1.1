# IdentityServer4-with-ASP.Net-Core-1.1

- [Setup IdentityServer](#setup-identityserver)
 - [Project Directory](#project-directory)
 - [Nuget Package Manager](#nuget-package-manager)
 - [Connectionstring to DB](#connectionstring-to-db)
 - [Configure IdentityServer and the stores](#configure-identityserver-and-the-stores)
   - [Startup](#startup)
  
- [Creating an MVC client](#creating-an-mvc-client)
 - [Configuring the stores](#configuring-the-stores)
 - [ConfigurationStore](#configurationstore)
 - [OperationalStore](#operationalstore)

##Setup IdentityServer
 [*Based on this repository.*](https://github.com/petervanhemert/ASP.NET-CORE-1.1-Development-with-SSL/blob/master/README.md)
 
###Project Directory
 
 - In Data/Migrations Add Folders
  - [x] Configuration
  - [x] Identity
  - [x] PersistedGrants
 
 Move the files of ApplicationDbContext Migration to Identity.
 ```ruby
 ApplicationDbContextModelSnapshot.cs
 00000000000000_CreateIdentitySchema.cs
 ```
 
###Nuget Package Manager
 
 Update Packages
 
 Add to project:
 ```ruby
 IdentityServer4
 IdentityServer4.AspNetIdentity
 IdentityServer4.EntityFramework
 ```
 
###Connectionstring to DB

 Open appsettings.json
 
 Change DefaultConnection in ConnectionStrings in the file appsettings.json
  ```ruby
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
###Configure IdentityServer and the stores  
  
####Startup

As before, IdentityServer needs to be configured in both ConfigureServices and in Configure in Startup.cs.

#####ConfigureServices

Modify your ConfigureServices in the Startup.cs to look like this:
```ruby
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IProfileService, IdentityWithAdditionalClaimsProfileService>();

            // add for identityserver
            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddOperationalStore(
                    builder => builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), options =>                  options.MigrationsAssembly(migrationsAssembly)))
                .AddConfigurationStore(
                    builder => builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), options => options.MigrationsAssembly(migrationsAssembly)))
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<IdentityWithAdditionalClaimsProfileService>();
        }
```

#####Configure

This shows both the template code generated for ASP.NET Identity, plus the additions needed for IdentityServer (just after UseIdentity). It’s important when using ASP.NET Identity that IdentityServer be registered after ASP.NET Identity in the pipeline because IdentityServer is relying upon the authentication cookie that ASP.NET Identity creates and manages.

Modify your Configure in the Startup.cs to look like this:
```ruby
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                // Browser Link is not compatible with Kestrel 1.1.0
                // For details on enabling Browser Link, see https://go.microsoft.com/fwlink/?linkid=840936
                // app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
           
            app.UseIdentity();
            
            app.UseIdentityServer();

            app.UseStaticFiles();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
```

  


#####Adding migrations

```ruby
PM> Add-Migration SetMigrationName -c ApplicationDbContext -o Data/Migrations/Identity

PM> Add-Migration SetMigrationName -c PersistedGrantDbContext -o Data/Migrations/PersistedGrants

PM> Add-Migration SetMigrationName -c ConfigurationDbContext -o Data/Migrations/Configuration

PM> Update-Database -c ApplicationDbContext

PM> Update-Database -c PersistedGrantDbContext

PM> Update-Database -c ConfigurationDbContext
```

     Add-Migration What is what?

     `-c ApplicationDbContext`
     Is the DbContext we are targeting

     `-o <ProjectName>\Data\Migrations\<DestinationFolder>`
     Is the file destination

     `SetMigrationName`
     Is the name we give of the Migration

     Update-Database What is what?

     `-c ApplicationDbContext`
     Is the DbContext we are targeting


## Creating an MVC client

Add an MVC application to your solution. Use the ASP.NET Core “Web Application” template for that. No authentication.


### Client Nuget Package Manager 
 
 Update Packages
 
 Add to project:
 ```ruby
Microsoft.AspNetCore.Authentication.Cookies
Microsoft.AspNetCore.Authentication.OpenIdConnect
 ```
Next add both middlewares to your pipeline - the cookies one is simple:
```ruby
app.UseCookieAuthentication(new CookieAuthenticationOptions
{
    AuthenticationScheme = "Cookies"
});
```
The OpenID Connect middleware needs slightly more configuration. You point it to your IdentityServer, specify a client ID and tell it which middleware will do the local signin (namely the cookies middleware). As well, we’ve turned off the JWT claim type mapping to allow well-known claims (e.g. ‘sub’ and ‘idp’) to flow through unmolested. This “clearing” of the claim type mappings must be done before the call to UseOpenIdConnectAuthentication():
```ruby
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
{
    AuthenticationScheme = "oidc",
    SignInScheme = "Cookies",

    Authority = "http://localhost:5000",
    RequireHttpsMetadata = false,

    ClientId = "mvc",
    SaveTokens = true
});
```

Both middlewares should be added before the MVC in the pipeline.

The last step is to trigger the authentication handshake. For that go to the home controller and add the `[Authorize]` on one of the actions. Also modify the view of that action to display the claims of the user, e.g.:


```ruby
<dl>
    @foreach (var claim in User.Claims)
    {
        <dt>@claim.Type</dt>
        <dd>@claim.Value</dd>
    }
</dl>
```





