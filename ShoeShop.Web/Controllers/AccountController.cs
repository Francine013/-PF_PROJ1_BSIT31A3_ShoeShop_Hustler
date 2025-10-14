using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ShoeShop.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login() => View();
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "Admin" && password == "1234")
            {
                HttpContext.Session.SetString("UserName", username);
                return RedirectToAction("Index", "Home");
            }
            else if (username == "Manager" && password == "manager")
            {
                HttpContext.Session.SetString("UserName", username);
                return RedirectToAction("Index", "Home");
            }
            else if (username == "Staff" && password == "staff")
            {
                HttpContext.Session.SetString("UserName", username);
                return RedirectToAction("Index", "Home");
            }
                ViewBag.Error = "Invalid credentials";
            return View();
        }



        public IActionResult CreateAccount() => View();

        [HttpPost]
        public IActionResult CreateAccount(string username, string password)
        {
            HttpContext.Session.SetString("UserName", username);
            return RedirectToAction("Dashboard", "Home");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
