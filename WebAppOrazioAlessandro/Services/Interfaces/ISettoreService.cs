using WebAppOrazioAlessandro.DTOs.Settore;

namespace WebAppOrazioAlessandro.Services.Interfaces
{
    public interface ISettoreService
    {
        Task<IEnumerable<SettoreResponseDto>> GetAllAsync();
        Task<SettoreResponseDto?> GetByIdAsync(int id);
        Task<SettoreResponseDto> CreateAsync(CreateSettoreDto dto);
        Task<SettoreResponseDto?> UpdateAsync(int id, CreateSettoreDto dto);
        Task<bool> DeleteAsync(int id);
    }

}
