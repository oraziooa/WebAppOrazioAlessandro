namespace WebAppOrazioAlessandro.Entities
{
    public class Stand
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public int SettoreId { get; set; }
        public Settore? Settore { get; set; }

        public int CategoriaMerceologicaId { get; set; }
        public CategoriaMerceologica? CategoriaMerceologica { get; set; }
    }
}
