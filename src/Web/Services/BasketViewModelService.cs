using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IBasketService _basketService;
        private readonly IAsyncRepository<Product> _profuctRepository;

        public BasketViewModelService(IHttpContextAccessor httpContextAccessor, IAsyncRepository<Basket> basketRepository, IBasketService basketService, IAsyncRepository<Product> profuctRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _basketRepository = basketRepository;
            _basketService = basketService;
            _profuctRepository = profuctRepository;
        }

        public async Task<BasketItemsCountViewModel> GetBasketItemsCountViewModel(int? basketId = null)
        {
            var vm = new BasketItemsCountViewModel();
            if (!basketId.HasValue)
            {
                //sepet yoksa oluşturma sıfır döndür Returns basket item count ,returns 0 if basket does not exist.
                string buyerId = GetBuyerId();
                if (buyerId == null) return vm;
                var spec = new BasketSpecification(buyerId);
                var basket = await _basketRepository.FirstOrDefaultAsync(spec);
                if (basket == null) return vm;
                basketId = basket.Id;

            }

            //basketId değeri yoksa 0 döndürür varsa basketId değerini döndürür
            vm.BasketItemsCount = await _basketService.BasketItemsCount(basketId.Value);
            return vm;

            //return new BasketItemsCountViewModel()
            //{
            //    BasketItemsCount = await _basketService.BasketItemsCount(basketId)
            //};
        }

        public async Task<BasketViewModel> GetBasketViewModel()
        {
            int basketId = await GetOrCreateBasketIdAsync(); //sepeti getir yoksa oluştur
            var specBasket = new BasketWithItemsSpecification(basketId); //basketId'si eşleşen sepetteki ürünleriyle getir
            var basket = await _basketRepository.FirstOrDefaultAsync(specBasket);// sepeti eşleştir
            var productIds = basket.Items.Select(x => x.ProductId).ToArray(); //sepetteki ögelerin ilişkili olduğu ürünleri getiriyoruz
            var specProducts = new ProductSpecification(productIds);
            var products = await _profuctRepository.ListAsync(specProducts);
            var basketItems = new List<BasketItemViewModel>();
            foreach (var item in basket.Items.OrderBy(x => x.Id))
            {
                var product = products.First(x => x.Id == item.ProductId);
                basketItems.Add(new BasketItemViewModel()
                {
                    //sepet ogelerini urun bilgileriyle birlikte BasketViewModel nesnelerini oluşturduk ve listeye ekledik
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ProductName = product.Name,
                    Price = product.Price,
                    PictureUri = product.PictureUri
                });
            }

            return new BasketViewModel()
            {
                //sepetId,AlıcıId,Sepet ogeleri ile basketViewModel oluşturduk
                Id = basketId,
                BuyerId = basket.BuyerId,
                Items = basketItems

            };
        }

        public string GetBuyerId()
        {
            var context = _httpContextAccessor.HttpContext;
            var user = context.User;
            var anonId = context.Request.Cookies[Constants.BASKET_COOKIE_NAME];

            return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? anonId;
            //null değilse sağdakini nullsa soldakini döndürür
            //if (user.Identity.IsAuthenticated)
            //    return user.FindFirstValue(ClaimTypes.NameIdentifier);
            //else if (anonId != null)
            //    return anonId;
            //else
            //    return null;


        }

        public async Task<int> GetOrCreateBasketIdAsync()
        {
            var buyerId = GetOrCreateBuyerId();
            var spec = new BasketSpecification(buyerId);
            var basket = await _basketRepository.FirstOrDefaultAsync(spec);

            if (basket == null)
            {
                basket = new Basket() { BuyerId = buyerId };
                await _basketRepository.AddAsync(basket);
            }

            return basket.Id;
        }

        public string GetOrCreateBuyerId()
        {
            var context = _httpContextAccessor.HttpContext;
            var user = context.User;

            // return user id if user is logged in
            if (user.Identity.IsAuthenticated)
            {
                return user.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else
            {
                // return anonymous user id if a basket cookie exists
                if (context.Request.Cookies.ContainsKey(Constants.BASKET_COOKIE_NAME))
                {
                    return context.Request.Cookies[Constants.BASKET_COOKIE_NAME];
                }
                // create and return an anonymous user id
                else
                {
                    string newBuyerId = Guid.NewGuid().ToString();
                    var cookieOptions = new CookieOptions()
                    {
                        IsEssential = true,
                        Expires = DateTime.Now.AddYears(10)
                    };
                    context.Response.Cookies.Append(Constants.BASKET_COOKIE_NAME, newBuyerId, cookieOptions);
                    return newBuyerId;
                }
            }
        }

        public async Task TransferBasketsAsync(string userId)
        {
            var context = _httpContextAccessor.HttpContext;
            //Transfer Baskets
            var anonId = context.Request.Cookies[Constants.BASKET_COOKIE_NAME];
            if (!string.IsNullOrEmpty(anonId))
                await _basketService.TransferBasketAsync(anonId, userId);
            context.Response.Cookies.Delete(Constants.BASKET_COOKIE_NAME); //anonim basket cooki'yi sil
        }
    }
}
