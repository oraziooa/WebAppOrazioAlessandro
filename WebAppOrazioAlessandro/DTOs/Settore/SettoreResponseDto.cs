namespace WebAppOrazioAlessandro.DTOs.Settore
{
    public class SettoreResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int PadiglioneId { get; set; }
        public string PadiglioneNome { get; set; } = string.Empty;
    }
}
