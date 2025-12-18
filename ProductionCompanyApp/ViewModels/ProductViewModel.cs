namespace ProductionCompanyApp.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Article { get; set; } = "";
        public string ProductTypeName { get; set; } = "";
        public string Name { get; set; } = "";
        public decimal MinPartnerPrice { get; set; }
        public decimal RollWidth { get; set; }
        public decimal CalculatedCost { get; set; }
    }
}