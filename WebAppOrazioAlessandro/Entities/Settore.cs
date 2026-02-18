namespace WebAppOrazioAlessandro.Entities
{
    public class Settore
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public int PadiglioneId { get; set; }
        public Padiglione? Padiglione { get; set; }
        public ICollection<Stand> Stands { get; set; } = new List<Stand>();
    }
}
