using rtdc_rest.api.Helpers;
using rtdc_rest.api.Models;
using rtdc_rest.api.Services.Abstract;
using System.Text.Json;

namespace rtdc_rest.api.BackgroundServices
{
    public class StockLvSyncJob : BackgroundService
    {
        public IServiceProvider _service { get; }
        private readonly IConfiguration _configuration;
        public StockLvSyncJob(IServiceProvider service, IConfiguration configuration)
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
                        string stockLevel = _configuration.GetSection("AppSettings:StockLevel").Value;

                        var stockLvService = scope.ServiceProvider.GetRequiredService<IStockLvService>();
                        var stockLvs = await stockLvService.GetStockLvListAsync();
                        var grouppedStockLvList = stockLvs.GroupBy(g => g.dataSourceCode).ToList();

                        foreach (var grouppedStockLv in grouppedStockLvList)
                        {
                            List<CreateStockLevelReqJson> stockLvList = new();
                            foreach (var stockLv in grouppedStockLv)
                            {
                                CreateStockLevelReqJson createStockLevelReqJson = new();

                                createStockLevelReqJson.dataSourceCode = stockLv.dataSourceCode;
                                createStockLevelReqJson.manufacturerCode = stockLv.manufacturerCode;
                                createStockLevelReqJson.stockDate = stockLv.stockDate;
                                createStockLevelReqJson.productCode = stockLv.productCode;
                                createStockLevelReqJson.itemQuantity = stockLv.itemQuantity;
                                createStockLevelReqJson.quantityInPackage = stockLv.quantityInPackage;
                                createStockLevelReqJson.packageQuantity = stockLv.packageQuantity;
                                createStockLevelReqJson.itemBarcode = stockLv.itemBarcode;
                                createStockLevelReqJson.packageBarcode = stockLv.packageBarcode;
                                createStockLevelReqJson.listPrice = stockLv.listPrice;

                                stockLvList.Add(createStockLevelReqJson);
                            }

                            string stockLvJsonString = JsonSerializer.Serialize(stockLvList);

                            LogFile("Hesaplanan süre", "Stok Datası:" + stockLvJsonString.ToString(), "", "true", "");

                            HttpClientHelper httpClientHelper = new(_configuration);

                            var response = httpClientHelper.SendPOSTRequest(apiUserName.ToString(), apiPassword.ToString(), stockLevel.ToString(), stockLvJsonString);

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
        public void LogFile(string logCaption, string stockLv, string grouppedStockLv, string isSuccess, string response)
        {
            StreamWriter log;
            if (!File.Exists(@"C:\stockdata.log"))
            {
                log = new StreamWriter(@"C:\stockdata.log");
            }
            else
            {
                log = File.AppendText(@"C:\stockdata.log");
            }
            log.WriteLine("------------------------");
            log.WriteLine("Hata Mesajı:" + response.ToString());
            log.WriteLine("Stok:" + stockLv.ToString() + " -> Bölge : " + grouppedStockLv.ToString());
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
