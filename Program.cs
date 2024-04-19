using LabWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AspNet.Security.OAuth.GitHub; // Required for GitHub authentication
using System.Security.Claims; // Required for adding claims

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register the ApplicationDbContext with the connection string from the appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add ASP.NET Core Identity using the default UI and Entity Framework stores with our ApplicationDbContext.
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configure GitHub authentication
builder.Services.AddAuthentication().AddGitHub(githubOptions =>
{
    githubOptions.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
    githubOptions.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
    githubOptions.CallbackPath = "/signin-github";
    githubOptions.Scope.Add("read:user");
    githubOptions.Events.OnCreatingTicket = context =>
    {
        if (context.AccessToken is { })
        {
            context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
        }
        return Task.CompletedTask;
    };
});

// Add framework services.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // This is necessary to support Razor Pages alongside MVC, which is used by Identity.

// Adds a developer exception page to display detailed information about exceptions during development.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Use the migration endpoint during development to apply any pending migrations.
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Use a custom error handler in production.
    app.UseHsts(); // Enforce HTTPS in production.
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // This enables authentication capabilities.
app.UseAuthorization();

// Map controller routes.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages, which are used by ASP.NET Core Identity for login, logout, etc.
app.MapRazorPages();

app.Run();