using Microsoft.Extensions.Configuration;
using Shared.TelemetryServer;
using System.Collections.Concurrent;
using System.Reflection;
using TelemetrySimulator;

ConcurrentDictionary<int, AirplaneSimulator> activeAirplanes = new();
CancellationTokenSource cts = new();
string? hubUrl = null;
string? apiBaseUrl = null;

try
{
    Console.WriteLine("Reading configurations...");

    var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .Build();

    hubUrl = configuration["SignalROptions:HubUrl"]!;
    apiBaseUrl = configuration["BackendOptions:BaseUrl"];
}
catch (Exception ex)
{
    Console.Error.WriteLine("Error reading configurations:\n" + ex.Message);

    return;
}


try
{
    foreach (var airplaneModel in Airplane.Airplanes)
    {
        Console.WriteLine($"Preparing Airplane {airplaneModel.Id}...");

        try
        {
            var simulatedAirplane = new AirplaneSimulator(hubUrl!, apiBaseUrl!, airplaneModel.Id);

            await simulatedAirplane.InitializeSignalRConnection();

            activeAirplanes.TryAdd(airplaneModel.Id, simulatedAirplane);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ERROR preparing Airplane {airplaneModel.Id}:\n{ex.Message}");
        }
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine("Error preparing airplanes:\n" + ex.Message);
}

// Keep the application running
Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("Application is shutting down...");

    foreach (var airplane in activeAirplanes.Values)
    {
        Console.WriteLine($"Disposing connection for Airplane {airplane.GetAirplaneId()}...");

        airplane.Dispose();
    }

    activeAirplanes.Clear();

    // Signal the cancellation token to stop the Task.Delay
    cts.Cancel();
};

// Keep the application running
while (!cts.IsCancellationRequested)
{
    await Task.Delay(1000);
}

Console.WriteLine("Application has exited gracefully.");
