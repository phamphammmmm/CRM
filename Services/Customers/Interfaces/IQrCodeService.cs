using Customer_Relationship_Managament.Models;

namespace Customer_Relationship_Managament.Services.Customers.Interfaces
{
    public interface IQrCodeService
    {
        string GenerateQRCodeUrl(Customer customer);
        void DeleteQrCodeImage(int id);
    }
}
