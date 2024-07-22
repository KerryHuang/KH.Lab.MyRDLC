# ASP.NET Core RDLC 報表設計

### 1. 建立專案

首先，創建一個新的 .NET 8 Web 應用程序專案：

```
dotnet new web -n RdlcReportApp
cd RdlcReportApp
```

### 2. 安裝延伸模組

1. 工具列表上選取[延伸模組]>>[管理延伸模組]。
2. 搜索“RDLC”，選取[Microsoft RDLC Reader Designer]點下載。

### 3. 範例資料庫下載

1. [AdventureWorks 範例資料庫](https://learn.microsoft.com/zh-tw/sql/samples/adventureworks-install-configure?view=sql-server-ver16&tabs=ssms)
2. [Northwind 範例資料庫](https://learn.microsoft.com/zh-tw/dotnet/framework/data/adonet/sql/linq/downloading-sample-databases#get-the-northwind-sample-database-for-sql-server)

### 4. 安裝必要的 NuGet 套件

安裝以下 NuGet 套件以支持 RDLC 報表：

使用`ReportViewerCore.NETCore`
```
dotnet add package ReportViewerCore.NETCore
```
使用`AspNetCore.Reporting`
```
dotnet add package AspNetCore.Reporting
dotnet add package System.Drawing.Common
dotnet add package System.Security.Permissions
```

### 5. 添加 RDLC 報表

在 Visual Studio 中，添加一個新的 RDLC 報表文件：

1. 右鍵點擊專案，在“添加”選項中選擇“新項目”。
2. 搜索“報表”，選擇“報表”（RDLC），並命名為 `Report1.rdlc`。
3. 打開 `Report1.rdlc` 文件，設計您的報表。

### 6. 設置資料源

設置資料源來填充報表中的資料：

1. 在報表設計器中，右鍵點擊“資料集”並選擇“加入資料集”。
2. 選擇“資料集”，然後配置您的資料源（例如，來自資料庫的資料）。

### 7. 在控制器中生成報表

在 ASP.NET Core 控制器中生成並顯示 RDLC 報表：

#### 加入一個控制器

創建一個新的控制器 `ReportsController.cs`：

使用`ReportViewerCore.NETCore`

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
```

使用`AspNetCore.Reporting`

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
    }
}
```

### 8. 設置檢視頁

建立報表下載連結(PDF 與 Excel)：

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

### 9. 配置服務

在 `Program.cs` 中，配置服務以支持 RDLC 報表：

```csharp
var builder = WebApplication.CreateBuilder(args);

// 增加服務
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 配置中間件
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

### 10. 運行應用程式

運行應用程式並訪問 `/reports` 路徑，應該能夠生成並顯示 RDLC 報表。

### 總結

這個教學展示了如何在 .NET 8 中設置和使用 RDLC 報表，包括創建專案、設置資料源、生成報表和配置路由。根據您的需求，您可以進一步自訂報表內容和資料源。

### ★擴充教學

配送單樣式報表

1. 建立資料集
2. 建立RDLC報表
3. BarCode 與 QRCode 圖片處理
4. 程式碼

使用`ReportViewerCore.NETCore`

```csharp
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
```

使用`AspNetCore.Reporting`

```csharp
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
```

### ★擴充類別

1. 資料表處理

建立靜態類別與方法

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

2.  QR Code 一維碼、二維碼

新增Nuget套件

```
dotnet add package ZXing.Net
dotnet add package ZXing.Net.Bindings.ZKWeb.System.Drawing
dotnet add package System.Drawing.Common
```

建立靜態類別與方法

```csharp
public static class BarcodeHelper
{
    /// <summary>
    ///  產生一維碼
    /// </summary>
    /// <param name="content">內容</param>
    /// <param name="width">寬度</param>
    /// <param name="height">高度</param>
    /// <param name="purebarcode">內容不放在BarCode下</param>
    public static byte[] CreateBarCodeBytes(string content, int width = 300, int height = 100, bool purebarcode = true)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        var writer = new BarcodeWriter
        {
            // 要產生的Code類型
            Format = BarcodeFormat.CODE_128,
            // 產生圖形的屬性
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
    /// 產生二維碼
    /// </summary>
    /// <param name="content">內容</param>
    /// <param name="width">寬度</param>
    /// <param name="height">高度</param>
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
            //要產生的Code類型
            Format = BarcodeFormat.QR_CODE,
            //產生圖形的屬性
            Options = new QrCodeEncodingOptions()
            {
                Margin = 0,
                Height = heig,   //圖形的高
                Width = width,    //圖形的寬
                CharacterSet = "UTF-8",  //編碼格式 UTF-8  中文才不會出現亂
                ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M  //錯誤修正內容
            }
        };
        var bm = writer.Write(content);
        return BitmapToArray(bm);
    }

    /// <summary>
    /// 將 Bitmap 轉換為 byte[] 的方法
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