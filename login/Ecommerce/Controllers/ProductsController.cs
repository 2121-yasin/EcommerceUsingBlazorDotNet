using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        public static string baseUrl = "https://localhost:7044/api/products/";
        
        public async Task<IActionResult> Index()
        {
            var products = await GetProducts();
            return View(products);
        }

        public async Task<IActionResult> Landing()
        {
            var products = await GetProducts();
            return View(products);
        }

        [HttpGet]
        public async Task<List<Products>> GetProducts()
        {
            // Use the access token to call a protected web API.
        
            var url = baseUrl;
            using (var httpClientHandler = new HttpClientHandler())
            {
                // httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // Below used as not all implementations support this callback, and some throw PlatformNotSupportedException.
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                  
                    string jsonStr = await httpClient.GetStringAsync(url);

                    var res = JsonConvert.DeserializeObject<List<Products>>(jsonStr).ToList();

                    return res;
                }
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("ProdId,ProdName,Description,Price,ImageURL,StartDate,StockQty")] Products products)
        public async Task<IActionResult> Create(Products products) // Using Tag helpers(asp-for) in the views automatically binds the view to the model. So above is not required.
        {
          
            var url = baseUrl;
            using (var httpClientHandler = new HttpClientHandler())
            {
                // httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // Below used as not all implementations support this callback, and some throw PlatformNotSupportedException.
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                   

                    var stringContent = new StringContent(JsonConvert.SerializeObject(products), Encoding.UTF8, "application/json");
                    await httpClient.PostAsync(url, stringContent);

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

           
            var url = baseUrl + id;
            using (var httpClientHandler = new HttpClientHandler())
            {
                // httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // Below used as not all implementations support this callback, and some throw PlatformNotSupportedException.
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                  
                    string jsonStr = await httpClient.GetStringAsync(url);
                    var res = JsonConvert.DeserializeObject<Products>(jsonStr);

                    if (res == null)
                    {
                        return NotFound();
                    }
                    return View(res);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("ProdId,ProdName,Description,Price,ImageURL,StartDate,StockQty")] Products products)
        public async Task<IActionResult> Edit(int id, Products products) // Using Tag helpers(asp-for) in the views automatically binds the view to the model. So above is not required.
        {
            if (id != products.ProdId)
            {
                return NotFound();
            }
          
            var url = baseUrl + id;
            using (var httpClientHandler = new HttpClientHandler())
            {
                // httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // Below used as not all implementations support this callback, and some throw PlatformNotSupportedException.
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                   

                    var stringContent = new StringContent(JsonConvert.SerializeObject(products), Encoding.UTF8, "application/json");
                    await httpClient.PutAsync(url, stringContent);

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var url = baseUrl + id;
            using (var httpClientHandler = new HttpClientHandler())
            {
                // httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // Below used as not all implementations support this callback, and some throw PlatformNotSupportedException.
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                   

                    string jsonStr = await httpClient.GetStringAsync(url);
                    var res = JsonConvert.DeserializeObject<Products>(jsonStr);

                    if (res == null)
                    {
                        return NotFound();
                    }
                    return View(res);
                }
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var url = baseUrl + id;
            using (var httpClientHandler = new HttpClientHandler())
            {
                // httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // Below used as not all implementations support this callback, and some throw PlatformNotSupportedException.
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                  

                    await httpClient.DeleteAsync(url);

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var url = baseUrl + id;
            using (var httpClientHandler = new HttpClientHandler())
            {
                // httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // Below used as not all implementations support this callback, and some throw PlatformNotSupportedException.
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                
                    string jsonStr = await httpClient.GetStringAsync(url);
                    var products = JsonConvert.DeserializeObject<Products>(jsonStr);

                    if (products == null)
                    {
                        return NotFound();
                    }
                    return View(products);
                }
            }
        }

        public async Task<IActionResult> ProductDescription(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var url = baseUrl + id;
            using (var httpClientHandler = new HttpClientHandler())
            {
                // httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // Below used as not all implementations support this callback, and some throw PlatformNotSupportedException.
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; // (NOT SECURE) connect to a server with a certificate that shouldn't be validated
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                   
                    string jsonStr = await httpClient.GetStringAsync(url);
                    var products = JsonConvert.DeserializeObject<Products>(jsonStr);

                    if (products == null)
                    {
                        return NotFound();
                    }
                    return View(products);
                }
            }
        }
        
    }
}