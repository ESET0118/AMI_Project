using AMI_Project.DTOs.Consumers;

public interface IConsumerService
{
    Task<IEnumerable<ConsumerReadDto>> GetAllAsync(CancellationToken ct);
    Task<ConsumerReadDto?> GetByIdAsync(long id, CancellationToken ct);
    Task<ConsumerReadDto> CreateAsync(ConsumerCreateDto dto, string createdBy, CancellationToken ct);
    Task<ConsumerReadDto?> UpdateAsync(long id, ConsumerUpdateDto dto, string updatedBy, CancellationToken ct);
    Task DeleteAsync(long id, CancellationToken ct);
}
