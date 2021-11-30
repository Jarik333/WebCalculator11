using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WebCalculator1;
using EFDataApp.Models;
using UAParser;
using Microsoft.AspNetCore.Http;
using PagedList;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebCalculator1.Models;
using Microsoft.AspNetCore.Authorization;

namespace EFDataApp.Controllers
{
    
    public class HomeController : Controller
    {
        public ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
           db = context;
        }
        Calculator calculator = new Calculator();
       
        private readonly ILogger<HomeController> _logger;
       
        
      
        [HttpGet]
        public IActionResult Index()
        {
            
            return View();
        }
        
        [HttpPost]
      
        public ActionResult Index(string expression, Operation operation)
        {
            HostString httpRequest = HttpContext.Request.Host;
            operation.IP = httpRequest.ToString();
           
            var userAgent = HttpContext.Request.Headers["User-Agent"];
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);
            operation.Browser = c.UA.Family.ToString();

            DateTime now = DateTime.Now;
            operation.Date = now;

       

            db.Operations.Add(operation);
       
            
            try
            {
                double result = 0;
                result = calculator.Evaluate(expression);
                
                if ((result != double.PositiveInfinity) && (result != double.NegativeInfinity))
                {
                   
                    ViewData["Message"] = $"Ответ: {result}";
                    operation.Result = result;
                    operation.Error = "-";
                }
                else
                {
                    ViewData["Message"] = "Ошибка: попытка деления на 0";
                    operation.Error = "Попытка деления на 0";
                   
                }
            }
            catch (System.FormatException)
            {
                ViewData["Message"] = "Ошибка: неверный формат ввода";
                operation.Error = "Неверный формат ввода";
                
            }

            db.SaveChangesAsync();
           
            return View();
           
        }
       
        public  ViewResult Info(int ? page)
        {
            
            var operations = from s in db.Operations
            select s;
            operations = operations.OrderByDescending(s => s.Date);
            int pageSize = 10;
            int pageIndex = (page ?? 1);
            return View(operations.ToPagedList(pageIndex, pageSize));

            
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? ID)
        {
            if (ID != null)
            {
                Operation operation = await db.Operations.FirstOrDefaultAsync(p => p.ID == ID);
                if (operation != null)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        return View(operation);
                    }
                    else
                    {
                        return View("Index");
                    }
                }
                   
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Operation operation)
        {
            db.Operations.Update(operation);
            await db.SaveChangesAsync();
            return RedirectToAction("Info");
        }
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? ID)
        {
            if (ID != null)
            {
                Operation operation = await db.Operations.FirstOrDefaultAsync(p => p.ID == ID);
                if (operation != null)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        return View(operation);
                    }
                    else
                    {
                        return View("Index");
                    }
                }
                   
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? ID)
        {
            if (ID != null)
            {
                Operation operation = await db.Operations.FirstOrDefaultAsync(p => p.ID == ID);
                if (operation != null)
                {
                    db.Operations.Remove(operation);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Info");
                }
            }
            return NotFound();
        }
        [HttpGet]
    
        public ViewResult Add()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else {
                return View("Index");
            }

        }
        [HttpPost]
        public async Task<IActionResult> Add(string Expression, double Result, DateTime Date, string Error,string Browser,string IP, Operation operation)
        {
            operation.Expression = Expression;
            operation.Result = Result;
            operation.Date = Date;
            operation.Error = Error;
            operation.Browser = Browser;
            operation.IP = IP;
            db.Operations.Add(operation);
            await db.SaveChangesAsync();
            return RedirectToAction("Info");
        }

    }              
}

