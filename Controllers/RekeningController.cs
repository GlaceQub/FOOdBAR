using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    [Authorize]
    public class RekeningController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public RekeningController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Rekening overzicht
        [Authorize(Roles = "Zaalverantwoordelijke, Eigenaar")]
        [HttpGet]
        public IActionResult Overzicht()
        {
            return View();
        }
        #endregion  
    }
}
