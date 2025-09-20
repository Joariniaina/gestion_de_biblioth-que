using LibraryApp.Web.Repositories;
using LibraryApp.Web.Services;
using MongoDB.Driver;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Configuration MVC --------------------
builder.Services.AddControllersWithViews();

// -------------------- MongoDB --------------------
var mongoConnection = builder.Configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["MongoDbDatabase"] ?? "LibraryDb";

var mongoClient = new MongoClient(mongoConnection);
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabaseName);
});

// -------------------- Repositories --------------------
builder.Services.AddScoped<IUserRepository, MongoUserRepository>();
builder.Services.AddScoped<IBookRepository, MongoBookRepository>();
builder.Services.AddScoped<ILoanRepository, MongoLoanRepository>(); // <-- ajouté

// -------------------- Redis --------------------
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
var redis = ConnectionMultiplexer.Connect(redisConnection);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddScoped<RedisCacheService>();

// -------------------- Services --------------------
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<LoanService>(); // <-- ajouté

builder.Services.AddHttpContextAccessor(); // <-- AJOUTÉ

// -------------------- Session --------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// -------------------- Middleware --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// -------------------- Route par défaut --------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
