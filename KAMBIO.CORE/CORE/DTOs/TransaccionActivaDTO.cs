namespace KAMBIO.CORE.Core.DTOs
{
    public class TransaccionActivaDTO
    {
        public int IdTransaccion { get; set; }
        public string OtraParteNombre { get; set; } = null!;
        public int IdOtraParte { get; set; }
        public string EstadoNombre { get; set; } = null!;
        public string? UltimoMensaje { get; set; }
        public DateTime? FechaUltimoMensaje { get; set; }
        public int MensajesNoLeidos { get; set; }
    }
}
