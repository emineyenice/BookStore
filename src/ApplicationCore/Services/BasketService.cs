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

        public BasketService(IAsyncRepository<BasketItem> basketItemRepository)
        {
            _basketItemRepository = basketItemRepository;
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

        public async Task DeleteBasketItem(int basketId, int basketItemId)
        {
            var spec = new ManageBasketItemsSpecification(basketId, basketItemId); //basketId si ve basketıtemID sini alıyoruz
            var item = await _basketItemRepository.FirstOrDefaultAsync(spec); //sepeti getir
            await _basketItemRepository.DeleteAsync(item); //sil

        }

    }
}
