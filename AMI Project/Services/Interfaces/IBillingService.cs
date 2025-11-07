using AMI_Project.DTOs.Billing;

namespace AMI_Project.Services.Interfaces
{
    public interface IBillingService
    {
        Task<IEnumerable<BillReadDto>> GetAllBillsAsync();
        Task<BillReadDto?> GetBillByIdAsync(long id);
        Task<BillReadDto> CreateBillAsync(BillCreateDto dto);
        Task DeleteBillAsync(long id);
    }
}
