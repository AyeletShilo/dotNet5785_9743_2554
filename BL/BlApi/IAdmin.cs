
namespace BlApi;

public interface IAdmin
{
    void InitializeDB();
    void ResetDB();
    TimeSpan GetMaxRange();
    void SetMaxRange(TimeSpan maxRange);
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);

    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5

}
