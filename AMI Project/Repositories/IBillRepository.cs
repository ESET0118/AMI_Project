using AMI_Project.Data.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IBillRepository
    {
        Task<Bill> CreateAsync(Bill bill);
        Task<IEnumerable<Bill>> GetAllAsync();
        Task<Bill?> GetByIdAsync(long id);
        Task<IEnumerable<Bill>> GetByMeterAsync(string meterSerialNo);
        Task<IEnumerable<Bill>> GetByConsumerAsync(long consumerId);
        Task<IEnumerable<Bill>> GetUnpaidByConsumerAsync(long consumerId);
        Task<Tariff?> GetTariffByConsumerIdAsync(long consumerId);
        Task UpdateAsync(Bill bill);
    }
}
