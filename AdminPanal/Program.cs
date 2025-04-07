using AdminPanal.Data;
using AdminPanal.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"))
);


builder.Services.AddHttpClient();
// Add Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Require confirmed account for login
    options.Password.RequireDigit = true; // Password must include a digit
    options.Password.RequireLowercase = true; // Password must include a lowercase letter
    options.Password.RequireUppercase = true; // Password must include an uppercase letter
    options.Password.RequireNonAlphanumeric = true; // Password must include a special character
    options.Password.RequiredLength = 8; // Minimum password length
})
.AddEntityFrameworkStores<AppDbContext>();

// Register custom services
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Enable HTTP Strict Transport Security (HSTS)
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // This serves files from wwwroot by default

// Serve files from the D:\Images directory
app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication(); // Add this line to enable authentication
app.UseAuthorization();

// Map Razor Pages for Identity (e.g., Login, Register)
app.MapRazorPages();

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();