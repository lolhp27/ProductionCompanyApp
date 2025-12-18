using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionCompanyApp.Models
{
    [Table("product_type")]
    public class ProductType
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";

        [Column("coefficient")]
        public double Coefficient { get; set; }

        public virtual List<Product> Products { get; set; } = new();
    }
}