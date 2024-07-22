using System.DrawingCore;
using System.DrawingCore.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.ZKWeb;

namespace KH.Lab.MyRDLC.Reports.Helpers;

/// <summary>
/// QR Code 一維碼、二維碼
/// </summary>
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
