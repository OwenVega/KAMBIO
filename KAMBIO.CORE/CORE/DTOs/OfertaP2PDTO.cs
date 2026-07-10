namespace KAMBIO.CORE.Core.DTOs
{
    public class OfertaP2PDTO
    {
        public int IdOferta { get; set; }
        public int IdAnunciante { get; set; }
        public string AnuncianteNombre { get; set; }
        public decimal PorcentajeReputacion { get; set; }
        public int OrdenesCompletadas { get; set; }
        public decimal TasaCambio { get; set; }
        public decimal MontoDisponible { get; set; }
        public decimal LimiteMinimo { get; set; }
        public decimal LimiteMaximo { get; set; }
        public List<string> MetodosPago { get; set; }
    }
}