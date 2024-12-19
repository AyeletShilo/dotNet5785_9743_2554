using BlApi;
using BO;
using Helpers;
using System.Data;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Update the clock in one unit
    /// </summary>
    /// <param name="unit">unit to advance </param>
    public void ForwardClock(TimeUnit unit) =>
    ClockManager.UpdateClock(unit switch
    {
        TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
        TimeUnit.Hour => ClockManager.Now.AddHours(1),
        TimeUnit.Day => ClockManager.Now.AddDays(1),
        TimeUnit.Month => ClockManager.Now.AddMonths(1),
        TimeUnit.Year => ClockManager.Now.AddYears(1),
        _=>ClockManager.Now
    });

    /// <summary>
    /// Return System clock
    /// </summary>
    /// <returns> System clock</returns>
    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    /// <summary>
    /// Return system risk range
    /// </summary>
    /// <returns>system risk range</returns>
    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange;
    }

    /// <summary>
    /// Preform initiaizetion to data base
    /// </summary>
    public void InitializeDB()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    /// <summary>
    /// Reset data base
    /// </summary>
    public void ResetDB()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    /// <summary>
    /// Update system risk range
    /// </summary>
    /// <param name="maxRange">new risk range</param>
    public void SetMaxRange(TimeSpan maxRange)
    {
        _dal.Config.RiskRange= maxRange;
    }
}
