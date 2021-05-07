using ApplicationCore.Entities;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Specifications
{
    public class ProductSpecification : Specification<Product>
    {
        public ProductSpecification(int[] ids)
        {
            //verilen ids'ler urunId ile eşleşirse sepete ekle
            Query.Where(x => ids.Contains(x.Id));
        }
    }
}
