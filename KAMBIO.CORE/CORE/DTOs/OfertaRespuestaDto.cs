namespace KAMBIO.CORE.Core.DTOs;

public class OfertaRespuestaDto
{
    public int IdOferta { get; set; }
    public string NombresAnunciante { get; set; } = null!;
    public string ApellidosAnunciante { get; set; } = null!;
    public string DivisaOrigen { get; set; } = null!;
    public string DivisaDestino { get; set; } = null!;
    public decimal MontoDisponible { get; set; }
    public decimal MontoMinimo { get; set; }
    public decimal MontoMaximo { get; set; }
    public decimal TasaCambio { get; set; }
    public string TipoOferta { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public List<string> Bancos { get; set; } = new();
    public DateTime FechaPublicacion { get; set; }
}
