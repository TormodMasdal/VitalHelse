using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using VitalHelse.Configuration;
using VitalHelse.Data;
using VitalHelse.Models;
using VitalHelse.Services;
/*using VitalHelse.Services;*/
using Product = Stripe.Product;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();
StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AspNetUsers>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddRouting(options => { options.LowercaseUrls = true; });

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.AddScoped<TripletexService>();
builder.Services.AddScoped<TripletexSyncService>();

builder.Services.Configure<StripeOptions>(options =>
{
    options.PublishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
    options.SecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
    options.WebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
    options.Price = Environment.GetEnvironmentVariable("PRICE");
    options.Domain = Environment.GetEnvironmentVariable("DOMAIN");
});

var app = builder.Build();

// Call the database initializer
using (var services = app.Services.CreateScope())
{
    var um = services.ServiceProvider.GetRequiredService<UserManager<AspNetUsers>>();
    var rm = services.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var db = services.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    var env = services.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    
    ApplicationDbInitializer.InitializeAsync(db, um, rm);
    TestData.Initialize(db, env);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var syncService = scope.ServiceProvider.GetRequiredService<TripletexSyncService>();
    var result = await syncService.SyncProductsAsync();
    Console.WriteLine($"Tripletex Sync Completed: Added={result.added}, Updated={result.updated}, Hidden={result.hidden}");
}
app.MapControllers();

app.Run();