using SINQIA.Models;
using SINQIA.Models.Calcular;

namespace SINQIA.Services
{
    public class CalcularService
    {
        private readonly SinqiaContext _context;
        private readonly ILogger<CalcularService> _logger;

        public CalcularService(SinqiaContext context, ILogger<CalcularService> logger)
        {
            _context = context; 
            _logger = logger;
        }

        public CalcularResponse Calcular(CalcularRequest request)
        {
            _logger.LogInformation("Iniciando cálculo de atualização de {Inicio} até {Fim}, valor {Valor}", request.DataInicio, request.DataFim, request.ValorAplicado);
            var datas = Enumerable.Range(0, (request.DataFim - request.DataInicio).Days + 1)
            .Select(offset => request.DataInicio.AddDays(offset))
            .Where(DiaUtil)
            .ToList();

            var datasDateOnly = datas.Select(d => DateOnly.FromDateTime(d)).ToList();

            var cotacoes = _context.Cotacaos
                .Where(c => datasDateOnly.Contains(c.Data)) 
                .ToDictionary(c => c.Data.ToDateTime(TimeOnly.MinValue).Date, c => c.Valor);
            decimal fatorAcumulado = 1m;
            decimal valorAplicado = request.ValorAplicado;

            var detalhes = new List<CalculoDiarioModel>();

            DateTime? dataAnterior = null;
            decimal? fatorDiarioAnterior = null;

            foreach (var data in datas)
            {
                decimal taxaAnual = cotacoes.ContainsKey(data) ? cotacoes[data] : throw new Exception($"Cotação não encontrada para {data:yyyy-MM-dd}");

                // Calcular fator diário
                var fatorDiario = Math.Pow(1 + (double)taxaAnual / 100, 1.0 / 252);
                fatorDiario = Math.Round(fatorDiario, 8, MidpointRounding.AwayFromZero);
                var fatorDiarioDecimal = (decimal)fatorDiario;

                // Se não é o primeiro dia, aplicar o fator do dia anterior
                if (dataAnterior != null && fatorDiarioAnterior.HasValue)
                {
                    fatorAcumulado *= fatorDiarioAnterior.Value;
                    fatorAcumulado = TruncarDecimal(fatorAcumulado, 16);
                }

                // Calcular valor atualizado
                var valorAtualizado = TruncarDecimal(valorAplicado * fatorAcumulado, 8);

                detalhes.Add(new CalculoDiarioModel
                {
                    Data = data,
                    FatorDiario = fatorDiarioDecimal,
                    FatorAcumulado = fatorAcumulado,
                    ValorAtualizado = valorAtualizado
                });

                dataAnterior = data;
                fatorDiarioAnterior = fatorDiarioDecimal;

                _logger.LogDebug("Dia {Data}: Taxa={TaxaAnual}, FatorDiario={FatorDiario}, Acumulado={FatorAcumulado}, Atualizado={ValorAtualizado}", data, taxaAnual, fatorDiarioDecimal, fatorAcumulado, valorAtualizado);
            }

            return new CalcularResponse
            {
                ValorAplicado = valorAplicado,
                DetalhamentoDiario = detalhes
            };
        }

        private static decimal TruncarDecimal(decimal valor, int casas)
        {
            decimal fator = (decimal)Math.Pow(10, casas);
            return Math.Truncate(valor * fator) / fator;
        }

        private static bool DiaUtil(DateTime data)
        {
            return data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday;
        }

        private static DateTime ObterProximoDiaUtil(DateTime data)
        {
            var proximo = data.AddDays(1);
            while (!DiaUtil(proximo))
            {
                proximo = proximo.AddDays(1);
            }
            return proximo;
        }
    }
}
