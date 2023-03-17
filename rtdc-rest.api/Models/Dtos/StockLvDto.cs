namespace rtdc_rest.api.Models.Dtos
{
    public class StockLvDto
    {
        public string dataSourceCode { get; set; }
        public string manufacturerCode { get; set; }
        public DateTime stockDate { get; set; }
        public string productCode { get; set; }
        public double itemQuantity { get; set; }
        public int quantityInPackage { get; set; }
        public double packageQuantity { get; set; }
        public string itemBarcode { get; set; }
        public string packageBarcode { get; set; }
        public double listPrice { get; set; }
    }
}