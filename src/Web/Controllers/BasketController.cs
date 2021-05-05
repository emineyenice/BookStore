using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    //todo: web > basketviewmodelservice > getcreatebasket
    //todo: appcore > basketservice > additemtobasket
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
