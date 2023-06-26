using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Ecommerce.Models;

public class CartService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public CartService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<CartItem> AddToCart(int productId, int productVendorId)
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "token");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(token);
        var userId = decodedToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

        var url = $"https://localhost:7044/api/cart/{userId}";

        var cartItem = new CartItem
        {
            ProductId = productId,
            ProductVendorId = productVendorId
        };

        var response = await _httpClient.PostAsJsonAsync(url, cartItem);
        response.EnsureSuccessStatusCode();

        var addedCartItem = await response.Content.ReadAsAsync<CartItem>();
        return addedCartItem;
    }
}
