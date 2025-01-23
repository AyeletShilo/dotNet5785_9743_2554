using BlApi;
using BO;
using DalApi;
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
    public void ForwardClock(TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.UpdateClock(unit switch
        {
            TimeUnit.Minute => AdminManager.Now.AddMinutes(1),
            TimeUnit.Hour => AdminManager.Now.AddHours(1),
            TimeUnit.Day => AdminManager.Now.AddDays(1),
            TimeUnit.Month => AdminManager.Now.AddMonths(1),
            TimeUnit.Year => AdminManager.Now.AddYears(1),
            _ => AdminManager.Now
        });
    }

    /// <summary>
    /// Return System clock
    /// </summary>
    /// <returns> System clock</returns>
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    /// <summary>
    /// Return system risk range
    /// </summary>
    /// <returns>system risk range</returns>
    public TimeSpan GetMaxRange() => AdminManager.MaxRange;

    /// <summary>
    /// Preform initialization to data base
    /// </summary>
    public void InitializeDB()
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            AdminManager.InitializeDB(); //stage 7
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    /// <summary>
    /// Reset data base
    /// </summary>
    public void ResetDB()
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            AdminManager.ResetDB(); //stage 7

        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    /// <summary>
    /// Update system risk range
    /// </summary>
    /// <param name="maxRange">new risk range</param>
    public void SetMaxRange(TimeSpan riskRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        _dal.Config.RiskRange = riskRange;
    }

    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
    public void StopSimulator()
        => AdminManager.Stop(); //stage 7


    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
            AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
            AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
            AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
            AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5

}
