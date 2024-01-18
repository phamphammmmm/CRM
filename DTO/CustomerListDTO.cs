using Customer_Relationship_Managament.Models;

namespace Customer_Relationship_Managament.DTO
{
    public class CustomerListDTO
    {
        public List<Customer> Customers { get; set; } 
        public int TotalPages { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public bool? Gender { get; set; }
        public bool? IsAsc { get; set; }
        public string ColName { get; set; } = string.Empty;
        public int Index { get; set; }
        public int Size { get; set; }
    }
}
