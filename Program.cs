using Game2048.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register UserDataService as Singleton (shared across all requests)
builder.Services.AddSingleton<UserDataService>();

// Add session support for storing user data
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseStaticFiles(); // Serve static files from wwwroot
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapControllers();

// Default route to index.html
app.MapFallbackToFile("index.html");

Console.WriteLine("[INFO] User data will be stored in: Data/users.json");

app.Run();
