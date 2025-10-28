### **Logging Configuration**
#### **appsettings.json**
the json file will need code something similar to below
 AppSettng.json code similar to below:
 "Logging": {
   "LogLevel": {
     "Default": "Information",
     "Microsoft.AspNetCore": "Warning"
   }
 },
 "Serilog": {
   "Using": [
     "Serilog.Sinks.Console",
     "Serilog.Sinks.File"
   ],
   "MinimumLevel": "Information",
   "WriteTo": [
     {
       "Name": "Console"
     },
     {
       "Name": "File",
       "Args": {
         "path": "logs\\MuseumApi.txt",
         "rollingInterval": "Day",
         "shared": true
         /*"outputTemplate": "{Timestamp:o} [{Level:u3}] ({Application}/{MachineName}/{ThreadId}) {Message}{NewLine}{Exception}",*/
         ,
         "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
       }
     }
   ],
   "Enrich": [
     "FromLogContext",
     "WithMachineName",
     "WithThreadId",
     "WithExceptionDetails"
   ],
   "Properties": {
     "Application": "MuseumApi"
   }
 }

#### **Logging Initialization**
in program.cs 
ConfigureLogging(builder);
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

and 
static void ConfigureLogging(WebApplicationBuilder builder)
{
    LoggerProviderCollection Providers = new();
    //how do I use ConfigurationReaderOptions with Serilog?
    
    var options = new ConfigurationReaderOptions() { SectionName = "Serilog" };

    
    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration, options)
                             .Enrich.FromLogContext()
                            .WriteTo.Providers(Providers)
                             .WriteTo.Console(new RenderedCompactJsonFormatter())
                             .CreateLogger();
    builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration, options)
                            .Enrich.FromLogContext()
                            .WriteTo.Providers(Providers)
                            .WriteTo.Console(new RenderedCompactJsonFormatter()
     ));
}
