using AMI_Project.Models;

namespace AMI_Project.Repositories.Interfaces
{
    public interface IBillRepository
    {
        Task<IEnumerable<Bill>> GetAllAsync();
        Task<Bill?> GetByIdAsync(long id);
        Task<Bill> CreateAsync(Bill bill);
        Task UpdateAsync(Bill bill);
        Task DeleteAsync(long id);
    }
}
