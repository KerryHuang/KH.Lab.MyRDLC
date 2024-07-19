using AspNetCore.Reporting;
using KH.Lab.MyRDLC.Web.Helpers;
using KH.Lab.MyRDLC.Web.Models;
using Microsoft.AspNetCore.Mvc;
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
            // ���o������|
            var reportPath = Path.Combine(Directory.GetCurrentDirectory(), "RDLCs", "Report1.rdlc");
            // ���J����
            var report = new LocalReport(reportPath);
            // ���o������
            var items = _northwindcontext.Employees.ToList().ToDataTable();
            // �]�w�����ƨӷ�
            report.AddDataSource(nameof(Employees), items);
            // �ͦ�����
            var result = report.Execute(RenderType.Excel);
            // �^���ɮ�
            return File(result.MainStream, MediaTypeNames.Application.Octet, "���u��ƪ�.xlsx");
        }

        public IActionResult Print()
        {
            // ���o������|
            var reportPath = Path.Combine(AppContext.BaseDirectory, "RDLCs", "Report1.rdlc");
            // ���J����
            var report = new LocalReport(reportPath);
            // ���o������
            var items = _northwindcontext.Employees.ToList().ToDataTable();
            // �]�w�����ƨӷ�
            report.AddDataSource(nameof(Employees), items);
            // �ͦ�����
            var result = report.Execute(RenderType.Pdf);
            // �^���ɮ�
            return File(result.MainStream, MediaTypeNames.Application.Pdf, "���u��ƪ�.pdf");
        }
    }
}
