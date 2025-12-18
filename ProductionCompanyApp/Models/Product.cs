using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionCompanyApp.Models
{
    [Table("product")] // Указываем точное имя таблицы
    public class Product
    {
        [Column("id")] // Точное имя столбца
        public int Id { get; set; }

        [Column("article")]
        public string Article { get; set; } = "";

        [Column("product_type_id")] // В БД: product_type_id
        public int ProductTypeId { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";

        [Column("min_partner_price")] // В БД: min_partner_price
        public decimal MinPartnerPrice { get; set; }

        [Column("roll_width")] // В БД: roll_width
        public decimal RollWidth { get; set; }

        // Навигационные свойства
        public virtual ProductType ProductType { get; set; } = null!;
        public virtual List<ProductMaterial> ProductMaterials { get; set; } = new();
    }
}