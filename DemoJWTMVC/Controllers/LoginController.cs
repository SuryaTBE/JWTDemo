using DemoJWTMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DemoJWTMVC.Controllers
{
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var canAcess = false;

            // check the refer
            var referer = filterContext.HttpContext.Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                var rUri = new System.UriBuilder(referer).Uri;
                var req = filterContext.HttpContext.Request;
                if (req.Host.Host == rUri.Host && req.Host.Port == rUri.Port && req.Scheme == rUri.Scheme)
                {
                    canAcess = true;
                }
            }

            // ... check other requirements

            if (!canAcess)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Login", area = "" }));
            }
        }
    }
    public class LoginController : Controller
    {

        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserTbl usertbl)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(usertbl), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:44363/api/Jwt/UserRegistration", content);
                return RedirectToAction("Login", "Login");
            }
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserTbl usertbl)
        {
            UserTbl user = new UserTbl();
            JWT jwt = new JWT();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(usertbl), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:44363/api/Jwt/UserLogin", content);
                if (response.IsSuccessStatusCode)
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    jwt=JsonConvert.DeserializeObject<JWT>(apiresponse);
                    if (jwt == null)
                    {
                        ViewBag.errormessage = "Invalid e-mail or password.";
                        return View();
                    }
                    HttpContext.Session.SetString("UserMail", jwt.User.mail);
                    HttpContext.Session.SetInt32("UserId", jwt.User.UserId);
                    HttpContext.Session.SetString("Token", jwt.key);
                    return RedirectToAction("Index", "Login"); 
                }
                return View();
            }
        }
        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> Index()
        {
            string token=HttpContext.Session.GetString("Token");
            List<Product> product = new List<Product>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                //Add for getting authenticated by the token provided by the Login Method
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("https://localhost:44363/api/Product");
                if (Res.IsSuccessStatusCode)
                {
                    var response = Res.Content.ReadAsStringAsync().Result;
                    product = JsonConvert.DeserializeObject<List<Product>>(response);
                    return View(product);
                }
                else
                {
                    ViewBag.indexmsg = "Please Login to proceed further...";
                    return View();
                }
            }
            
        }
    }
}

