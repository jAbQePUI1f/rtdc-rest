namespace rtdc_rest.api.Models.Dtos
{
    public class StockFlDto
    {
        public string dataSourceCode { get; set; }
        public string manufacturerCode { get; set; }
        public string retailerCode { get; set; }
        public long retailerRefId { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public DateTime invoiceDate { get; set; }
        public DateTime invoiceDateSystem { get; set; }
        public long invoiceId { get; set; }
        public string invoiceNo { get; set; }
        public int invoiceLine { get; set; }
        public string productCode { get; set; }
        public int itemQuantity { get; set; }
        public int quantityInPackage { get; set; }
        public double packageQuantity { get; set; }
        public string itemBarcode { get; set; }
        public string packageBarcode { get; set; }
        public double lineAmount { get; set; }
        public double discountAmount { get; set; }
        public long salesOrderId { get; set; }
        public bool isReturnInvoice { get; set; }
    }
}