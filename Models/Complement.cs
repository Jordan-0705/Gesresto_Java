using Models.Enums;

namespace Models
{
    public class Complement
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string? Photo { get; set; }
        public string Nom { get; set; }
        public decimal Prix { get; set; }
        public ComplementType ComplementType { get; set; }
        public EtatProduit Etat { get; set; }
    }
}
