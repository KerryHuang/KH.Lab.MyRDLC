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

        public IActionResult List()
        {
            // ���J����
            var report = new LocalReport();
            // ���o������
            var items = _northwindcontext.Orders.ToList().ToDataTable();
            // ���o������|
            var assembly = typeof(Reports.Const).Assembly;
            using var rs = assembly.GetManifestResourceStream("KH.Lab.MyRDLC.Reports.RDLCs.Report2.rdlc");
            report.LoadReportDefinition(rs);
            // �]�w�����ƨӷ�
            report.DataSources.Add(new ReportDataSource("Orders", items));
            // �ͦ�����
            var result = report.Render("PDF");
            // �^���ɮ�
            return File(result, MediaTypeNames.Application.Pdf, "�t�e��.pdf");
        }

        public IActionResult List2()
        {
            // ���J����
            var report = new LocalReport();
            // ���o������|
            var assembly = typeof(Reports.Const).Assembly;
            using var rs = assembly.GetManifestResourceStream("KH.Lab.MyRDLC.Reports.RDLCs.Report3.rdlc");
            report.LoadReportDefinition(rs);
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
            report.DataSources.Add(new ReportDataSource("DeliveryOrder", ds.Tables["DeliveryOrder"]));
            // �ͦ�����
            var result = report.Render("PDF");
            // �^���ɮ�
            return File(result, MediaTypeNames.Application.Pdf, "�t�e��2.pdf");
        }
    }
}
