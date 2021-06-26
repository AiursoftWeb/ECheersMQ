using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Aiursoft.XelNaga.Tools;

namespace Echeers.Mq.Services
{
    public class WebSocketPusher
    {
        private WebSocket _ws;
        public bool Connected => _ws.State == WebSocketState.Open;

        public async Task Accept(HttpContext context)
        {
            _ws = await context.WebSockets.AcceptWebSocketAsync();
        }

        public async Task SendMessage(string Message)
        {
            await _ws.SendMessage(Message);
        }
    }
}
