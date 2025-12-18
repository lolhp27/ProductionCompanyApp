using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionCompanyApp.Models
{
    [Table("product_material")]
    public class ProductMaterial
    {
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("material_id")]
        public int MaterialId { get; set; }

        [Column("quantity_per_product")]
        public double QuantityPerProduct { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual Material Material { get; set; } = null!;
    }
}