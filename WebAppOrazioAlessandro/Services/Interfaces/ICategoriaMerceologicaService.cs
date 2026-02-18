using WebAppOrazioAlessandro.DTOs.CategoriaMerceologica;

namespace WebAppOrazioAlessandro.Services.Interfaces
{
    public interface ICategoriaMerceologicaService
    {
        Task<IEnumerable<CategoriaResponseDto>> GetAllAsync();
        Task<CategoriaResponseDto?> GetByIdAsync(int id);
        Task<CategoriaResponseDto> CreateAsync(CreateCategoriaDto dto);
        Task<CategoriaResponseDto?> UpdateAsync(int id, CreateCategoriaDto dto);
        Task<bool> DeleteAsync(int id);
    }

}
