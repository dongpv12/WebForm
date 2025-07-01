using Microsoft.AspNetCore.ResponseCompression;
using NLog;
using NLog.Web;
using System.IO.Compression;
using WebForm;
using WebForm.Common;

string ConfigFolder = "";
#if DEBUG
ConfigFolder = "Config";
#else
            ConfigFolder = "../Config";
#endif

var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigFolder, "appsettings.json");

// Cấu hình đọc cấu hình từ file appsettings.json
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile(path, optional: true, reloadOnChange: true)
    .Build();


var path_log = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ConfigFolder));
var _full_path_nlog = Path.Combine(path_log, "nlog.config");

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile(_full_path_nlog);
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseNLog();

builder.Services.AddLogging(builder =>
{
    builder.AddConfiguration(configuration.GetSection("Logging"))
           .AddConsole()
           .AddDebug();
});

WebForm.Common.ConfigInfo.wwwRootPath = builder.Environment.WebRootPath;
WebForm.Common.ConfigInfo.ContentRootPath = builder.Environment.ContentRootPath;

WebForm.Common.ConfigInfo.GetConfig(configuration);

DataMemory.LoadDbMem();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => 
    {
        // Giữ nguyên định dạng thuộc tính thay vì chuyển sang camelCase
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });


// muốn dùng session thì set context = false
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "WebLucy";
    options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToDouble(60));
    options.Cookie.HttpOnly = true;

    // Make the session cookie essential
    options.Cookie.IsEssential = true;
}); // Additional

//nén dữ liệu
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// nén dữ liệu
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.EnableForHttps = true;
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {
                    "application/xhtml+xml",
                    "application/atom+xml",
                    "image/svg+xml",
                });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseSession(); // Additional
app.UseRouting();

// nén dữ liệu
app.UseResponseCompression();
app.UseCookiePolicy();

app.UseSession(); // Additional

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


var _HostUrl = configuration["AppUrls"] ?? "http://*:9000";
WebForm.Common.Logger.Log.Info("Start webform server port " + _HostUrl);
Console.WriteLine("HostUrl " + _HostUrl);

app.Urls.Add(_HostUrl);
app.Run();
