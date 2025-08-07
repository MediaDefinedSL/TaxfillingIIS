using Microsoft.AspNetCore.Mvc;

namespace TaxFiling.Web.Controllers
{
    public class PackageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
