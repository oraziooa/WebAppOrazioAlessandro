namespace WebAppOrazioAlessandro.Entities
{
    public class Padiglione
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public ICollection<Settore> Settori { get; set; } = new List<Settore>();
    }
}
