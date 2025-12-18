using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionCompanyApp.Models
{
    [Table("material")]
    public class Material
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";

        [Column("material_type_id")]
        public int MaterialTypeId { get; set; }

        [Column("cost")]
        public decimal Cost { get; set; }

        [Column("stock_quantity")]
        public decimal StockQuantity { get; set; }

        [Column("min_stock")]
        public decimal MinStock { get; set; }

        [Column("package_quantity")]
        public decimal PackageQuantity { get; set; }

        [Column("unit")]
        public string Unit { get; set; } = "";

        public virtual MaterialType MaterialType { get; set; } = null!;
        public virtual List<ProductMaterial> ProductMaterials { get; set; } = new();
    }
}