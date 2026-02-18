namespace WebAppOrazioAlessandro.DTOs.Stand
{
    public class StandResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public int SettoreId { get; set; }
        public string SettoreNome { get; set; } = string.Empty;

        public int CategoriaMerceologicaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
    }
}
