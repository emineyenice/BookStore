using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    //todo: web > basketviewmodelservice > getcreatebasket
    //todo: appcore > basketservice > additemtobasket
    public class BasketController : Controller
    {
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly IBasketService _basketService;

        public BasketController(IBasketViewModelService basketViewModelService, IBasketService basketService)
        {
            _basketViewModelService = basketViewModelService;
            _basketService = basketService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToBasket(int productId, int quantity)
        {
            // application core projesinde BasketService:IBasketService
            //AddItemToBasket(int basketId, int catologId,int quantity)
            //sepette varsa adedini güncelle yoksa ekle
            //sepetteki öğe sayısını döndür

            var basketId = await _basketViewModelService.GetOrCreateBasketIdAsync();
            await _basketService.AddItemToBasket(basketId, productId, quantity);
            int basketItemsCount = await _basketService.BasketItemsCount(basketId);

            return Json(new BasketItemsCountViewModel() { BasketItemsCount = basketItemsCount });
        }
    }
}
