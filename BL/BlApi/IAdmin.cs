
namespace BlApi;

public interface IAdmin
{
    void InitializeDB();
    void ResetDB();
    TimeSpan GetMaxRange();
    void SetMaxRange(TimeSpan maxRange);
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
}
