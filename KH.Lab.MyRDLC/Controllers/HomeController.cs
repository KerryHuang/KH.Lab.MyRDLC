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
            // ���J����
            var report = new LocalReport();
            // ���o������
            var items = _northwindcontext.Employees.ToList().ToDataTable();
            // ���o������|
            var assembly = typeof(Reports.Const).Assembly;
            using var rs = assembly.GetManifestResourceStream("KH.Lab.MyRDLC.Reports.RDLCs.Report1.rdlc");
            report.LoadReportDefinition(rs);
            // �]�w�����ƨӷ�
            report.DataSources.Add(new ReportDataSource("Employees", items));
            // �ͦ�����
            var result = report.Render("EXCEL");
            // �^���ɮ�
            return File(result, MediaTypeNames.Application.Octet, "���u��ƪ�.xlsx");
        }

        public IActionResult Print()
        {
            // ���J����
            var report = new LocalReport();
            // ���o������
            var items = _northwindcontext.Employees.ToList().ToDataTable();
            // ���o������|
            var assembly = typeof(Reports.Const).Assembly;
            using var rs = assembly.GetManifestResourceStream("KH.Lab.MyRDLC.Reports.RDLCs.Report1.rdlc");
            report.LoadReportDefinition(rs);
            // �]�w�����ƨӷ�
            report.DataSources.Add(new ReportDataSource("Employees", items));
            // �ͦ�����
            var result = report.Render("PDF");
            // �^���ɮ�
            return File(result, MediaTypeNames.Application.Pdf, "���u��ƪ�.pdf");
        }
    }
}
