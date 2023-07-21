using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Ecommerce.Data;
using Microsoft.AspNetCore.Http;
using AuthJwtDbApi.Models;
using Ecommerce.Models;
using Microsoft.JSInterop;
using Firebase.Database;
using Firebase.Database.Query;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using CurrieTechnologies.Razor.SweetAlert2;



// string credentialPath =@"D:\myclonings\EcommerceUsingBlazorDotNet\login\eshop-7f58a-firebase-adminsdk-kbagk-63176e3c5e.json";
// Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",credentialPath);


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSweetAlert2();

//builder.Services.AddScoped<ICartService, CartService>();
// Add this line

//firebase config
// builder.Services.AddScoped<IFirebaseClient>(_ =>
// {
//     IFirebaseConfig config = new FirebaseConfig
//     {
//         AuthSecret = "K6HGOj9LIhd5PmFR1GfzmQZN1RLYo4rQuYuc6P0I",
//         BasePath = "https://eshop-7f58a-default-rtdb.firebaseio.com"
//     };

//     IFirebaseClient client = new FireSharp.FirebaseClient(config);
//     return client;
// });

// Add services to the container.
//builder.Services.AddScoped<RedirectUrls>(); // register the service
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
// builder.Services.AddSingleton<IJSRuntime, JSRuntime>();

builder.Services.AddCors();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<HttpClient>();
builder.Services.AddHttpClient();


builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(new HttpClient());
var app = builder.Build();


// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
//     endpoints.MapFallbackToFile("index.html");
// });

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Products}/{action=/home/landing}/{id?}");

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Products}/{action=/home/details}/{id?}");


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
