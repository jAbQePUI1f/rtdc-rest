using rtdc_rest.api.Helpers;
using rtdc_rest.api.Models;
using rtdc_rest.api.Services.Abstract;
using System.Linq;
using System.Text.Json;

namespace rtdc_rest.api.BackgroundServices
{
    public class StockFlSyncJob : BackgroundService
    {
        public IServiceProvider _service { get; }
        private readonly IConfiguration _configuration;
        public StockFlSyncJob(IServiceProvider service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _service.CreateScope())
                    {
                        string apiUserName = _configuration.GetSection("AppSettings:ApiUserName").Value;
                        string apiPassword = _configuration.GetSection("AppSettings:ApiPassword").Value;
                        string stockFlow = _configuration.GetSection("AppSettings:StockFlow").Value;

                        var stockFlService = scope.ServiceProvider.GetRequiredService<IStockFlService>();
                        var stockFls = await stockFlService.GetStockFlListAsync();
                        var grouppedStockFlList = stockFls.GroupBy(g => g.dataSourceCode).ToList();

                        foreach (var grouppedStockFl in grouppedStockFlList)
                        {
                            List<CreateStockFlowReqJson> stockFlList = new();
                            foreach (var stockfl in grouppedStockFl)
                            {
                                CreateStockFlowReqJson createStockFlowReqJson = new();

                                createStockFlowReqJson.dataSourceCode = stockfl.dataSourceCode;
                                createStockFlowReqJson.manufacturerCode = stockfl.manufacturerCode;
                                createStockFlowReqJson.retailerCode = stockfl.retailerCode;
                                createStockFlowReqJson.retailerRefId = stockfl.retailerRefId;
                                createStockFlowReqJson.year = stockfl.year;
                                createStockFlowReqJson.month = stockfl.month;
                                createStockFlowReqJson.invoiceDate = stockfl.invoiceDate;
                                createStockFlowReqJson.invoiceDateSystem = stockfl.invoiceDateSystem;
                                createStockFlowReqJson.invoiceId = stockfl.invoiceId;
                                createStockFlowReqJson.invoiceNo = stockfl.invoiceNo;
                                createStockFlowReqJson.invoiceLine = stockfl.invoiceLine;
                                createStockFlowReqJson.productCode = stockfl.productCode;
                                createStockFlowReqJson.itemQuantity = stockfl.itemQuantity;
                                createStockFlowReqJson.quantityInPackage = stockfl.quantityInPackage;
                                createStockFlowReqJson.packageQuantity = stockfl.packageQuantity;
                                createStockFlowReqJson.itemBarcode = stockfl.itemBarcode;
                                createStockFlowReqJson.packageBarcode = stockfl.packageBarcode;
                                createStockFlowReqJson.lineAmount = stockfl.lineAmount;
                                createStockFlowReqJson.discountAmount = stockfl.discountAmount;
                                createStockFlowReqJson.salesOrderId = stockfl.salesOrderId;
                                createStockFlowReqJson.isReturnInvoice = stockfl.isReturnInvoice;

                                stockFlList.Add(createStockFlowReqJson);
                            }

                            string stockFlJsonString = JsonSerializer.Serialize(stockFlList);
                            LogFile("Hesaplanan süre", "Satış Datası:" + stockFlJsonString.ToString(), "", "true", "");

                            HttpClientHelper httpClientHelper = new(_configuration);

                            var response = httpClientHelper.SendPOSTRequest(apiUserName.ToString(), apiPassword.ToString(), stockFlow.ToString(), stockFlJsonString);

                            LogFile("Hesaplanan süre", "Data Logs:" + response.ToString(), "", "true", "");
                        }

                        await Task.Delay(1000 * 60, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    await Task.FromCanceled(stoppingToken);
                }
            }
        }
        public void LogFile(string logCaption, string stockfl, string grouppedStockFl, string isSuccess, string response)
        {
            StreamWriter log;
            if (!File.Exists(@"C:\salesdata.log"))
            {
                log = new StreamWriter(@"C:\salesdata.log");
            }
            else
            {
                log = File.AppendText(@"C:\salesdata.log");
            }
            log.WriteLine("------------------------");
            log.WriteLine("Hata Mesajı:" + response.ToString());
            log.WriteLine("Satış:" + stockfl.ToString() + " -> Bölge : " + grouppedStockFl.ToString());
            log.WriteLine("Başarılı mı ? :" + isSuccess.ToString());
            log.WriteLine("Log Adı:" + logCaption.ToString());
            log.WriteLine("Log Zamanı:" + DateTime.Now);

            log.Close();
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
