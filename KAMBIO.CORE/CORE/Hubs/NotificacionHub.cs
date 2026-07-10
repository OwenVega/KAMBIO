using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;

namespace KAMBIO.CORE.CORE.Hubs
{
    public class NotificacionHub : Hub
    {
        public async Task UnirseAGrupo(string usuarioId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"usuario_{usuarioId}");
        }

        public async Task SalirDeGrupo(string usuarioId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"usuario_{usuarioId}");
        }
    }
}
