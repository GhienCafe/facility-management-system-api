using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MainData.Hubs
{
    public class TimeHub : Hub
    {
        private Timer timer;

        public TimeHub()
        {
            timer = new Timer(state => SendTimeToClients(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        [HubMethodName("Dumamay")]
        public async Task SendTimeToClients()
        {
            try
            {
                if (Clients != null)
                {
                    await Clients.All.SendAsync("ReceiveTime", DateTime.Now.ToString());
                }
                else
                {
                    timer.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Ensure safe disposal of the timer when TimeHub is disposed
                timer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}