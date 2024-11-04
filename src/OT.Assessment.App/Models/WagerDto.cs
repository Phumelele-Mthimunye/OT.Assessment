namespace OT.Assessment.App.Models
{
    public class WagerDto
    {
        public Guid WagerId { get; set; }
        public string Game { get; set; }
        public string Provider { get; set; }
        public double? Amount { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
