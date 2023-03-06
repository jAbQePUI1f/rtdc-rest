﻿using rtdc_rest.api.Helpers;
using rtdc_rest.api.Models;
using rtdc_rest.api.Services.Abstract;
using System.Linq;
using System.Text.Json;

namespace rtdc_rest.api.BackgroundServices
{
    public class StockFlSyncJob : BackgroundService
    {
        public IServiceProvider _service { get; }
        public StockFlSyncJob(IServiceProvider service)
        {
            _service = service;
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

                            string retailerJsonString = JsonSerializer.Serialize(stockFlList);
                            HttpClientHelper httpClientHelper = new();
                            var response = httpClientHelper.SendPOSTRequest("aykanlar", "AyKanLar&2023", "/StockFlows", retailerJsonString);
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
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
