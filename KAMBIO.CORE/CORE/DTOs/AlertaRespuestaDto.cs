// DTO de respuesta - esto es lo que el API te devuelve cuando
// consultas tus alertas de tipo de cambio.
namespace KAMBIO.CORE.Core.DTOs;

public class AlertaRespuestaDto
{
    public int IdAlerta { get; set; }         // Nro. de la alerta en la BD
    public string DivisaOrigen { get; set; } = null!;   // Ej: "USD"
    public string DivisaDestino { get; set; } = null!;  // Ej: "PEN"
    public decimal ValorUmbral { get; set; }  // El valor que configuraste
    public bool Activa { get; set; }          // True = activa, False = desactivada
    public DateTime FechaCreacion { get; set; }  // Cuándo la creaste
    public DateTime? FechaDisparo { get; set; }  // Cuándo se "disparó" (la alerta se cumplió)
}
