using Microsoft.EntityFrameworkCore;
using ProductionCompanyApp.Data;
using ProductionCompanyApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProductionCompanyApp.Services
{
    public class ProductCostService
    {
        private readonly AppDbContext _db;

        public ProductCostService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<decimal> CalculateProductCostAsync(int productId)
        {
            try
            {
                var product = await _db.Products
                    .Include(p => p.ProductMaterials)
                        .ThenInclude(pm => pm.Material)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null || product.ProductMaterials == null)
                    return 0m;

                decimal totalCost = 0m;

                foreach (var pm in product.ProductMaterials)
                {
                    if (pm.Material != null)
                    {
                        var quantity = (decimal)pm.QuantityPerProduct;
                        totalCost += quantity * pm.Material.Cost;
                    }
                }

                return Math.Round(totalCost, 2);
            }
            catch (Exception)
            {
                return 0m;
            }
        }
    }
}