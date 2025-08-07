namespace TaxFiling.Web.Models
{
    public class PackagesViewModel
    {
        public int PackagesId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int IsSelfFiling { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Curancy { get; set; } = string.Empty;
    }
}
