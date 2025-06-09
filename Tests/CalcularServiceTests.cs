using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SINQIA.Models;
using SINQIA.Services;
using SINQIA.Models.Calcular;

namespace SINQIA.Tests
{
    public class CalcularServiceTests
    {
        [Fact]
        public void Calcular_DeveRetornarValorAtualizadoCorreto_ComTaxaConstante()
        {
            // Arrange
            var cotacoes = new List<Cotacao>
        {
            new Cotacao { Data = new DateOnly(2025, 6, 3), Valor = 10 },
            new Cotacao { Data = new DateOnly(2025, 6, 4), Valor = 10 },
            new Cotacao { Data = new DateOnly(2025, 6, 5), Valor = 10 }
        };

            var cotacoesDbSet = GetQueryableMockDbSet(cotacoes);

            var mockContext = new Mock<SinqiaContext>();
            mockContext.Setup(c => c.Cotacaos).Returns(cotacoesDbSet.Object);

            var mockLogger = new Mock<ILogger<CalcularService>>();
            var service = new CalcularService(mockContext.Object, mockLogger.Object);

            var request = new CalcularRequest
            {
                DataInicio = new DateTime(2025, 6, 2),
                DataFim = new DateTime(2025, 6, 5),
                ValorAplicado = 1000
            };

            // Act
            var resultado = service.Calcular(request);

            // Assert
            Assert.Equal(4, resultado.DetalhamentoDiario.Count); // Inclui o dia inicial
            Assert.Equal(1000m, resultado.DetalhamentoDiario[0].ValorAtualizado); // Primeiro dia
            Assert.True(resultado.DetalhamentoDiario[3].ValorAtualizado > 1000m); // Último dia com juros
        }

        private static Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet;
        }
    }
}
