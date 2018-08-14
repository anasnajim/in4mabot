using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace PngToJpeg.Controllers
{
    public class AsJPGController : ApiController
    {
        public HttpResponseMessage Get(string url)
        {

            WebRequest req = WebRequest.Create(url);
            WebResponse response = req.GetResponse();
            Stream stream = response.GetResponseStream();
            int dataLength = (int)response.ContentLength;
            byte[] buffer = new byte[1024];
            MemoryStream memStream = new MemoryStream();

            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                memStream.Write(buffer, 0, bytesRead);
            }

            memStream.ToArray();


            Bitmap img = new Bitmap(memStream);
            using (MemoryStream ms = new MemoryStream())
            {
                img = Transparent2Color(img, Color.White);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(ms.ToArray());
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                return result;
            }
        }

        Bitmap Transparent2Color(Bitmap bmp1, Color target)
        {
            Bitmap bmp2 = new Bitmap(bmp1.Width, bmp1.Height);
            Rectangle rect = new Rectangle(Point.Empty, bmp1.Size);
            using (Graphics G = Graphics.FromImage(bmp2))
            {
                G.Clear(target);
                G.DrawImageUnscaledAndClipped(bmp1, rect);
            }
            return bmp2;
        }
    }
}
