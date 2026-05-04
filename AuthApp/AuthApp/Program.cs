using AuthApp.Data;
using Microsoft.EntityFrameworkCore;
using AuthApp.Models;
using AuthApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        "server=localhost;port=3307;database=authapp;user=root;password=1234;",
        new MySqlServerVersion(new Version(8, 0, 34))
    ));

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
);
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Register}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.KatoEntries.Any())
    {
        var csvPath = Path.Combine(app.Environment.ContentRootPath, "Data", "KATO_18_02_2026.csv");
        Console.WriteLine("Ищу файл: " + csvPath);
        Console.WriteLine("Файл существует: " + File.Exists(csvPath));
        if (File.Exists(csvPath))
        {
            var entries = File.ReadLines(csvPath)
                .Skip(1)
                .Select(line => line.Split(';'))
                .Where(p => p.Length >= 8)
                .Select(p => new AuthApp.Models.KatoEntry
                {
                    Code = p[0],
                    Ab = p[1],
                    Cd = p[2],
                    Ef = p[3],
                    Hij = p[4],
                    Level = p[5],
                    KazName = p[6],
                    RusName = p[7]
                });
            db.KatoEntries.AddRange(entries);
            db.SaveChanges();
        }
        else
        {
            Console.WriteLine("❌ KATO.csv не найден: " + csvPath);
        }
    }
}

app.Run();