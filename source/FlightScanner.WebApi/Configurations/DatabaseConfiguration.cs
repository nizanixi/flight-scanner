
using Microsoft.EntityFrameworkCore;

namespace FlightScanner.WebApi.Configurations;

public class DatabaseConfiguration
{
    private readonly IConfigurationSection _configurationSection;

    public DatabaseConfiguration(IConfigurationSection configurationSection)
    {
        _configurationSection = configurationSection;
    }

    public string DatabaseFolder => _configurationSection.GetValue<string>("DatabaseFolder")!;

    public string DatabaseName => _configurationSection.GetValue<string>("DatabaseName")!;

    public int CommandTimeoutInSeconds => _configurationSection.GetValue<int>("CommandTimeoutInSeconds");

    public QueryTrackingBehavior QueryTrackingBehavior => _configurationSection.GetValue<QueryTrackingBehavior>("QueryTrackingBehavior");

    public bool EnableDetailedErrors => _configurationSection.GetValue<bool>("EnableDetailedErrors");

    public bool EnableSensitiveDataLogging => _configurationSection.GetValue<bool>("EnableSensitiveDataLogging");
}
