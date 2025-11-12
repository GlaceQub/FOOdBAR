namespace Restaurant.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Klant, Eigenaar")]
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
            // Get cart from session or create new
            var cartItems = HttpContext.Session.GetObject<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

            // Find if product with same opmerking exists
            string normalizedOpmerking = (dto.Opmerking ?? "").Trim();
            var existing = cartItems.FirstOrDefault(ci =>
                ci.ProductId == dto.ProductId &&
                ((ci.Opmerking ?? "").Trim() == normalizedOpmerking)
            );

            if (existing != null)
            {
                existing.Aantal += dto.Aantal > 0 ? dto.Aantal : 1;
            }
            else
            {
                cartItems.Add(new CartItemDto
                {
                    ProductId = dto.ProductId,
                    Aantal = dto.Aantal > 0 ? dto.Aantal : 1,
                    Opmerking = normalizedOpmerking
                });
            }

            HttpContext.Session.SetObject("Cart", cartItems);

            // Enrich and return cart as before...
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
                        Prijs = prijs.Value,
                        Opmerking = item.Opmerking
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
            // Get cart from session or create new
            var cartItems = HttpContext.Session.GetObject<List<CartItemDto>>("Cart") ?? new List<CartItemDto>();

            string normalizedOpmerking = (dto.Opmerking ?? "").Trim();

            var item = cartItems.FirstOrDefault(ci =>
                ci != null &&
                ci.ProductId == dto.ProductId &&
                ((ci.Opmerking ?? "").Trim() == normalizedOpmerking) &&
                ci.Aantal > 0);

            if (item != null)
            {
                item.Aantal--;
                if (item.Aantal <= 0)
                    cartItems.Remove(item);
            }

            // Update session cart
            HttpContext.Session.SetObject("Cart", cartItems);

            // Enrich and return cart as before
            var cartItemsWithProduct = new List<CartItemWithProductViewModel>();
            decimal totaalBedrag = 0;
            foreach (var ci in cartItems)
            {
                var product = await _unitOfWork.Producten.GetByIdWithPriceAsync(ci.ProductId);
                var prijs = product?.PrijsProducten.OrderByDescending(pp => pp.DatumVanaf).FirstOrDefault()?.Prijs;
                if (product != null && prijs.HasValue)
                {
                    cartItemsWithProduct.Add(new CartItemWithProductViewModel
                    {
                        ProductId = ci.ProductId,
                        Aantal = ci.Aantal,
                        Naam = product.Naam,
                        Prijs = prijs.Value,
                        Opmerking = ci.Opmerking
                    });
                    totaalBedrag += prijs.Value * ci.Aantal;
                }
            }

            return Ok(new
            {
                cartItems = cartItemsWithProduct,
                totaalBedrag
            });
        }
    }
}