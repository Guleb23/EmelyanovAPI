namespace EmelyanovDiplom.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int IdClient { get; set; }
        public int IdUslugi { get; set; }
        public DateTime DateOformleniya { get; set; }
    }
}

