using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    // Acts as a fallback in order to fix the routing problem that exists otherwise.
    [AllowAnonymous]
    public class Fallback : Controller
    {
        public IActionResult index ()
            => PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
    }
}