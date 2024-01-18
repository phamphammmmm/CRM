using System;
using System.IO;
using Customer_Relationship_Managament.Models;
using Customer_Relationship_Managament.Services.Customers.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using QRCoder;

namespace Customer_Relationship_Managament.Services.Customers
{
    public class QrCodeService : IQrCodeService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public QrCodeService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        public string GenerateQRCodeUrl(Customer customer)
        {
            var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "qrcodes");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var customerDataJson = JsonConvert.SerializeObject(customer);

            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(customerDataJson, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(data);
            var qrCodeBytes = qrCode.GetGraphic(20);
            var fileName = $"{customer.Id}_qrcode.png";
            var filePath = Path.Combine(directoryPath, fileName);

            // Ghi hình ảnh QR Code vào thư mục
            System.IO.File.WriteAllBytes(filePath, qrCodeBytes);

            // Utilize _urlHelperFactory to create an IUrlHelper
            var actionContext = _actionContextAccessor.ActionContext;
            var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);

            // Sử dụng IUrlHelper để tạo URL
            var qrCodeUrl = urlHelper.Content($"~/qrcodes/{fileName}");
            Console.WriteLine("Generated QR Code URL: " + qrCodeUrl);

            return qrCodeUrl;
        }
        public void DeleteQrCodeImage(int id)
        {
            var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "qrcodes");
            var fileName = $"{id}_qrcode.png";
            var filePath = Path.Combine(directoryPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
