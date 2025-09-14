namespace PruebaTecnica.Application.DTOs
{
    public class BetRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string BetType { get; set; } = string.Empty;  
        public string? Color { get; set; }                
        public bool? IsEven { get; set; }                 
        public int? Number { get; set; }                 
    }
}
