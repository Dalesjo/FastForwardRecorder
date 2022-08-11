using FastForwardRecorder;
using FastForwardRecorder.Hubs;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.EventLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});
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
var MyAllowSpecificOrigins = "DalesjoCors";
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        
                      policy =>
                      {
                          policy.WithOrigins("https://samuel.dalesjo.nu");
                          policy.AllowCredentials();
                          policy.WithHeaders("x-requested-with", "x-signalr-user-agent");
                      });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"c:\tmp\ljud\"),
    RequestPath = "/audio"
});


app.MapHub<RecordHub>("/hubs/record");

app.Run();
