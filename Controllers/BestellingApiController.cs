using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Klant, Eigenaar, zaalverantwoordelijke, ober, kok")]
    public class BestellingApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BestellingApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var cartItems = string.IsNullOrEmpty(dto.CartItemsJson)
                ? new List<CartItemDto>()
                : System.Text.Json.JsonSerializer.Deserialize<List<CartItemDto>>(dto.CartItemsJson) ?? new List<CartItemDto>();

            // Strictly filter out invalid/default objects
            cartItems = cartItems.Where(ci => ci != null && ci.ProductId > 0 && ci.Aantal > 0).ToList();

            // Find if product already exists in cart
            var existing = cartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (existing != null)
            {
                existing.Aantal++;
            }
            else
            {
                cartItems.Add(new CartItemDto
                {
                    ProductId = dto.ProductId,
                    Aantal = 1
                });
            }

            // Now build the response with product info
            var cartItemsWithProduct = new List<CartItemWithProductViewModel>();
            foreach (var item in cartItems)
            {
                var product = await _unitOfWork.Producten.GetByIdWithPriceAsync(item.ProductId);
                var prijs = product?.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).FirstOrDefault()?.Prijs;
                if (product != null && prijs.HasValue)
                {
                    cartItemsWithProduct.Add(new CartItemWithProductViewModel
                    {
                        ProductId = item.ProductId,
                        Aantal = item.Aantal,
                        Naam = product.Naam,
                        Prijs = prijs.Value
                    });
                }
            }

            var totaalBedrag = cartItemsWithProduct.Sum(ci => ci.Aantal * ci.Prijs);

            return Ok(new
            {
                cartItems = cartItemsWithProduct,
                totaalBedrag
            });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartDto dto)
        {
            var cartItems = string.IsNullOrEmpty(dto.CartItemsJson)
                ? new List<CartItemDto>()
                : System.Text.Json.JsonSerializer.Deserialize<List<CartItemDto>>(dto.CartItemsJson) ?? new List<CartItemDto>();

            // Remove the item with the given ProductId
            cartItems = cartItems
                .Where(ci => ci != null && ci.ProductId > 0 && ci.Aantal > 0 && ci.ProductId != dto.ProductId)
                .ToList();

            // Build the response with product info
            var cartItemsWithProduct = new List<CartItemWithProductViewModel>();
            decimal totaalBedrag = 0;
            foreach (var item in cartItems)
            {
                var product = await _unitOfWork.Producten.GetByIdWithPriceAsync(item.ProductId);
                var prijs = product?.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).FirstOrDefault()?.Prijs;
                if (product != null && prijs.HasValue)
                {
                    cartItemsWithProduct.Add(new CartItemWithProductViewModel
                    {
                        ProductId = item.ProductId,
                        Aantal = item.Aantal,
                        Naam = product.Naam,
                        Prijs = prijs.Value
                    });
                    totaalBedrag += prijs.Value * item.Aantal;
                }
            }

            return Ok(new
            {
                cartItems = cartItemsWithProduct.Select(ci => new
                {
                    productId = ci.ProductId,
                    aantal = ci.Aantal,
                    naam = ci.Naam,
                    prijs = ci.Prijs
                }),
                totaalBedrag
            });
        }
    }
}