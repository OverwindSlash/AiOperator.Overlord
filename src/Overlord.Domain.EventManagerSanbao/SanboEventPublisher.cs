using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Overlord.Domain.Event;
using Overlord.Domain.Interfaces;

namespace Overlord.Domain.EventManagerSanbao
{
    public class SanboEventPublisher : ITrafficEventPublisher
    {
        private readonly string _publishApiUri;

        public SanboEventPublisher(string publishApiUri)
        {
            _publishApiUri = publishApiUri;
        }
        
        public async Task<bool> ReportEvent(TrafficEvent trafficEvent)
        {
            SanbaoTrafficEvent sanbaoTrafficEvent = trafficEvent as SanbaoTrafficEvent;
            if (sanbaoTrafficEvent == null) return false;

            sanbaoTrafficEvent.EventImagePath = GenerateServerPath(sanbaoTrafficEvent.LocalImageFilePath);
            sanbaoTrafficEvent.EventVideoPath = GenerateServerPath(sanbaoTrafficEvent.LocalVideoFilePath);
            
            SanbaoTrafficMessage message = SanbaoTrafficMessage.GenerateTrafficMessage(
                sanbaoTrafficEvent.DeviceNo, sanbaoTrafficEvent.LaneIndex, sanbaoTrafficEvent);

            // Posting.  
            using (var client = new HttpClient())
            {
                // Setting Base address.  
                client.BaseAddress = new Uri(_publishApiUri);

                // Setting content type.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Initialization.  
                HttpResponseMessage response = new HttpResponseMessage();

                // HTTP POST  
                response = await client.PostAsJsonAsync(client.BaseAddress, message).ConfigureAwait(false);

                // Verification  
                if (response.IsSuccessStatusCode)
                {
                    // Reading Response.  
                    string result = response.Content.ReadAsStringAsync().Result;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private string GenerateServerPath(string localFilePath)
        {
            string urlPrefix = "http://10.115.1.11/";
            string url = localFilePath.Replace(@"D:\", urlPrefix);
            return url.Replace(@"\", "/");
        }
    }
}
