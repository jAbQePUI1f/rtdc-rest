using Dapper;
using rtdc_rest.api.Models.Dtos;
using rtdc_rest.api.Services.Abstract;
using System.Data.SqlClient;

namespace rtdc_rest.api.Services.Concrete
{
    public class StockLvManager : IStockLvService
    {
        private readonly IConfiguration _configuration;
        public StockLvManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<StockLvDto>> GetStockLvListAsync()
        {
            string connection = _configuration.GetSection("AppSettings:DbConnection").Value;
            string companyCode = _configuration.GetSection("AppSettings:CompanyCode").Value;
            string season = _configuration.GetSection("AppSettings:Season").Value;

            {
                SqlConnection connect = new SqlConnection(connection);
                connect.Open(); 

                var sql = " SELECT DataSourceCode = CASE StLinePort.SOURCEINDEX WHEN 35 THEN 'AYKIZM' WHEN 7 THEN 'AYKANT'" +
                    "  WHEN 42 THEN 'AYKKNY' WHEN 50 THEN 'AYKIST' ELSE 'TANIMSIZ' END " +
                    ", ManufacturerCode = CASE StCardPort.SPECODE WHEN 'BP' THEN 'BYR' ELSE StCardPort.SPECODE END " +
                    ", StockDate = getdate() " +
                    ", ProductCode = StCardPort.PRODUCERCODE " +
                    ", ItemQuantity = SUM(CASE WHEN StLinePort.IOCODE IN(1, 2) THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 THEN 0 ELSE StLinePort.UINFO2 END) " +
                    "  WHEN StLinePort.IOCODE IN(3,4) THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 THEN 0 ELSE StLinePort.UINFO2 END ) *-1 ELSE 0 END ) " +
                    ", QuantityInPackage = (SELECT CONVFACT2 FROM LG_" + companyCode + "_ITMUNITA ITMN WHERE StCardPort.LOGICALREF = ITMN.ITEMREF AND LINENR = 2 ) " +
                    ", PackageQuantity = SUM(CASE WHEN StLinePort.IOCODE IN(1, 2) " +
                    "  THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 THEN 0 ELSE StLinePort.UINFO2 / ITMUNITA.CONVFACT2 END) " +
                    "  WHEN StLinePort.IOCODE IN(3,4) THEN StLinePort.AMOUNT * (CASE WHEN ITMUNITA.CONVFACT2 = 0 THEN 0 ELSE StLinePort.UINFO2 / ITMUNITA.CONVFACT2 END ) *-1 ELSE 0 END ) " +
                    ", ItemBarcode = EK.URUNBARKODU " +
                    ", PackageBarcode = EK.KOLİBARKODU " +
                    ", ListPrice = (SELECT TOP(1) PRICE FROM LG_" + companyCode + "_PRCLIST AS PRC WHERE(StCardPort.LOGICALREF = CARDREF) AND(PTYPE = 1) ORDER BY BEGDATE DESC ) " +
                    "  FROM LG_" + companyCode + "_" + season + "_STFICHE StFichePort WITH(NOLOCK) " +
                    "  INNER JOIN LG_" + companyCode + "_" + season + "_STLINE StLinePort WITH(NOLOCK) ON StFichePort.LOGICALREF = StLinePort.STFICHEREF " +
                    "  INNER JOIN LG_" + companyCode + "_ITEMS StCardPort WITH(NOLOCK) ON StLinePort.STOCKREF = StCardPort.LOGICALREF " +
                    "  LEFT OUTER JOIN LG_" + companyCode + "_ITMUNITA ITMUNITA WITH(NOLOCK) ON StCardPort.LOGICALREF = ITMUNITA.ITEMREF AND ITMUNITA.LINENR = '2' " +
                    "  LEFT OUTER JOIN LG_" + companyCode + "_ITMUNITA ITMUNITA1 WITH(NOLOCK) ON StCardPort.LOGICALREF = ITMUNITA1.ITEMREF AND ITMUNITA1.LINENR = '4' " +
                    "  LEFT OUTER JOIN LG_XT1001_" + companyCode + " AS EK ON StCardPort.LOGICALREF = EK.PARLOGREF " +
                    "  WHERE StLinePort.LINETYPE IN(0,1)  AND StLinePort.SOURCEINDEX IN('35','7','42','50')  AND StFichePort.CANCELLED = 0 " +
                    "  AND StCardPort.SPECODE IN('3M','BP','WL') " +
                    "  GROUP BY StLinePort.SOURCEINDEX,StCardPort.SPECODE,StCardPort.PRODUCERCODE,StCardPort.LOGICALREF,EK.URUNBARKODU ,EK.KOLİBARKODU ";

                var result = connect.Query<StockLvDto>(sql).ToList();
                return result;
            }
        }
    }
}
