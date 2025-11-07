using AMI_Project.DTOs.Billing;
using AMI_Project.Models;
using AMI_Project.Repositories.Interfaces;
using AMI_Project.Services.Interfaces;
using AutoMapper;

namespace AMI_Project.Services
{
    public class BillingService : IBillingService
    {
        private readonly IBillRepository _billRepo;
        private readonly IMapper _mapper;

        public BillingService(IBillRepository billRepo, IMapper mapper)
        {
            _billRepo = billRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BillReadDto>> GetAllBillsAsync()
        {
            var bills = await _billRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<BillReadDto>>(bills);
        }

        public async Task<BillReadDto?> GetBillByIdAsync(long id)
        {
            var bill = await _billRepo.GetByIdAsync(id);
            return bill == null ? null : _mapper.Map<BillReadDto>(bill);
        }

        public async Task<BillReadDto> CreateBillAsync(BillCreateDto dto)
        {
            var bill = _mapper.Map<Bill>(dto);
            bill.BillGeneratedAt = DateTime.UtcNow;

            var created = await _billRepo.CreateAsync(bill);
            return _mapper.Map<BillReadDto>(created);
        }

        public async Task DeleteBillAsync(long id)
        {
            await _billRepo.DeleteAsync(id);
        }
    }
}
