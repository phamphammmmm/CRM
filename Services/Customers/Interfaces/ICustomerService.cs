using Customer_Relationship_Managament.DTO;
using Customer_Relationship_Managament.Models;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Relationship_Managament.Services.Customers.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerListDTO> GetCustomers(string keyword, bool? gender, string colName, bool? isAsc, int index, int size);
        Task<Customer> GetCustomerById(int id);
        Task<bool> CreateCustomer(Customer customer);
        Task<bool> UpdateCustomer(int id, Customer customer);
        Task<bool> DeleteCustomer(Customer customer);
        Task ReadDataFromCCCD(IFormFile file);
        void ImportFromExcel(IFormFile file);
        void ExportExcel(List<Customer> customers, Stream stream);
    }
}
