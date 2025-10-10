using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShoeShop.Web.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (string.IsNullOrEmpty(session.GetString("UserName")))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
