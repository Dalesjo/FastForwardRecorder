using FastForwardRecorder;
using FastForwardRecorder.Hubs;
using Microsoft.Extensions.Logging.EventLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR();
builder.Services.AddControllers();
//builder.Services.AddHostedService<RecordWorker>();

builder.Services.AddSingleton<RecordWorker>();
builder.Services.AddHostedService<RecordWorker>(provider => provider.GetService<RecordWorker>()!);



// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
//builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
builder.Host.UseNLog();

// Enable windows service
if (OperatingSystem.IsWindows())
{
    builder.Services.Configure<EventLogSettings>(config =>
    {
        config.LogName = "TallyServer";
        config.SourceName = "TallyServer";
    });
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<RecordHub>("/hubs/record");

app.Run();
