namespace Selu383.SP25.P02.Api.Models
{
    public class Theater
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int SeatCount { get; set; }
        public int? ManagerId { get; set; } 
        public ApplicationUser? Manager { get; set; } 
    }

}
