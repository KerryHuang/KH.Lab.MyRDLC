using KH.Lab.MyRDLC.Helpers;
using KH.Lab.MyRDLC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
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
    }
}
