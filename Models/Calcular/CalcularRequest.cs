namespace SINQIA.Models.Calcular
{
    public class CalcularRequest
    {
        public decimal ValorAplicado { get; set; }
        public decimal TaxaAnualPercentual { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
