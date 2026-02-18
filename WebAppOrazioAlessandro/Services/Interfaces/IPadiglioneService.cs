using WebAppOrazioAlessandro.DTOs.Padiglione;

namespace WebAppOrazioAlessandro.Services.Interfaces
{
    public interface IPadiglioneService
    {
        Task<IEnumerable<PadiglioneResponseDto>> GetAllAsync();
        Task<PadiglioneResponseDto?> GetByIdAsync(int id);
        Task<PadiglioneResponseDto> CreateAsync(CreatePadiglioneDto dto);
        Task<PadiglioneResponseDto?> UpdateAsync(int id, CreatePadiglioneDto dto);
        Task<bool> DeleteAsync(int id);
    }
}