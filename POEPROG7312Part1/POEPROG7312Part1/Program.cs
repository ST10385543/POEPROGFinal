using POEPROG7312Part1.Services;
using POEPROG7312Part1.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var mongoConnection = builder.Configuration.GetConnectionString("MongoDbConnection");


builder.Services.AddSingleton(new POEPROG7312Part1.Services.MongoDbContext(mongoConnection));


builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;                 
    options.Cookie.IsEssential = true;              
});

builder.Services.AddSingleton<UserCacheService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var users = mongoContext.User;

    var existingAdmin = users.Find(u => u.Role == "Admin").FirstOrDefault();

    if (existingAdmin == null)
    {
        users.InsertOne(new User
        {
            Username = "Kayden",
            Email = "KaydenAdmin@gmail.com",
            Password = "Kayden@29", 
            Role = "Admin"
        });

        Console.WriteLine(" Default admin created: Kayden / Kayden@29");
    }
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
