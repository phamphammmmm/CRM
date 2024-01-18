using Customer_Relationship_Managament.DTO;
using Customer_Relationship_Managament.Models;
using Microsoft.AspNetCore.Mvc;

namespace Customer_Relationship_Managament.Repositories.Customers
{
    public interface ICustomerRepository
    {
        Task<CustomerListDTO> GetCustomers(string keyword,
                                          bool? gender,
                                          string colName,
                                          bool? isAsc,
                                          int index,
                                          int size);
        Task<Customer> GetCustomerById(int id);
        Task<Customer> CreateCustomer(Customer customer);
        Task<bool> UpdateCustomer(Customer customer);
        Task<bool> DeleteCustomer(Customer customer);
        void ReadDataFromCCCD(Customer customer);
        Task<List<Customer>> ExportToExcel();
        void AddCustomers(List<Customer> customers);    
        void DetachEntity(Customer? oldCustomer);
    }
}
