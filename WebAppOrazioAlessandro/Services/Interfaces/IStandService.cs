using WebAppOrazioAlessandro.DTOs.Stand;

namespace WebAppOrazioAlessandro.Services.Interfaces
{
    public interface IStandService
    {
        Task<IEnumerable<StandResponseDto>> GetAllAsync();
        Task<StandResponseDto?> GetByIdAsync(int id);
        Task<StandResponseDto> CreateAsync(CreateStandDto dto);
        Task<StandResponseDto?> UpdateAsync(int id, CreateStandDto dto);
        Task<bool> DeleteAsync(int id);
    }

}
