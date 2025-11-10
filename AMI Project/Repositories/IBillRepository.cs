using AMI_Project.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IBillRepository
    {
        Task<Bill> CreateAsync(Bill bill);
        Task<IEnumerable<Bill>> GetAllAsync();
        Task<Bill?> GetByIdAsync(long id);

        // Add this
        Task<Tariff?> GetTariffByConsumerIdAsync(long consumerId);
    }
}
