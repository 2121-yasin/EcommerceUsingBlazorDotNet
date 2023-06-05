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
using System.Net;
using System.Text.Json;
// using System.Security.Permissions;

namespace Auth.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Register()
    {
        return View();
    }

    public async Task<IActionResult> RegisterUser(UserDto user)
    {
        if (!ModelState.IsValid)
        {
            return View("Register", user);
        }

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

                    string? clientId = null;
                    if (user.Role == "Vendor")
                    {
                        clientId = "ff84a00f-99ab-4f81-9f52-26df485a9dcf";
                        // clientId = "b6782e13-5669-4156-82a8-1850883214e4";
                    }
                    else if (user.Role == "User")
                    {
                        clientId = "a0d0b3a2-efa4-47ca-b193-45bdbd950f3a";
                    }

                    if (clientId == null)
                    {
                        TempData["Error"] = "Select a valid Role.";

                        return View("Register", user);
                    }

                    return Redirect($"~/Home/Login/RedirectUrl?clientId={clientId}");
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

    public async Task<IActionResult> Login(string clientId, string userEmail)
    {
        // ViewBag.clientId = HttpUtility.UrlEncode(clientId);

        ViewBag.clientId = clientId;



        using (var httpClientHandler = new HttpClientHandler())
        {
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            using (var client = new HttpClient(httpClientHandler))
            {
                var result = await client.GetAsync($"https://localhost:7240/api/UserInfo/RedirectUrl?clientId={clientId}");
                if (result.IsSuccessStatusCode)
                {
                    ViewBag.redirect = await result.Content.ReadAsStringAsync();

                    // if (!string.IsNullOrEmpty(user))
                    // {
                    //     var userModel = System.Text.Json.JsonSerializer.Deserialize<UserLoginDto>(user);
                    //     return View("Login", userModel);
                    // }
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        var userModel = new UserLoginDto { Email = userEmail };
                        return View("Login", userModel);
                    }
                }
            }
        }

        return View();
    }


    public async Task<IActionResult> LoginUser(UserLoginDto user, string redirect, string clientId)
    {
        string token;

        // if (!ModelState.IsValid)
        // {
        //     return View("Login", user);
        // }

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
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var responseObj = JsonConvert.DeserializeObject<dynamic>(responseData);

                        token = responseObj.token.ToString();

                        // token = await response.Content.ReadAsStringAsync();  // ClaimType.Role = http://schemas.microsoft.com/ws/2008/06/identity/claims/role
                        // if (token == "Invalid credentials")
                        // {
                        //     ViewBag.Message = "Incorrect UserId or Password!";
                        //     return Redirect("~/Home/Login");
                        // }

                        // HttpContext.Session.SetString("JWToken", token);
                        // HttpContext.Session.SetString("Email", user.Email);

                        // Setting role in HttpContext.Session to use for conditionally rendering HTML elements.
                        var handler = new JwtSecurityTokenHandler();
                        var tokenForRole = handler.ReadJwtToken(token);
                        // var roleClaim = tokenForRole.Claims.FirstOrDefault(c => c.Type == "role");
                        var roleClaim = tokenForRole.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                        if (roleClaim != null)
                        {
                            HttpContext.Session.SetString("UserRole", roleClaim);
                        }

                        if (string.IsNullOrEmpty(redirect))
                        {
                            // Redirect to the login view with clientId as a query string parameter
                            return RedirectToAction("Login", new { clientId });
                        }
                        else
                        {
                            // Redirect to the specified redirect URL with the token as a query string parameter
                            return Redirect(redirect + "?token=" + token);
                        }

                        //return Redirect("~/Home/Privacy"); //redirect url
                        //return Redirect(redirect + "?token=" + token);
                        // return Redirect(HttpUtility.UrlDecode(redirect) + "?token=" + token);
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var responseObj = JsonConvert.DeserializeObject<dynamic>(responseData);
                        var errorMessage = responseObj.message.ToString();

                        ModelState.AddModelError("Email", errorMessage);
                        TempData["Error"] = errorMessage;
                        // return View("Login", user);

                        // return RedirectToAction("Login", new { clientId, user = System.Text.Json.JsonSerializer.Serialize(user) });
                        return RedirectToAction("Login", new { clientId, userEmail = user.Email });
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var responseObj = JsonConvert.DeserializeObject<dynamic>(responseData);
                        var errorMessage = responseObj.message.ToString();

                        if (errorMessage == "Incorrect password")
                        {
                            ModelState.AddModelError("Password", errorMessage);
                        }

                        TempData["Error"] = errorMessage;
                        // return View("Login", user);

                        // return RedirectToAction("Login", new { clientId, user = System.Text.Json.JsonSerializer.Serialize(user) });
                        return RedirectToAction("Login", new { clientId, userEmail = user.Email });
                    }
                    else
                    {
                        var errorMessage = "An error occurred during login.";
                        TempData["Error"] = errorMessage;
                        // return Redirect("~/Home/Login");
                        return RedirectToAction("Login", new { clientId });
                    }
                }
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
