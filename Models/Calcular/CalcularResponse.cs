namespace SINQIA.Models.Calcular
{
    public class CalcularResponse
    {
        public decimal ValorAplicado { get; set; }
        public decimal TaxaAnualPercentual { get; set; }
        public List<CalculoDiarioModel> DetalhamentoDiario { get; set; }
    }
}
