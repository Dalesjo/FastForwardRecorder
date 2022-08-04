using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FastForwardRecorder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        private RecordWorker RecordWorker { get; set; }

        public RecordController(RecordWorker recordWorker)
        {
            RecordWorker = recordWorker;
        }


        [HttpGet("start")]
        public IActionResult Start()
        {
            var command = @"-stats_period 1 -y -f dshow -i audio=""@device_cm_{33D9A762-90C8-11D0-BD43-00A0C911CE86}\wave_{F991B35A-4E49-4AF8-87F0-092D41B79661}"" ""c:\tmp\9\file - name.mp3""";
            command = @"-stats_period 1 -y -f dshow -i audio=""@device_cm_{33D9A762-90C8-11D0-BD43-00A0C911CE86}\wave_{DE47F64F-DB1D-4EC7-94C9-F4A00D89DDE0}"" ""c:\tmp\9\name2.mp3""";

            RecordWorker.Start(command);

            return Ok(command);
        }

        [HttpGet("stop")]
        public async Task<IActionResult> Stop()
        {
            await RecordWorker.Stop();

            return Ok("Stopped");
        }
    }
}
