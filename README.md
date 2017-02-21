# IdentityServer4-with-ASP.Net-Core-1.1

- [Setup IdentityServer](#setup-identityserver)
 - [Project Directory](#project-directory)
 - [Nuget Package Manager](#nuget-package-manager)
 - [Connectionstring to DB](#connectionstring-to-db)
 - [Startup](#startup)
- [Using EntityFramework Core for configuration data](#using-entityframework-core-for-configuration-data)

##Setup IdentityServer
 [*Based on this repository.*](https://github.com/petervanhemert/ASP.NET-CORE-1.1-Development-with-SSL/blob/master/README.md)
 
###Project Directory
 
 - In Data/Migrations Add Folders
  - [x] Configuration
  - Identity
  - PersistedGrants
 
 Put files of ApplicationDbContext Migration to Identity.
 ```
 ApplicationDbContextModelSnapshot.cs
 00000000000000_CreateIdentitySchema.cs
 ```
 
###Nuget Package Manager
 
 Update Packages
 
 Add to project:
 ```
 IdentityServer4
 IdentityServer4.AspNetIdentity
 IdentityServer4.EntityFramework
 ```
 
###Connectionstring to DB

 Open appsettings.json
 
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
###Startup

Modify your Startup.cs file to look like this:
```
public class Startup
{
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

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                DisplayName = "Google",
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = Configuration["GoogleIdentityProvider:ClientId"],
                ClientSecret = Configuration["GoogleIdentityProvider:ClientSecret"]
        });

            app.UseFacebookAuthentication(new FacebookOptions()
            {
                AppId = Configuration["FacebookIdentityProvider:AppId"],
                AppSecret = Configuration["FacebookIdentityProvider:AppSecret"]
            });

            app.UseTwitterAuthentication(new TwitterOptions()
            {
                ConsumerKey = Configuration["TwitterIdentityProvider:ConsumerKey"],
                ConsumerSecret = Configuration["TwitterIdentityProvider:ConsumerSecret"]
            });

            app.UseMicrosoftAccountAuthentication(new MicrosoftAccountOptions()
            {
                ClientId = Configuration["MicrosoftIdentityProvider:ClientId"],
                ClientSecret = Configuration["MicrosoftIdentityProvider:ClientSecret"]
            });

            app.UseLinkedInAuthentication(new LinkedInOptions()
            {
                ClientId = Configuration["LinkedinIdentityProvider:ClientId"],
                ClientSecret = Configuration["LinkedinIdentityProvider:ClientSecret"]
            });

            app.UseInstagramAuthentication(new InstagramAuthenticationOptions()
            {
                ClientId = Configuration["InstagramIdentityProvider:ClientId"],
                ClientSecret = Configuration["InstagramIdentityProvider:ClientSecret"]
            });

                (new InstagramOptions()
            {
                ClientId = Configuration["LinkedinIdentityProvider:ClientId"],
                ClientSecret = Configuration["LinkedinIdentityProvider:ClientSecret"]
            });

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
}
```

  
## Using EntityFramework Core for configuration data



```
PM> Add-Migration -Context ApplicationDbContext -OutputDir <ProjectName>\Data\Migrations\<DestinationFolder> SetMigrationName

PM> Update-Database -Context ApplicationDbContext
```














diff --git a/README.md b/README.md
index 88fcf69..8614873 100644
--- a/README.md
+++ b/README.md
@@ -28,6 +28,7 @@ All the hidden and not hidden features of Git and GitHub. This cheat sheet was i
 - [Merged Branches](#merged-branches)
 - [Quick Licensing](#quick-licensing)
 - [TODO Lists](#todo-lists)
+- [Relative Links](#relative-links)
 - [.gitconfig Recommendations](#gitconfig-recommendations)
     - [Aliases](#aliases)
     - [Auto-correct](#auto-correct)
@@ -381,6 +382,19 @@ When they are clicked, they will be updated in the pure Markdown:
 - [ ] Sleep

(...)
