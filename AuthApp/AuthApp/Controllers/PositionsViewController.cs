using Microsoft.AspNetCore.Mvc;

namespace AuthApp.Controllers
{
    [Route("Positions")]
    public class PositionsViewController : Controller
    {
        [HttpGet("Index")]
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
                return RedirectToAction("Login", "Account");

            return View("~/Views/Positions/Index.cshtml");
        }
    }
}
