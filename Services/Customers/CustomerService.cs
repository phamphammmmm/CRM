using Customer_Relationship_Managament.DTO;
using Customer_Relationship_Managament.Models;
using Customer_Relationship_Managament.Repositories.Customers;
using Customer_Relationship_Managament.Services.Customers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;

namespace Customer_Relationship_Managament.Services.Customers.Interfaces
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IQrCodeService _qrCodeService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomerService(ICustomerRepository customerRepository, IQrCodeService qrCodeService,IWebHostEnvironment webHostEnvironment)
        {
            _customerRepository = customerRepository;
            _qrCodeService = qrCodeService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> CreateCustomer(Customer customer)
        {
            try
            {
                var newCustomer = await _customerRepository.CreateCustomer(customer);

                if (newCustomer != null)
                {
                    // Cập nhật giá trị mới cho trường QRCodeURL
                    newCustomer.QRCodeURL = _qrCodeService.GenerateQRCodeUrl(newCustomer);
                    if (newCustomer.QRCodeURL != null)
                    {
                        Console.WriteLine("éo có ảnh");
                    }

                    // Cập nhật lại thông tin của khách hàng với trường QRCodeURL mới
                    await _customerRepository.UpdateCustomer(newCustomer);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCustomer(Customer customer)
        {
            return await _customerRepository.DeleteCustomer(customer);
        }

        public void ExportExcel(List<Customer> customers, Stream stream)
        {
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Customers");

                // Header row
                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Full Name";
                worksheet.Cells["C1"].Value = "Country";
                worksheet.Cells["D1"].Value = "Gender";
                worksheet.Cells["E1"].Value = "Date of Birth";

                // Data rows
                for (var i = 0; i < customers.Count; i++)
                {
                    var row = i + 2; // Start from row 2 (1 for header)
                    var customer = customers[i];

                    worksheet.Cells[$"A{row}"].Value = customer.Id;
                    worksheet.Cells[$"B{row}"].Value = customer.FullName;
                    worksheet.Cells[$"C{row}"].Value = customer.Country;
                    worksheet.Cells[$"D{row}"].Value = customer.Gender.HasValue ? (customer.Gender.Value ? "Male" : "Female") : "Unknown";
                    worksheet.Cells[$"E{row}"].Value = customer.DateOfBirth.HasValue
                        ? customer.DateOfBirth.Value.ToShortDateString() : "Unknown";
                }

                // Auto-fit columns for better readability
                worksheet.Cells.AutoFitColumns();

                // Style header row
                using (var range = worksheet.Cells["A1:E1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                package.Save();
            }
        }

        public Task<Customer> GetCustomerById(int id)
        {
            return _customerRepository.GetCustomerById(id);
        }

        public async Task<CustomerListDTO> GetCustomers(string keyword, bool? gender, string colName, bool? isAsc, int index, int size)
        {
            return await _customerRepository.GetCustomers(keyword, gender, colName, isAsc, index, size);
        }

        public void ImportFromExcel(IFormFile file)
        {
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet

                int rowCount = worksheet.Dimension.Rows;

                List<Customer> importedCustomers = new List<Customer>();

                for (int row = 2; row <= rowCount; row++) // Assuming first row is header
                {
                    var newCustomer = new Customer
                    {
                        FullName = worksheet.Cells[row, 2].Value?.ToString(),
                        Country = worksheet.Cells[row, 3].Value?.ToString(),
                        Gender = worksheet.Cells[row, 4].Value?.ToString() == "Male",
                        DateOfBirth = worksheet.Cells[row, 5].Value != null ?
                                        DateTime.ParseExact(worksheet.Cells[row, 5].Value.ToString(),
                                        "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null
                    };
                    importedCustomers.Add(newCustomer);
                }
                _customerRepository.AddCustomers(importedCustomers);
            }
        }

        public async Task ReadDataFromCCCD(IFormFile file)
        {
            var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "upload");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            var filePath = Path.Combine(uploads, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent())
            {
                var api_key = "cPwObuutd9l2kQJIsDJJszsB729DLU2h";
                var api_url = "https://api.fpt.ai/vision/idr/vnm";
                // Thêm api-key vào header
                client.DefaultRequestHeaders.Add("api-key", api_key);

                // Thêm ảnh vào content từ file đã lưu
                var imageContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                content.Add(imageContent, "image", file.FileName);

                // Gửi yêu cầu PO
                var response = await client.PostAsync(api_url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ReadCCCD_DTO>(responseString);
                    Console.WriteLine(resultObject.data[0].name);

                    if (resultObject == null)
                    {
                        Console.WriteLine("resultObject is null");
                    }

                    var newCustomer = new Customer();
                        newCustomer.FullName = resultObject.data[0].name;
                        newCustomer.Country = resultObject.data[0].nationality;
                        newCustomer.Gender = resultObject.data[0].sex.ToLower().Equals("nam") ? true : false;
                        newCustomer.DateOfBirth = DateTime.ParseExact(resultObject.data[0].dob, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        _customerRepository.ReadDataFromCCCD(newCustomer);

                    if(newCustomer != null)
                    {
                        newCustomer.QRCodeURL = _qrCodeService.GenerateQRCodeUrl(newCustomer);
                        Console.WriteLine("Your image's link: " + newCustomer.QRCodeURL);
                    }
                }
            }
        }

        public async Task<bool> UpdateCustomer(int id, Customer customer)
        {
            var oldCustomer = await _customerRepository.GetCustomerById(id);

            if (oldCustomer != null)
            {
                // Delete the old QR Code image
                _qrCodeService.DeleteQrCodeImage(id);
            }

            _customerRepository.DetachEntity(oldCustomer);

            // Update customer's information
            await _customerRepository.UpdateCustomer(customer);

            _qrCodeService.GenerateQRCodeUrl(customer);

            return true;
        }
    }
}
