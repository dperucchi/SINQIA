using System;
using System.Collections.Generic;

namespace SINQIA.Models;

public partial class Cotacao
{
    public int Id { get; set; }

    public DateOnly Data { get; set; }

    public string Indexador { get; set; } = null!;

    public decimal Valor { get; set; }
}
