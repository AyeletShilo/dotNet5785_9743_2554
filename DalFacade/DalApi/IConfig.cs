
namespace DalApi;

public interface IConfig
{


    DateTime Clock { get; set; }
    TimeSpan RiskRang { get; set; }
    void Reset();
}
