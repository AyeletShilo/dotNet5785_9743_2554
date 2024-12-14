namespace Dal;
using DalApi;
//sealed internal class DalList : IDal
//{
//    public static IDal Instance { get; } = new DalList();
//    private DalList() { }
//    public IAssignment Assignment { get; } = new AssignmentImplementation();
//    public ICall Call { get; } = new CallImplementation();
//    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
//    public IConfig Config { get; } = new ConfigImplementation();

//    public void ResetDB()
//    {
//        Assignment.DeleteAll();
//        Call.DeleteAll();
//        Volunteer.DeleteAll();
//        Config.Reset();
//    }
//}


sealed internal class DalList : IDal
{
    private static IDal instance = null;
    private static readonly object lockObject = new object();
    
    public static IDal Instance //Static constructor for the father
    {
        get
        {
            if (instance == null) //one check for singleton
                lock (lockObject)
                {
                    if (instance == null) //second check for thread safe
                        instance = new DalList(); //Creating an instance of DalList.
                }
            return instance;
        }
    }
    private DalList() { }
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Assignment.DeleteAll();
        Call.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}