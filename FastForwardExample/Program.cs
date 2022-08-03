// See https://aka.ms/new-console-template for more information
using FastForwardLibrary;

Console.WriteLine("Hello, World!");


void logger(object? sender, string line)
{
    if(sender is FastForward fastForward)
    {
        Console.WriteLine($"log ={line}");
    }
}

void stats(object? sender, EventArgs e)
{
    if (sender is FastForwardState state)
    {
        Console.WriteLine($"time={state.Time}");
    }
}

// Initiate class, and point out where ffmpeg executable is located
var ffmpeg = new FastForward(@"c:\Program Files\ffmpeg\ffmpeg-5.1\bin\ffmpeg.exe");

// Register eventhandlers
ffmpeg.Log += logger;
ffmpeg.State.Update += stats;


// CancelationToken can be used to terminate exeuction 
var cancellationTokenSource = new CancellationTokenSource();
cancellationTokenSource.CancelAfter(10000);

try
{
    await ffmpeg.Execute(@"-stats_period 1 -y -f dshow -i audio=""@device_cm_{33D9A762-90C8-11D0-BD43-00A0C911CE86}\wave_{F991B35A-4E49-4AF8-87F0-092D41B79661}"" ""c:\tmp\file - name.mp3""", cancellationTokenSource.Token);
}
catch (Exception)
{
    Console.WriteLine("Execution failed");
}
finally
{
    Console.WriteLine($"exitcode {ffmpeg.ExitCode}");
}

