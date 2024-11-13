
using DalApi;

namespace Dal;

public class ConfigImplementation: IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskRang  { get; set; }
    public void Reset()
    {
        Config.Reset();
    }
}
