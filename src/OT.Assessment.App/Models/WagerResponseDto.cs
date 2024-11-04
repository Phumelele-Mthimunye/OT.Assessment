namespace OT.Assessment.App.Models
{
    public class WagerResponseDto
    {
        public List<WagerDto> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
    }
}
