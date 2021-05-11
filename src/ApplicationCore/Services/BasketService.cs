using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class BasketService : IBasketService
    {
        private readonly IAsyncRepository<BasketItem> _basketItemRepository;
        private readonly IAsyncRepository<Basket> _basketRepository;

        public BasketService(IAsyncRepository<BasketItem> basketItemRepository, IAsyncRepository<Basket> basketRepository)
        {
            _basketItemRepository = basketItemRepository;
            _basketRepository = basketRepository;
        }
        public async Task AddItemToBasket(int basketId, int productId, int quantity)
        {
            //sepetteki kalem adedi 1 den az'sa hata fırlat 
            if (quantity < 1)
                throw new ArgumentException("Quantity can not be zero or a negative number ");
            var spec = new BasketItemSpecification(basketId, productId);
            var basketItem = await _basketItemRepository.FirstOrDefaultAsync(spec);

            if (basketItem != null)
            {
                //
                basketItem.Quantity += quantity;
                await _basketItemRepository.UpdateAsync(basketItem);
            }

            else
            {
                basketItem = new BasketItem() { BasketId = basketId, ProductId = productId, Quantity = quantity };
                await _basketItemRepository.AddAsync(basketItem);
            }
        }

        public async Task<int> BasketItemsCount(int basketId)
        {
            // basketId 'sine göre sepetteki kalem adedini say
            //basketItemspecification' da sepetId'leri eşleştirdik, eşleştirdiğimiz sepetin kalem adedini say.
            var spec = new BasketItemSpecification(basketId);
            return await _basketItemRepository.CountAsync(spec);
        }

        public async Task DeleteBasketAsync(int basketId)
        {
            //ödemeden sonra sepeti silecek metot
            var basket = await _basketRepository.GetByIdAsync(basketId); //sepeti Id siyle getir
            await _basketRepository.DeleteAsync(basket);
        }

        public async Task DeleteBasketItem(int basketId, int basketItemId)
        {
            var spec = new ManageBasketItemsSpecification(basketId, basketItemId); //basketId si ve basketıtemID sini alıyoruz
            var item = await _basketItemRepository.FirstOrDefaultAsync(spec); //sepeti getir
            await _basketItemRepository.DeleteAsync(item); //sil

        }

        public async Task TransferBasketAsync(string anonymousId, string userId)
        {
            var specAnon = new BasketWithItemsSpecification(anonymousId);
            var basketAnon = await _basketRepository.FirstOrDefaultAsync(specAnon);

            if (basketAnon == null || !basketAnon.Items.Any()) return; //sepet yoksa ya da sepet oluşmus ancak sepet ogesi yoksa dönüştürecek bisey yok

            var specUser = new BasketWithItemsSpecification(userId);
            var basketUser = await _basketRepository.FirstOrDefaultAsync(specUser);

            if (basketUser == null)
                //giriş yapmadan sepete oge eklediyse alıcıId lerini eşleştir
                basketUser = await _basketRepository.AddAsync(new Basket() { BuyerId = userId });

            //anonim sepetten kullanıcı sepetine transfer edelim
            foreach (BasketItem itemAnon in basketAnon.Items)
            {
                var itemUser = basketUser.Items.FirstOrDefault(x => x.ProductId == itemAnon.ProductId);//kullanıcının sepetinde ürün var mı

                if (itemUser == null)
                {
                    //sepette oge yoksa anonim basketId ile kullanıcı basket ıd'yi eşleştir
                    //basketUser.Items.Add(new BasketItem() { ProductId = item.ProductId, });
                    basketUser.Items.Add(new BasketItem()
                    {
                        ProductId = itemAnon.ProductId,
                        Quantity = itemAnon.Quantity
                    });
                }

                else
                {
                    // sepette urun varsa sayısını artır
                    itemUser.Quantity += itemAnon.Quantity;
                }

            }
            await _basketRepository.UpdateAsync(basketUser);
            await _basketRepository.DeleteAsync(basketAnon); //anonim sepeti sil
        }

        public async Task UpdateBasketItem(int basketId, int basketItemId, int quantity)
        {
            if (quantity < 1) throw new Exception("The quantity cannot be less than 1");
            var spec = new ManageBasketItemsSpecification(basketId, basketItemId);
            var item = await _basketItemRepository.FirstOrDefaultAsync(spec);
            item.Quantity = quantity;
            await _basketItemRepository.UpdateAsync(item);
        }
    }
}
