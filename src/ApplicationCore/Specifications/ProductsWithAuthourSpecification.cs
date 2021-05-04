using ApplicationCore.Entities;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Specifications
{
    public class ProductsWithAuthourSpecification : Specification<Product>
    {
        public ProductsWithAuthourSpecification()
        {
            Query.Include(x => x.Author);
        }
        //iki seçenek sunduk filtreleyerek de kullanılabilir filtresizde
        public ProductsWithAuthourSpecification(int? categoryId, int? authorId) : this() //bu const. cagırılınca önce üstteki const çagirsın
        {
            if (categoryId.HasValue)
            {
                Query.Where(x => x.CategoryId == categoryId);
            }

            if (authorId.HasValue)
            {
                Query.Where(x => x.AuthorId == authorId);
            }
        }
        public ProductsWithAuthourSpecification(int? categoryId, int? authorId, int skip, int take) : this(categoryId, authorId)
        {
            Query.Skip(skip).Take(take);
        }
    }
}
