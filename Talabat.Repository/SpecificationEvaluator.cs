using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    public static class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery; // _dbContext.Set<Product>()

            if (spec.Criteria is not null) // P => P.Id == 1
                query = query.Where(spec.Criteria);


            // _dbContext.Set<Product>()

            if (spec.OrderBy is not null) // P => P.Price
                query = query.OrderBy(spec.OrderBy);

            //query = _dbContext.Products.OrderBy(P => P.Price);

            if (spec.OrderByDesc is not null)
                query = query.OrderByDescending(spec.OrderByDesc);

            //query = _dbContext.Products.OrderByDescending(P => P.Price);

            if (spec.IsPaginationEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);


            // Includes
            // P => P.ProductBrand
            // P => P.ProductType

            query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));
            // _dbContext.Set<Product>().Where(P => P.Id == 1).Include(P => P.ProductBrand)
            // _dbContext.Set<Product>().Where(P => P.Id == 1).Include(P => P.ProductBrand).Include(P => P.ProductType)

            return query;
        }
    }
}
