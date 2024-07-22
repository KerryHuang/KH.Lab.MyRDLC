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

        public IActionResult List()
        {
            // ���o������|
            var reportPath = Path.Combine(AppContext.BaseDirectory, "RDLCs", "Report2.rdlc");
            // ���J����
            var report = new LocalReport(reportPath);
            // ���o������
            var items = _northwindcontext.Orders.ToList().ToDataTable();
            // �]�w�����ƨӷ�
            report.AddDataSource(nameof(Orders), items);
            // �ͦ�����
            var result = report.Execute(RenderType.Pdf);
            // �^���ɮ�
            return File(result.MainStream, MediaTypeNames.Application.Pdf, "�t�e��.pdf");
        }

        public IActionResult List2()
        {
            // ���o������|
            var reportPath = Path.Combine(AppContext.BaseDirectory, "RDLCs", "Report3.rdlc");
            // ���J����
            var report = new LocalReport(reportPath);
            // ���o������
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
            // �]�w�����ƨӷ�
            report.AddDataSource("DeliveryOrder", ds.Tables["DeliveryOrder"]);
            // �ͦ�����
            var result = report.Execute(RenderType.Pdf);
            // �^���ɮ�
            return File(result.MainStream, MediaTypeNames.Application.Pdf, "�t�e��2.pdf");
        }
    }
}
