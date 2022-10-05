using ContosoWeb.Models;
using ContosoWeb.Models.SchoolViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistencia.Entidades;
using Persistencia.Repositorio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoWeb.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
  
            private readonly ILogger<HomeController> _logger;
            private readonly SchoolContext _context;

        public readonly UserManager<ApplicationUser> _userManager;

        private IWebHostEnvironment _environment;


        public HomeController(ILogger<HomeController> logger, 
                              SchoolContext context,
                              UserManager<ApplicationUser> userManager,
                              IWebHostEnvironment environment)
            {
                _logger = logger;
                _userManager = userManager;
                _environment = environment;
                _context = context;
            }

            public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            var usuario = await _userManager.GetUserAsync(User);


            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddSeconds(30);
            Response.Cookies.Append("UserName", usuario.Email, option);
            Response.Cookies.Append("DataHora", DateTime.Now.ToString(), option);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        public async Task<ActionResult> About()
        {
            string userName = Request.Cookies["UserName"];
            string dataHora = Request.Cookies["DataHora"];

            if (userName != null)
            {
                ViewData["usuario"] = userName;
                ViewData["dataH"] = dataHora;
            }
            else
            {
                ViewData["usuario"] = "não logado";
                ViewData["dataH"] =  "";
            }
            IQueryable<EnrollmentDateGroup> data =
                from student in _context.Students
                group student by student.EnrollmentDate into dateGroup
                select new EnrollmentDateGroup()
                {
                    EnrollmentDate = dateGroup.Key,
                    StudentCount = dateGroup.Count()
                };
            return View(await data.AsNoTracking().ToListAsync());
        }
    }
}
