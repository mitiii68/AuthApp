using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthApp.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = HttpContext.Session.GetString("user");

            if (user == null)
            {
                context.Result = RedirectToAction("Login", "Account");
            }

            base.OnActionExecuting(context); 
        }
    }
}