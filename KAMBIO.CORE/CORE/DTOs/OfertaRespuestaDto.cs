// DTO de respuesta - así se ve una oferta cuando la consultas
namespace KAMBIO.CORE.Core.DTOs;

public class OfertaRespuestaDto
{
    public int IdOferta { get; set; }                        // ID de la oferta
    public string NombresAnunciante { get; set; } = null!;   // Nombre del anunciante (ej: "Juan")
    public string ApellidosAnunciante { get; set; } = null!; // Apellido (ej: "Pérez")
    public string DivisaOrigen { get; set; } = null!;        // "USD"
    public string DivisaDestino { get; set; } = null!;       // "PEN"
    public decimal MontoDisponible { get; set; }             // Total que tiene disponible
    public decimal MontoMinimo { get; set; }                 // Mínimo por transacción
    public decimal MontoMaximo { get; set; }                 // Máximo por transacción
    public decimal TasaCambio { get; set; }                  // Tipo de cambio ofrecido
    public string TipoOferta { get; set; } = null!;          // "Compra" o "Venta"
    public string Estado { get; set; } = null!;              // "Activa", "Cancelada", "Completada"
    public List<string> Bancos { get; set; } = new();        // Bancos que acepta: ["BCP", "Interbank"]
    public DateTime FechaPublicacion { get; set; }           // Cuándo se publicó
}
