namespace KAMBIO.CORE.Core.DTOs;

public class AlertaRespuestaDto
{
    public int IdAlerta { get; set; }
    public string DivisaOrigen { get; set; } = null!;
    public string DivisaDestino { get; set; } = null!;
    public decimal ValorUmbral { get; set; }
    public bool Activa { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaDisparo { get; set; }
}
