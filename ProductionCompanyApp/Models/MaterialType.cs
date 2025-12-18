using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionCompanyApp.Models
{
    [Table("material_type")]
    public class MaterialType
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";

        [Column("defect_percent")]
        public double DefectPercent { get; set; }

        public virtual List<Material> Materials { get; set; } = new();
    }
}