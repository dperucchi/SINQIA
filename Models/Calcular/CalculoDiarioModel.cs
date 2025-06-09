namespace SINQIA.Models.Calcular
{
    public class CalculoDiarioModel
    {
        public DateTime Data { get; set; }
        public decimal FatorDiario { get; set; }
        public decimal FatorAcumulado { get; set; }
        public decimal ValorAtualizado { get; set; }
    }
}
