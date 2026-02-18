namespace WebAppOrazioAlessandro.DTOs.Stand
{
    public class CreateStandDto
    {
        public string Nome { get; set; } = string.Empty;
        public int SettoreId { get; set; }
        public int CategoriaMerceologicaId { get; set; }
    }
}
