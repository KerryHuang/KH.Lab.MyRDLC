using AspNetCore.Reporting;
using KH.Lab.MyRDLC.Reports;
using KH.Lab.MyRDLC.Reports.Helpers;
using KH.Lab.MyRDLC.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Net.Mime;

namespace KH.Lab.MyRDLC.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext _northwindcontext;

        public HomeController(ILogger<HomeController> logger, NorthwindContext northwindcontext)
        {
            _logger = logger;
            _northwindcontext = northwindcontext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Export()
        {
            // 取得報表路徑
            var reportPath = Path.Combine(Directory.GetCurrentDirectory(), "RDLCs", "Report1.rdlc");
            // 載入報表
            var report = new LocalReport(reportPath);
            // 取得報表資料
            var items = _northwindcontext.Employees.ToList().ToDataTable();
            // 設定報表資料來源
            report.AddDataSource(nameof(Employees), items);
            // 生成報表
            var result = report.Execute(RenderType.Excel);
            // 回傳檔案
            return File(result.MainStream, MediaTypeNames.Application.Octet, "員工資料表.xlsx");
        }

        public IActionResult Print()
        {
            // 取得報表路徑
            var reportPath = Path.Combine(AppContext.BaseDirectory, "RDLCs", "Report1.rdlc");
            // 載入報表
            var report = new LocalReport(reportPath);
            // 取得報表資料
            var items = _northwindcontext.Employees.ToList().ToDataTable();
            // 設定報表資料來源
            report.AddDataSource(nameof(Employees), items);
            // 生成報表
            var result = report.Execute(RenderType.Pdf);
            // 回傳檔案
            return File(result.MainStream, MediaTypeNames.Application.Pdf, "員工資料表.pdf");
        }

        public IActionResult List()
        {
            // 取得報表路徑
            var reportPath = Path.Combine(AppContext.BaseDirectory, "RDLCs", "Report2.rdlc");
            // 載入報表
            var report = new LocalReport(reportPath);
            // 取得報表資料
            var items = _northwindcontext.Orders.ToList().ToDataTable();
            // 設定報表資料來源
            report.AddDataSource(nameof(Orders), items);
            // 生成報表
            var result = report.Execute(RenderType.Pdf);
            // 回傳檔案
            return File(result.MainStream, MediaTypeNames.Application.Pdf, "配送單.pdf");
        }

        public IActionResult List2()
        {
            // 取得報表路徑
            var reportPath = Path.Combine(AppContext.BaseDirectory, "RDLCs", "Report3.rdlc");
            // 載入報表
            var report = new LocalReport(reportPath);
            // 取得報表資料
            var items = _northwindcontext.Orders.ToList();
            var ds = new DeliveryDataSet();
            foreach (var item in items)
            {
                DataRow dr = ds.Tables["DeliveryOrder"].NewRow();
                string barcode = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                dr["DeliveryNumber"] = barcode;
                dr["ShipName"] = item.ShipName;
                dr["ShipAddress"] = item.ShipAddress;
                dr["BarCode"] = BarcodeHelper.CreateBarCodeBytes(barcode, 300, 100);
                dr["QRCode"] = BarcodeHelper.CreateQRCodeBytes(barcode, 200, 200);
                ds.Tables["DeliveryOrder"].Rows.Add(dr);
            }
            // 設定報表資料來源
            report.AddDataSource("DeliveryOrder", ds.Tables["DeliveryOrder"]);
            // 生成報表
            var result = report.Execute(RenderType.Pdf);
            // 回傳檔案
            return File(result.MainStream, MediaTypeNames.Application.Pdf, "配送單2.pdf");
        }
    }
}
