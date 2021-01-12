using System.Threading.Tasks;
using dotenv.net.Interfaces;

namespace VideoDelayWPF.MainApp.Services
{
    public class CameraFeedService
    {
        public CameraFeedService(IEnvReader settings)
        {
            
        }

        public string MinHueColor { get; set; }

        /// <summary>
        /// Client screen should be blinking. Find the area that the client is blinking at and return when found.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> FindClientLocation(int timeoutSeconds)
        {
            return false;
        }
    }
}