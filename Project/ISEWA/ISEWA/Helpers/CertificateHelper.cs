using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Web;

public class CertificateBgHelper : PdfPageEventHelper
{
    public override void OnEndPage(PdfWriter writer, Document document)
    {
        string bgPath = HttpContext.Current.Server.MapPath("~/Certificates/Certificate_Background.png");

        if (File.Exists(bgPath))
        {
            Image bg = Image.GetInstance(bgPath);
            bg.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
            bg.SetAbsolutePosition(0, 0);

            PdfContentByte canvas = writer.DirectContentUnder;
            canvas.AddImage(bg);
        }
    }
}
