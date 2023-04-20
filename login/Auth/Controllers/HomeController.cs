using System.Diagnostics;
using System.Text;
using Auth.DTOs;
using Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Authorization;
// using System.Security.Permissions;

namespace Auth.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // public async Task<IActionResult> Landing()
    // {
    //     var products = new ProductsController().GetProducts();
    //     return View(products);
    // }

    public IActionResult Register()
    {
        return View();
    }

    public async Task<IActionResult> RegisterUser(UserDto user)
    {
        using (var httpClientHandler = new HttpClientHandler())
        {
            // httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // Below used as not all implementations support this callback, and some throw PlatformNotSupportedException.
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
            using (var httpClient = new HttpClient(httpClientHandler))
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"); // System.Security.Permissions NuGet package required for this to work. Commented method below works without NuGet package.
                using (var response = await httpClient.PostAsync("https://localhost:7240/api/UserInfo", stringContent))
                // JsonContent jsonContent = JsonContent.Create(user);
                // using (var response = await httpClient.PostAsync("https://localhost:7040/api/userinfo", jsonContent))

                {
                    var userExists = await response.Content.ReadAsStringAsync();

                    if (userExists == "User already exists")

                    {

                        ModelState.AddModelError("Email", "Email address is already registered");
                        TempData["Error"] = "This email address is already in use";

                        return View("Register", user);
                    }
                    return Redirect("~/Home/Login");
                }
            }
        }
    }


/*
public async Task<IActionResult> Login(string clientId)
    {
        //ViewBag.redirect = redirect;  //
        ViewBag.clientId = HttpUtility.UrlEncode(clientId);
        //StringContent stringContent = new StringContent(JsonConvert.SerializeObject(), Encoding.UTF8, "application/json"); // System.Security.Permissions NuGet package required for this to work. Commented method below works without NuGet package.

        //using (var response = await HttpClient.GetAsync("https://localhost:7240/api/UserInfo/RedirectUrl?clientId=" + clientId))
         using (var httpClientHandler = new HttpClientHandler())
        {
        httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
        var response = string.Empty;
        using (var client = new HttpClient(httpClientHandler))
        {
            HttpResponseMessage result = await client.GetAsync("https://localhost:7240/api/UserInfo/RedirectUrl?clientId=" + clientId);
            if (result.IsSuccessStatusCode)
            {
                ViewBag.redirect = await result.Content.ReadAsStringAsync();
            }
        }
        }
        return View();
    }
*/


    // public IActionResult Login(string redirect)
    // {
    //     //ViewBag.redirect = redirect;  //
    //     ViewBag.redirect = HttpUtility.UrlEncode(redirect);
    //     return View();
    // }

[AllowAnonymous]
public async Task<IActionResult> Login(string clientId)
{
    ViewBag.clientId = HttpUtility.UrlEncode(clientId);

    using (var httpClientHandler = new HttpClientHandler())
    {
        httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        using (var client = new HttpClient(httpClientHandler))
        {
            var result = await client.GetAsync($"https://localhost:7240/api/UserInfo/RedirectUrl?clientId={clientId}");
            if (result.IsSuccessStatusCode)
            {
                ViewBag.redirect = await result.Content.ReadAsStringAsync();
            }
        }
    }
    
    return View();
}


    public async Task<IActionResult> LoginUser(UserDto user, string redirect)
    {
        string token;

        using (var httpClientHandler = new HttpClientHandler())
        {
            // httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // Below used as not all implementations support this callback, and some throw PlatformNotSupportedException.
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
            using (var httpClient = new HttpClient(httpClientHandler))
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"); // System.Security.Permissions NuGet package required for this to work. Commented method below works without NuGet package.
                using (var response = await httpClient.PostAsync("https://localhost:7240/api/token", stringContent))
                // JsonContent jsonContent = JsonContent.Create(user);
                // using (var response = await httpClient.PostAsync("https://localhost:7040/api/token", jsonContent))
                {
                    token = await response.Content.ReadAsStringAsync();  // ClaimType.Role = http://schemas.microsoft.com/ws/2008/06/identity/claims/role
                    if (token == "Invalid credentials")
                    {
                        ViewBag.Message = "Incorrect UserId or Password!";
                        return Redirect("~/Home/Login");
                    }
                    HttpContext.Session.SetString("JWToken", token);

                    HttpContext.Session.SetString("Email", user.Email);

                    // Setting role in HttpContext.Session to use for conditionally rendering HTML elements.


                    var handler = new JwtSecurityTokenHandler();
                    var tokenForRole = handler.ReadJwtToken(token);
                    // var roleClaim = tokenForRole.Claims.FirstOrDefault(c => c.Type == "role");
                    var roleClaim = tokenForRole.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    if (roleClaim != null)
                    {
                        HttpContext.Session.SetString("UserRole", roleClaim);
                    }

                }

                //return Redirect("~/Home/Privacy"); //redirect url
                //return Redirect(redirect + "?token=" + token);
                return Redirect(HttpUtility.UrlDecode(redirect) + "?token=" + token);

            }
        }

    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // Clear token
        return Redirect("~/Home/Login");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
