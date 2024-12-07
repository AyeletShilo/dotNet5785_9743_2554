
namespace BlApi;

public interface IAdmin
{
    void InitializeDB();
    void ResetDB();
    int GetMaxRange();
    void SetMaxRange(int maxRange);
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
}
