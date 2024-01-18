using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Customer_Relationship_Managament.Data;
using Customer_Relationship_Managament.Models;
using Microsoft.AspNetCore.Authorization;
using Customer_Relationship_Managament.Services.Customers.Interfaces;
using Customer_Relationship_Managament.Repositories.Customers;
using OfficeOpenXml;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Azure;

namespace Customer_Relationship_Managament.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerService _customerService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IQrCodeService _qrCodeService;

        public CustomersController(ApplicationDbContext context, ICustomerService customerService, IQrCodeService qrCodeService, ICustomerRepository customerRepository)
        {
            _context = context;
            _customerService = customerService;
            _qrCodeService = qrCodeService;
            _customerRepository = customerRepository;
        }


        [AllowAnonymous]
        // GET: Customers
        public async Task<IActionResult> Index(string keyword = "", bool? gender = null,
                                       string colName = "Id", bool? isAsc = true,
                                       int index = 1, int size = 3)
        {
            var result = await _customerService.GetCustomers(keyword, gender, colName, isAsc, index, size);

            ViewBag.keyword = result.Keyword;
            ViewBag.gender = result.Gender;
            ViewBag.isAsc = result.IsAsc;
            ViewBag.colName = result.ColName;
            ViewBag.index = result.Index;
            ViewBag.size = result.Size;
            ViewBag.totalPages = result.TotalPages;

            return View(result.Customers);
        }


        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerById(id.Value);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Country,Gender,DateOfBirth,QRCodeURL")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var createResult = await _customerService.CreateCustomer(customer);

                if (createResult != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi khi tạo khách hàng.");
                }
            }

            // ModelState không hợp lệ hoặc có lỗi khi tạo khách hàng
            return View(customer);
        }


        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerById(id.Value);

            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Country,Gender,DateOfBirth,QRCodeURL")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updateResult = await _customerService.UpdateCustomer(id,customer);
                if (updateResult)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _customerService.GetCustomerById(id.Value);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerById(id);

                if (customer == null)
                {
                    return NotFound(); 
                }

                _qrCodeService.DeleteQrCodeImage(id);
                await _customerService.DeleteCustomer(customer);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        //Export data from excel
        public async Task<IActionResult> ExportToExcel()
        {
            var customers = await _customerRepository.ExportToExcel();

            using (var stream = new MemoryStream())
            {
                _customerService.ExportExcel(customers, stream);
                var fileName = $"Customers_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        
        [HttpPost]
        public IActionResult ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            try
            {
                _customerService.ImportFromExcel(file);

                TempData["message"] = "Customers imported successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error importing data: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReadDataFromCCCD(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File không hop lệ.");
            }

            try
            {
                _customerService.ReadDataFromCCCD(file);

                TempData["message"] = "Đọc thông tin thành công!";
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["error"] = $"Error importing data: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
