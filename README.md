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

這個教學展示了如何在 .NET 8 中設置和使用 RDLC 報表，包括創建專案、設置數據源、生成報表和配置路由。根據您的需求，您可以進一步自訂報表內容和數據源。