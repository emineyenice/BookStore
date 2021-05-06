# First set src/Infrastructure as default project in Package Manager Console

#Migrations
 Add-Migration [-Name] <String> [-OutputDir <String>] [-Context <String>] [-Project <String>] [-Startup
    Project <String>] [-Namespace <String>] [-Args <String>] [<CommonParameters>]
    
Add-Migration InitialApp -OutputDir Data/Migrations -Context Infrastructure.Data.AppDbContext -StartupProject Web
update-database -context AppDbContext

Add-Migration InitialIdentity -OutputDir Identity/Migrations -Context Infrastructure.Identity.AppIdentityDbContext -StartupProject Web
update-database -context AppIdentityDbContext


Add-Migration IOrderBasketAdded -OutputDir Data/Migrations -Context Infrastructure.Data.AppDbContext -StartupProject Web

remove-migration -context AppDbContext

# AntiforgeryToken JavaScript Injection Creating AntiforgeryToken in Viewa
https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-5.0#javascript

