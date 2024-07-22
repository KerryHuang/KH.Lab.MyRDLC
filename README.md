# ASP.NET Core RDLC ����]�p

### 1. �إ߱M��

�����A�Ыؤ@�ӷs�� .NET 8 Web ���ε{�ǱM�סG

```
dotnet new web -n RdlcReportApp
cd RdlcReportApp
```

### 2. �w�˩����Ҳ�

1. �u��C��W���[�����Ҳ�]>>[�޲z�����Ҳ�]�C
2. �j����RDLC���A���[Microsoft RDLC Reader Designer]�I�U���C

### 3. �d�Ҹ�Ʈw�U��

1. [AdventureWorks �d�Ҹ�Ʈw](https://learn.microsoft.com/zh-tw/sql/samples/adventureworks-install-configure?view=sql-server-ver16&tabs=ssms)
2. [Northwind �d�Ҹ�Ʈw](https://learn.microsoft.com/zh-tw/dotnet/framework/data/adonet/sql/linq/downloading-sample-databases#get-the-northwind-sample-database-for-sql-server)

### 4. �w�˥��n�� NuGet �M��

�w�˥H�U NuGet �M��H��� RDLC ����G

�ϥ�`ReportViewerCore.NETCore`
```
dotnet add package ReportViewerCore.NETCore
```
�ϥ�`AspNetCore.Reporting`
```
dotnet add package AspNetCore.Reporting
dotnet add package System.Drawing.Common
dotnet add package System.Security.Permissions
```

### 5. �K�[ RDLC ����

�b Visual Studio ���A�K�[�@�ӷs�� RDLC ������G

1. �k���I���M�סA�b���K�[���ﶵ����ܡ��s���ء��C
2. �j���������A��ܡ������]RDLC�^�A�éR�W�� `Report1.rdlc`�C
3. ���} `Report1.rdlc` ���A�]�p�z������C

### 6. �]�m��Ʒ�

�]�m��Ʒ��Ӷ�R��������ơG

1. �b����]�p�����A�k���I������ƶ����ÿ�ܡ��[�J��ƶ����C
2. ��ܡ���ƶ����A�M��t�m�z����Ʒ��]�Ҧp�A�Ӧ۸�Ʈw����ơ^�C

### 7. �b������ͦ�����

�b ASP.NET Core ������ͦ������ RDLC ����G

#### �[�J�@�ӱ��

�Ыؤ@�ӷs����� `ReportsController.cs`�G

�ϥ�`ReportViewerCore.NETCore`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Diagnostics;
using System.Net.Mime;

namespace RdlcReportApp.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
```

�ϥ�`AspNetCore.Reporting`

```csharp
using AspNetCore.Reporting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mime;

namespace RdlcReportApp.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
```

### 8. �]�m�˵���

�إ߳���U���s��(PDF �P Excel)�G

```csharp
@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">Report</h1>
    <a asp-action="Print" asp-controller="Home">Export PDF</a>
    <a asp-action="Export" asp-controller="Home">Export Excel</a>
</div>
```

### 9. �t�m�A��

�b `Program.cs` ���A�t�m�A�ȥH��� RDLC ����G

```csharp
var builder = WebApplication.CreateBuilder(args);

// �W�[�A��
builder.Services.AddControllersWithViews();

var app = builder.Build();

// �t�m������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### 10. �B�����ε{��

�B�����ε{���óX�� `/reports` ���|�A���ӯ���ͦ������ RDLC ����C

### �`��

�o�ӱоǮi�ܤF�p��b .NET 8 ���]�m�M�ϥ� RDLC ����A�]�A�ЫرM�סB�]�m��Ʒ��B�ͦ�����M�t�m���ѡC�ھڱz���ݨD�A�z�i�H�i�@�B�ۭq�����e�M��Ʒ��C

### ���X�R�о�

�t�e��˦�����

1. �إ߸�ƶ�
2. �إ�RDLC����
3. BarCode �P QRCode �Ϥ��B�z
4. �{���X

�ϥ�`ReportViewerCore.NETCore`

```csharp
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
```

�ϥ�`AspNetCore.Reporting`

```csharp
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
```

### ���X�R���O

1. ��ƪ�B�z

�إ��R�A���O�P��k

```csharp
public static class DataTableHelper
{
    public static DataTable ToDataTable<T>(this List<T> items)
    {
        var dataTable = new DataTable(typeof(T).Name);
        //Get all the properties
        PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in Props)
        {
            Type type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            type = type.IsEnum ? Enum.GetUnderlyingType(type) : type;
            //Setting column names as Property names
            dataTable.Columns.Add(prop.Name, type);
        }
        foreach (T item in items)
        {
            var values = new object[Props.Length];
            for (int i = 0; i < Props.Length; i++)
            {
                //inserting property values to datatable rows
                values[i] = Props[i].GetValue(item, null) ?? DBNull.Value;
            }
            dataTable.Rows.Add(values);
        }
        //put a breakpoint here and check datatable
        return dataTable;
    }
}
```

2.  QR Code �@���X�B�G���X

�s�WNuget�M��

```
dotnet add package ZXing.Net
dotnet add package ZXing.Net.Bindings.ZKWeb.System.Drawing
dotnet add package System.Drawing.Common
```

�إ��R�A���O�P��k

```csharp
public static class BarcodeHelper
{
    /// <summary>
    ///  ���ͤ@���X
    /// </summary>
    /// <param name="content">���e</param>
    /// <param name="width">�e��</param>
    /// <param name="height">����</param>
    /// <param name="purebarcode">���e����bBarCode�U</param>
    public static byte[] CreateBarCodeBytes(string content, int width = 300, int height = 100, bool purebarcode = true)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        var writer = new BarcodeWriter
        {
            // �n���ͪ�Code����
            Format = BarcodeFormat.CODE_128,
            // ���͹ϧΪ��ݩ�
            Options = new EncodingOptions()
            {
                Margin = 0,
                Height = height,
                Width = width,
                PureBarcode = purebarcode
            }
        };
        var b2 = writer.Write(content);
        return BitmapToArray(b2);
    }

    /// <summary>
    /// ���ͤG���X
    /// </summary>
    /// <param name="content">���e</param>
    /// <param name="width">�e��</param>
    /// <param name="height">����</param>
    /// <returns></returns>
    public static byte[] CreateQRCodeBytes(string content, int width = 250, int height = 250)
    {
        int heig = width;
        if (width > height)
        {
            heig = height;
            width = height;
        }
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        var writer = new BarcodeWriter
        {
            //�n���ͪ�Code����
            Format = BarcodeFormat.QR_CODE,
            //���͹ϧΪ��ݩ�
            Options = new QrCodeEncodingOptions()
            {
                Margin = 0,
                Height = heig,   //�ϧΪ���
                Width = width,    //�ϧΪ��e
                CharacterSet = "UTF-8",  //�s�X�榡 UTF-8  ����~���|�X�{��
                ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M  //���~�ץ����e
            }
        };
        var bm = writer.Write(content);
        return BitmapToArray(bm);
    }

    /// <summary>
    /// �N Bitmap �ഫ�� byte[] ����k
    /// </summary>
    /// <param name="bmp">The BMP.</param>
    /// <returns></returns>
    public static byte[] BitmapToArray(Bitmap bmp)
    {
        byte[] byteArray = null;
        using (var stream = new MemoryStream())
        {
            bmp.Save(stream, ImageFormat.Jpeg);
            byteArray = stream.GetBuffer();
        }
        return byteArray;
    }
}
```