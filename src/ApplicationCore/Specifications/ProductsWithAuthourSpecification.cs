using ApplicationCore.Entities;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Specifications
{
    public class ProductsWithAuthourSpecification:Specification<Product>
    {
        public ProductsWithAuthourSpecification()
        {
            Query.Include(x => x.Author);
        }
    }
}
