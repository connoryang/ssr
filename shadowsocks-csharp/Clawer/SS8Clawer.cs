using Shadowsocks.Controller;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace Shadowsocks.Clawer
{
    class SS8Clawer
    {
        protected ShadowsocksController controller;
        protected static string DownloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp\\qrcode");
        private string[] QrUrls = new string[] { "https://en.ss8.fun/images/server02.png", "https://en.ss8.fun/images/server03.png" };

        public SS8Clawer(ShadowsocksController controller)
        {
            this.controller = controller;
            if (!Directory.Exists(DownloadPath))
            {
                Directory.CreateDirectory(DownloadPath);
            }
        }

        public void Fetch()
        {
            foreach (var url in QrUrls)
            {
                ProcessUrl(url);
            }
        }

        protected void ProcessUrl(string url)
        {
            var file = Download(url);
            var ssr = DecodeQr(file);
            AddServer(ssr);
        }

        protected string Download(string url)
        {
            string localFilename = Path.Combine(DownloadPath, Guid.NewGuid().ToString()+".bmp");
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, localFilename);
            }
            return localFilename;
        }

        protected string DecodeQr(string file)
        {
            var bmp = new Bitmap(file);
            var source = new BitmapLuminanceSource(bmp);
            var bitmap = new BinaryBitmap(new HybridBinarizer(source));
            QRCodeReader reader = new QRCodeReader();
            var result = reader.decode(bitmap);
            return result.Text;
        }

        protected void AddServer(string url)
        {
            var success = controller.AddServerBySSURL(url);
        }
    }
}
