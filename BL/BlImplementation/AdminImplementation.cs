using BlApi;
using BO;
using Helpers;
using System.Data;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void ForwardClock(TimeUnit unit) =>
    ClockManager.UpdateClock(unit switch
    {
        TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
        TimeUnit.hour => ClockManager.Now.AddHours(1),
        TimeUnit.day => ClockManager.Now.AddDays(1),
        TimeUnit.month => ClockManager.Now.AddMonths(1),
        TimeUnit.year => ClockManager.Now.AddYears(1),
    });

    //public void ForwardClock(TimeUnit unit)
    //{
    //    unit switch
    //    {
    //        TimeUnit.Minute => ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1)),
    //        TimeUnit.hour => ClockManager.UpdateClock(ClockManager.Now.AddHours(1)),
    //        TimeUnit.day => ClockManager.UpdateClock(ClockManager.Now.AddDays(1)),
    //        TimeUnit.month => ClockManager.UpdateClock(ClockManager.Now.AddMonths(1)),
    //        TimeUnit.year => ClockManager.UpdateClock(ClockManager.Now.AddYears(1)),
    //        _ => throw new NotInCurrentFormat(nameof(unit), $"Unsupported TimeUnit: {unit}")
    //    };
       
    //}

    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange;
    }

    public void InitializeDB()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void ResetDB()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void SetMaxRange(TimeSpan maxRange)
    {
        _dal.Config.RiskRange= maxRange;
    }
}
