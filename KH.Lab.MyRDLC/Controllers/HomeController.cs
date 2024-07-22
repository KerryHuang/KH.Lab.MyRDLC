using KH.Lab.MyRDLC.Models;
using KH.Lab.MyRDLC.Reports;
using KH.Lab.MyRDLC.Reports.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Diagnostics;
using System.Net.Mime;

namespace KH.Lab.MyRDLC.Controllers
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Export()
        {
            // 載入報表
            var report = new LocalReport();
            // 取得報表資料
            var items = _northwindcontext.Employees.ToList().ToDataTable();
            // 取得報表路徑
            var assembly = typeof(Reports.Const).Assembly;
            using var rs = assembly.GetManifestResourceStream("KH.Lab.MyRDLC.Reports.RDLCs.Report1.rdlc");
            report.LoadReportDefinition(rs);
            // 設定報表資料來源
            report.DataSources.Add(new ReportDataSource("Employees", items));
            // 生成報表
            var result = report.Render("EXCEL");
            // 回傳檔案
            return File(result, MediaTypeNames.Application.Octet, "員工資料表.xlsx");
        }

        public IActionResult Print()
        {
            // 載入報表
            var report = new LocalReport();
            // 取得報表資料
            var items = _northwindcontext.Employees.ToList().ToDataTable();
            // 取得報表路徑
            var assembly = typeof(Reports.Const).Assembly;
            using var rs = assembly.GetManifestResourceStream("KH.Lab.MyRDLC.Reports.RDLCs.Report1.rdlc");
            report.LoadReportDefinition(rs);
            // 設定報表資料來源
            report.DataSources.Add(new ReportDataSource("Employees", items));
            // 生成報表
            var result = report.Render("PDF");
            // 回傳檔案
            return File(result, MediaTypeNames.Application.Pdf, "員工資料表.pdf");
        }

        public IActionResult List()
        {
            // 載入報表
            var report = new LocalReport();
            // 取得報表資料
            var items = _northwindcontext.Orders.ToList().ToDataTable();
            // 取得報表路徑
            var assembly = typeof(Reports.Const).Assembly;
            using var rs = assembly.GetManifestResourceStream("KH.Lab.MyRDLC.Reports.RDLCs.Report2.rdlc");
            report.LoadReportDefinition(rs);
            // 設定報表資料來源
            report.DataSources.Add(new ReportDataSource("Orders", items));
            // 生成報表
            var result = report.Render("PDF");
            // 回傳檔案
            return File(result, MediaTypeNames.Application.Pdf, "配送單.pdf");
        }

        public IActionResult List2()
        {
            // 載入報表
            var report = new LocalReport();
            // 取得報表路徑
            var assembly = typeof(Reports.Const).Assembly;
            using var rs = assembly.GetManifestResourceStream("KH.Lab.MyRDLC.Reports.RDLCs.Report3.rdlc");
            report.LoadReportDefinition(rs);
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
            report.DataSources.Add(new ReportDataSource("DeliveryOrder", ds.Tables["DeliveryOrder"]));
            // 生成報表
            var result = report.Render("PDF");
            // 回傳檔案
            return File(result, MediaTypeNames.Application.Pdf, "配送單2.pdf");
        }
    }
}
