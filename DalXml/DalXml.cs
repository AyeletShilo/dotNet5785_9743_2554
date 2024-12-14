using DalApi;
namespace Dal;

//sealed internal class DalXml : IDal
//{
//    public static IDal Instance { get; } = new DalXml();
//    private DalXml() { }

//    public IAssignment Assignment { get; } = new AssignmentImplementation();
//    public ICall Call { get; } = new CallImplementation();
//    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
//    public IConfig Config {  get; } = new ConfigImplementation();

//    public void ResetDB()
//    {
//        Assignment.DeleteAll();
//        Call.DeleteAll();
//        Volunteer.DeleteAll();
//        Config.Reset();
//    }
//}



sealed internal class DalXml : IDal
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
                        instance = new DalXml(); //Creating an instance of DalList.
                }
            return instance;
        }
    }
    private DalXml() { }
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


