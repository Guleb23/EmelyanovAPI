namespace EmelyanovDiplom.Models
{
    public class Uslugi
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int Provider { get; set; }

        public int Category { get; set; }
    }
}




 
