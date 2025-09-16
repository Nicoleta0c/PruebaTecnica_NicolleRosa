namespace PruebaTecnica.Application.DTOs
{
    // DTO para devolver el resultado de una apuesta
    public class BetResultDto
    {
        public string Name { get; set; } = string.Empty; 
        public decimal NewBalance { get; set; }              
        public int RouletteNumber { get; set; }            
        public string RouletteColor { get; set; } = string.Empty;
        public decimal AmountWon { get; set; }             
    }

}