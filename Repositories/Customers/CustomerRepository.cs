using Customer_Relationship_Managament.Data;
using Customer_Relationship_Managament.DTO;
using Customer_Relationship_Managament.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Customer_Relationship_Managament.Repositories.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        public readonly ApplicationDbContext _context;
        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Customer> CreateCustomer(Customer customer)
        {
            try
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();

                // Lấy thông tin mới được thêm vào cơ sở dữ liệu
                var newCustomer = await _context.Customers.FindAsync(customer.Id);

                return newCustomer;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteCustomer(Customer customer)
        {
            try
            {
                _context.Remove(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<List<Customer>> ExportToExcel()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<CustomerListDTO> GetCustomers(string keyword, bool? gender, string colName, bool? isAsc, int index, int size)
        {
            var result = _context.Customers.AsQueryable();

            //Filter by keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                result = result.Where(r => r.FullName.ToLower().Contains(keyword.ToLower())
                                       || r.Country.ToLower().Contains(keyword.ToLower()));
            }

            //Filter by gender
            if (gender != null)
            {
                result = result.Where(r => r.Gender == gender);
            }

            //Sorting
            if (!string.IsNullOrEmpty(colName) && isAsc != null)
            {
                result = colName switch
                {
                    "Id" => isAsc == true ? result.OrderBy(c => c.Id) : result.OrderByDescending(c => c.Id),
                    "FullName" => isAsc == true ? result.OrderBy(c => c.FullName) : result.OrderByDescending(c => c.FullName),
                    "Country" => isAsc == true ? result.OrderBy(c => c.Country) : result.OrderByDescending(c => c.Country),
                    "Gender" => isAsc == true ? result.OrderBy(c => c.Gender) : result.OrderByDescending(c => c.Gender),
                    "DateOfBirth" => isAsc == true ? result.OrderBy(c => c.DateOfBirth) : result.OrderByDescending(c => c.DateOfBirth),
                    _ => result,
                };
            }

            //Panigation
            var total = await result.CountAsync();
            var totalPage = total / size;
            if (total % size > 0)
            {
                totalPage++;
            }
            var customers = await result.Skip((index - 1) * size).Take(size).ToListAsync();

            // Create CustomerResult object
            var customerResult = new CustomerListDTO
            {
                Customers = customers,
                TotalPages = totalPage,
                Keyword = keyword,
                Gender = gender,
                IsAsc = isAsc,
                ColName = colName,
                Index = index,
                Size = size
            };

            return customerResult;
        }

        public void ReadDataFromCCCD(Customer customer)
        {
                _context.Customers.Add(customer);
                _context.SaveChanges();       
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public void DetachEntity(Customer entity)
        {
            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Added)
            {
                entry.State = EntityState.Detached;
            }
            else
            {
                // If the entity is not in the Added state, use the DbContext to detach it
                _context.Entry(entity).State = EntityState.Detached;
            }
        }

        public void AddCustomers(List<Customer> customers)
        {
                _context.Customers.AddRange(customers);
                _context.SaveChanges();
        }
    }
}
