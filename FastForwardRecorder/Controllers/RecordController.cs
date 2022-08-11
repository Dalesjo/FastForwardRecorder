﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FastForwardRecorder.Controllers
{
    [Route("api/record")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        public RecordController(RecordWorker recordWorker)
        {
            RecordWorker = recordWorker;
        }

        private RecordWorker RecordWorker { get; set; }
        [HttpGet("start")]
        public IActionResult Start()
        {
            var command = @"-stats_period 1 -y -f dshow -i audio=""@device_cm_{33D9A762-90C8-11D0-BD43-00A0C911CE86}\wave_{F991B35A-4E49-4AF8-87F0-092D41B79661}"" ""c:\tmp\9\file - name.mp3""";
            command = @"-stats_period 1 -y -f dshow -i audio=""@device_cm_{33D9A762-90C8-11D0-BD43-00A0C911CE86}\wave_{DE47F64F-DB1D-4EC7-94C9-F4A00D89DDE0}"" ""ljudfile_{date}.mp3""";

            command = @"-stats_period 1 -y -f dshow -i audio=""@device_cm_{33D9A762-90C8-11D0-BD43-00A0C911CE86}\wave_{DE47F64F-DB1D-4EC7-94C9-F4A00D89DDE0}"" -acodec flac ""ljudfile_{date}.ogg""";
            

            command = replaceCommand(command);
            RecordWorker.Start(command);

            return Ok(command);
        }

        private string replaceCommand(string command)
        {

            var date = DateTime.Now.ToString("yyyymmdd_HHmmss_fff");
            command = command.Replace("{date}", date);

            return command;
        }

        [HttpGet("stop")]
        public async Task<IActionResult> Stop()
        {
            await RecordWorker.Stop();

            return Ok("Stopped");
        }
    }
}
