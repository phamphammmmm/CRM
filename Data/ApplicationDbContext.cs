using Customer_Relationship_Managament.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Customer_Relationship_Managament.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            this.initData();
        }
        public DbSet<Customer> Customers { get; set; }
        private void initData()
        {
            if (this.Customers.Count() <= 0)
            {
                var c1 = new Customer()
                {
                    FullName = "Nguyen Van A",
                    Country = "VN",
                    DateOfBirth = DateTime.Parse("2002-12-20"),
                    Gender = true
                };
                var c2 = new Customer()
                {
                    FullName = "Nguyen Van B",
                    Country = "Mexico",
                    DateOfBirth = DateTime.Parse("2002-12-20"),
                    Gender = true
                };
                var c3 = new Customer()
                {
                    FullName = "Nguyen Van C",
                    Country = "NewYork",
                    DateOfBirth = DateTime.Parse("2002-12-20"),
                    Gender = true
                };
                var c4 = new Customer()
                {
                    FullName = "Nguyen Van D",
                    Country = "VN",
                    DateOfBirth = DateTime.Parse("2002-12-20"),
                    Gender = false
                };
                var c5 = new Customer()
                {
                    FullName = "Nguyen Van E",
                    Country = "VN",
                    DateOfBirth = DateTime.Parse("2002-12-20"),
                    Gender = true
                };
                var c6 = new Customer()
                {
                    FullName = "Nguyen Van F",
                    Country = "VN",
                    DateOfBirth = DateTime.Parse("2002-12-20"),
                    Gender = false
                };

                this.Customers.Add(c1);
                this.Customers.Add(c2);
                this.Customers.Add(c3);
                this.Customers.Add(c4);
                this.Customers.Add(c5);
                this.Customers.Add(c6);

                this.SaveChanges();
            }
        }

    }
}

