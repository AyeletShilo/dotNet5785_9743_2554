namespace DalTest;

using Dal;
using DalApi;
using DO;
using System.Net.Mail;

enum choiceMain { exit, assignment, call, volunteer, init, print, congfi, reset };
enum choiceA { exit, create, read, readAll, update, delete, deleteAll };
enum choiceB { exit, minute, hour, day, month, year, clock, update, read, delete };
enum choiceC { clock, risk };



internal class Program
{
    //private static ICall? s_dalCall = new CallImplementation();//stage 1
    //private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();//stage 1
    //private static IAssignment? s_dalAssignment = new AssignmentImplementation();//stage 
    //private static IConfig? s_dalConfig = new ConfigImplementation();//stage 1

    //static readonly IDal s_dal = new DalList(); //stage 2
    static readonly IDal s_dal = new DalXml(); //stage 3

    static void Main(string[] args)
    {
        try
        {
            Program p = new();
            p.menu();
        }
        catch (Exception e) { Console.WriteLine(e); }
    }

    private void menu()
    {

        bool stop = false;
        while (stop is not true)
        {
            Console.WriteLine(@"
Press 1 to display submenu for Assignment,
Press 2 to display submenu for Call,
Press 3 to display submenu for Volunteer,
Press 4 to perform an initialization,
Press 5 to show all data objects,
Press 6 to display submenu for configuration,
Press 7 to reset the database and the configuration,
Press 0 to exit"
        );

            string input = Console.ReadLine();
            try
            {
                if (Enum.TryParse(typeof(choiceMain), input, true, out object? result) && result is choiceMain c)
                    switch (c)
                    {
                        case choiceMain.assignment:
                            subMenu("Assignment");
                            break;
                        case choiceMain.call:
                            subMenu("Call");
                            break;
                        case choiceMain.volunteer:
                            subMenu("Volunteer");
                            break;
                        case choiceMain.init:
                            //Initialization.Do(s_dalAssignment, s_dalCall, s_dalVolunteer, s_dalConfig); //stage 1
                            Initialization.Do(s_dal);//stage 2
                            break;
                        case choiceMain.print:
                            printA();
                            Console.WriteLine("\n");
                            printC();
                            Console.WriteLine("\n");
                            printV();
                            Console.WriteLine("\n");
                            break;
                        case choiceMain.congfi:
                            subConfig();
                            break;
                        case choiceMain.reset:
                            //s_dalAssignment?.DeleteAll(); //stage 1
                            //s_dalCall?.DeleteAll(); //stage 1
                            //s_dalVolunteer?.DeleteAll(); //stage 1
                            //s_dalConfig?.Reset(); //stage 1
                            s_dal.ResetDB();//stage 2
                            break;
                        default:
                            stop = true;
                            break;
                    }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }
    }

    private void subMenu(string type)
    {
        bool stopA = false;
        while (stopA != true)
        {
            Console.WriteLine(@"
Press 1 to create new object,
Press 2 to print an object,
Press 3 to print all the objects,
Press 4 to update an existing object,
Press 5 to delete an existing object
Press 6 to delete all the objects,
Press 0 to exit"
        );
            string inputB = Console.ReadLine();

            if (Enum.TryParse(typeof(choiceA), inputB, true, out object? result) && result is choiceA c)
                switch (c)
                {

                    case choiceA.create:
                        createA(type);
                        break;
                    case choiceA.read:
                        Console.WriteLine("Write ID to read:");
                        readA(type);
                        break;
                    case choiceA.readAll:
                        if (type == "Assignment")
                            printA();
                        if (type == "Call")
                            printC();
                        if (type == "Volunteer")
                            printV();
                        break;
                    case choiceA.update:
                        updateA(type);
                        break;
                    case choiceA.delete:
                        Console.WriteLine("Write ID to delete:");
                        int num = int.Parse(Console.ReadLine());
                        try
                        {
                            if (type == "Assignment")
                            {
                                //s_dalAssignment?.Delete(num); //stage 1
                                s_dal?.Assignment.Delete(num); //stage 2
                            }
                            if (type == "Call")
                            {
                                //s_dalCall?.Delete(num); //stage 1
                                s_dal?.Call.Delete(num); //stage 2
                            }
                            if (type == "Volunteer")
                            {
                                //s_dalVolunteer?.Delete(num); //stage 1
                                s_dal?.Volunteer.Delete(num); //stage 2
                            }
                        }
                        catch (Exception e) { Console.WriteLine(e); }
                        break;
                    case choiceA.deleteAll:
                        if (type == "Assignment")
                        {
                            //s_dalAssignment?.DeleteAll(); //stage 1
                            s_dal?.Assignment.DeleteAll(); //stage 2
                        }
                        if (type == "Call")
                        {
                            //s_dalCall?.DeleteAll(); //stage 1
                            s_dal?.Call.DeleteAll(); //stage 2
                        }
                        if (type == "Volunteer")
                        {
                            //s_dalVolunteer?.DeleteAll(); //stage 1
                            s_dal?.Volunteer.DeleteAll(); //stage 2
                        }
                        break;
                    default:
                        stopA = true;
                        break;
                }

        }
    }
    private void subConfig()
    {
        bool stopB = false;
        while (stopB != true)
        {
            Console.WriteLine(@"Press 1 to add minute to system clock,
Press 2 to add hour to system clock,
Press 3 to add day to system clock,
Press 4 to add month to system clock,
Press 5 to add year to system clock
Press 6 to show current system clock value,
Press 7 to Set a new value to any configuration variable,
Press 8 to show a current value for any configuration variable,
Press 9 to reset values ​​for all configuration variables,
Press 0 to exit
");

            string inputC = Console.ReadLine();

            if (Enum.TryParse(typeof(choiceB), inputC, true, out object? result) && result is choiceB b)
            {
                switch (b)
                {
                    case choiceB.minute:
                        //s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1); //stage 1
                        s_dal!.Config.Clock = s_dal!.Config.Clock.AddMinutes(1); //stage 2
                        break;
                    case choiceB.hour:
                        //s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1); //stage 1
                        s_dal!.Config.Clock = s_dal!.Config.Clock.AddHours(1); //stage 2
                        break;
                    case choiceB.day:
                        //s_dalConfig.Clock = s_dalConfig.Clock.AddDays(1);  //stage 1
                        s_dal!.Config.Clock = s_dal!.Config.Clock.AddDays(1); //stage 2
                        break;
                    case choiceB.month:
                        //s_dalConfig.Clock = s_dalConfig.Clock.AddMonths(1); //stage1
                        s_dal!.Config.Clock = s_dal!.Config.Clock.AddMonths(1); //stage 2
                        break;
                    case choiceB.year:
                        //s_dalConfig.Clock = s_dalConfig.Clock.AddYears(1); //stage 1
                        s_dal!.Config.Clock = s_dal!.Config.Clock.AddYears(1); //stage 2
                        break;
                    case choiceB.clock:
                        //Console.WriteLine(s_dalConfig?.Clock); //stage 1
                        Console.WriteLine(s_dal!.Config?.Clock); //stage 2
                        break;
                    case choiceB.update:
                        Console.WriteLine(
                        "Press 0 for clock, " +
                        "Press 1 for risk range");
                        string inputD = Console.ReadLine();
                        if (Enum.TryParse(typeof(choiceC), inputD, true, out object? result2) && result2 is choiceC d)
                        {
                            Console.WriteLine("Write update value:");
                            switch (d)
                            {
                                case choiceC.clock:
                                    //s_dalConfig.Clock = DateTime.Parse(Console.ReadLine()); //stage 1
                                    s_dal!.Config.Clock = DateTime.Parse(Console.ReadLine()); //stage 2
                                    break;
                                case choiceC.risk:
                                    //s_dalConfig.RiskRange = TimeSpan.Parse(Console.ReadLine()); //stage 1
                                    s_dal!.Config.RiskRange = TimeSpan.Parse(Console.ReadLine()); //stage 2
                                    break;
                            }
                        }
                        break;
                    case choiceB.read:
                        Console.WriteLine("Press 0 for clock," +
                        "Press 1 for risk range");
                        string inputE = Console.ReadLine();
                        if (Enum.TryParse(typeof(choiceC), inputE, true, out object? result3) && result3 is choiceC e)
                            switch (e)
                            {
                                case choiceC.clock:
                                    //Console.WriteLine(s_dalConfig.Clock); //stage 1
                                    Console.WriteLine(s_dal.Config.Clock); //stage 2
                                    break;
                                case choiceC.risk:
                                    //Console.WriteLine(s_dalConfig.RiskRange); //stage 1
                                    Console.WriteLine(s_dal.Config.RiskRange); //stage 2
                                    break;
                            }
                        break;
                    case choiceB.delete:
                        //s_dalConfig?.Reset(); //stage 1
                        s_dal!.Config?.Reset(); //stage 2
                        break;
                    default:
                        stopB = true;
                        break;
                }
            }
        }
    }
    private void createA(string type)
    {
        if (type == "Assignment")
        {
            Console.WriteLine("Write new values:\n");

            Console.WriteLine("new callId:");
            int callId = int.Parse(Console.ReadLine());

            Console.WriteLine("new volunteerId:");
            int volunteerId = int.Parse(Console.ReadLine());

            DateTime openTime = DateTime.Now;
            //DateTime closeTime = DateTime.Now + s_dalConfig.RiskRang; //stage 1
            DateTime closeTime = DateTime.Now + s_dal.Config.RiskRange; //stage 2

            Console.WriteLine("press 0 for TakenCare" +
                    "press 1 for SelfCancel," +
                    "press 2 for CancelAdmin," +
                    "press 3 for CancelExpired");

            string input = Console.ReadLine();
            DO.AssignmentEnum? finishType = Enum.Parse<DO.AssignmentEnum>(input);

            //s_dalAssignment.Create(new(0, callId, volunteerId, openTime, closeTime, finishType)); //stage 1
            s_dal!.Assignment.Create(new(0, callId, volunteerId, openTime, closeTime, finishType)); //stage 2

        }
        if (type == "Call")
        {
            Console.WriteLine(
                "press 0 for shopping," +
                "press 1 for cleaning," +
                "press 2 for repairing," +
                "press 3 for technologyHelp," +
                "press 4 for talking");

            string input = Console.ReadLine();
            DO.TypeOfCall cType = Enum.Parse<DO.TypeOfCall>(input);


            Console.WriteLine("new address:");
            string address = Console.ReadLine()!;

            Console.WriteLine("latitude and longitude:");
            double latitude = double.Parse(Console.ReadLine());
            double longitude = double.Parse(Console.ReadLine());

            //DateTime? tempOpen = s_dalConfig?.Clock; //stage 1
            DateTime? tempOpen = s_dal!.Config?.Clock; //stage 2
            DateTime openTime = (DateTime)tempOpen!;

            //DateTime maxTime = openTime + s_dalConfig.RiskRange; //stage 1
            DateTime maxTime = openTime + s_dal!.Config.RiskRange; //stage 2

            Console.WriteLine("new description:");
            string description = Console.ReadLine();

            //s_dalCall?.Create(new(0, cType, address, latitude, longitude, openTime, description, maxTime)); //stage 1
            s_dal!.Call?.Create(new(0, cType, address, latitude, longitude, openTime, description, maxTime)); //stage 2
        }
        if (type == "Volunteer")
        {
            Console.WriteLine("new Id:");
            int id = int.Parse(Console.ReadLine());

            Console.WriteLine("new name:");
            string fullName = Console.ReadLine()!;

            Console.WriteLine("new phone number:");
            string phoneNumber = Console.ReadLine()!;

            Console.WriteLine("new email:");
            string email = Console.ReadLine()!;

            Console.WriteLine("press 0 for manager, " +
                "press 1 for volunteer:");

            string input = Console.ReadLine();
            DO.Role job = Enum.Parse<DO.Role>(input);

            Console.WriteLine("active?");
            string tempActive = Console.ReadLine()!;
            bool active = false;
            if (tempActive == "true")
                active = true;

            Console.WriteLine("press 0 for air, " +
                               "press 1 for walking, " +
                               "press 2 for car");

            input = Console.ReadLine();
            DO.RangeType distance = Enum.Parse<DO.RangeType>(input);

            Console.WriteLine("new address:");
            string? add = Console.ReadLine();

            Console.WriteLine("new maximom distance:");
            double dis = double.Parse(Console.ReadLine());

            //s_dalVolunteer?.Create(new(id, fullName, phoneNumber, email, job, active, distance, add, 0, 0, dis)); //stage 1
            s_dal.Volunteer?.Create(new(id, fullName, phoneNumber, email, job, active, distance, add, 0, 0, dis)); //stage 2
        }
    }
    private void readA(string type)
    {
        int num = int.Parse(Console.ReadLine());
        if (type == "Assignment")
        {
            //Console.WriteLine(s_dalAssignment?.Read(num)); //stage 1
            Console.WriteLine(s_dal.Assignment?.Read(num)); //stage 2
        }
        if (type == "Call")
        {
            //Console.WriteLine(s_dalCall?.Read(num)); //stage 1
            Console.WriteLine(s_dal.Call?.Read(num)); //stage 2
        }
        if (type == "Volunteer")
        {
            //Console.WriteLine(s_dalVolunteer?.Read(num)); //stage 1
            Console.WriteLine(s_dal.Volunteer?.Read(num)); //stage 2
        }
    }
    private void updateA(string type)
    {
        if (type == "Assignment")
        {
            Console.WriteLine("Write assignment id to update:");
            //Assignment? oldItem = s_dalAssignment?.Read(int.Parse(Console.ReadLine())); //stage 1
            Assignment? oldItem = s_dal.Assignment?.Read(int.Parse(Console.ReadLine())); //stage 2

            Console.WriteLine("Write new values for update:");
            Console.WriteLine("new callId:");
           
            string input = Console.ReadLine();
            int callId;
            if (input == "")
                callId = oldItem.CallId;
            else
                callId = int.Parse(input);


            Console.WriteLine("new volunteerId:");
            input = Console.ReadLine();
            int volunteerId;
            if (input == "")
                volunteerId = oldItem.VolunteerId;
            else
                volunteerId= int.Parse(input);

            DateTime openTime = DateTime.Now;
            //DateTime closeTime = DateTime.Now + s_dalConfig.RiskRange; //stage 1
            DateTime closeTime = DateTime.Now + s_dal.Config.RiskRange; //stage 2

            Console.WriteLine("press 0 for TakenCare, " +
                    "press 1 for SelfCancel, " +
                    "press 2 for CancelAdmin, " +
                    "press 3 for CancelExpired");

            input = Console.ReadLine();
            DO.AssignmentEnum? finishType; 
            if (input == "")
                finishType = oldItem.EndTreatment;
            else
                finishType= Enum.Parse<DO.AssignmentEnum>(input);

            //s_dalAssignment.Update(new(0, callId, volunteerId, openTime, closeTime, finishType)); //stage 1
            s_dal!.Assignment.Update(new(oldItem.Id, callId, volunteerId, openTime, closeTime, finishType)); //stage 2

        }
        if (type == "Call")
        {
            Console.WriteLine("Write CallId to update:");
            //Call? oldItem = s_dalCall?.Read(int.Parse(Console.ReadLine())); //stage 1
            Call? oldItem = s_dal.Call?.Read(int.Parse(Console.ReadLine())); //stage 2
            if (oldItem != null)
            {
                Console.WriteLine("Write new values for update:");
                Console.WriteLine(
                    "press 0 for shopping, " +
                    "press 1 for cleaning, " +
                    "press 2 for repairing, " +
                    "press 3 for technologyHelp," +
                    "press 4 for talking");

                string? input = Console.ReadLine();
                DO.TypeOfCall cType;
                if (input == "")
                    cType = oldItem.CallType;
                else
                    cType = Enum.Parse<DO.TypeOfCall>(input);

                Console.WriteLine("new address:");
                string? address = Console.ReadLine();
                if (address == "")
                    address = oldItem.CallAddress;

                double latitude = oldItem.Latitude;
                double longitude = oldItem.Longitude;
                //double latitude = double.Parse(Console.ReadLine());
                //double longitude = double.Parse(Console.ReadLine());


                DateTime? openTime = DateTime.Now;
                //DateTime? maxTime = DateTime.Now + s_dalConfig.RiskRange; //stage 1
                DateTime? maxTime = DateTime.Now + s_dal.Config.RiskRange; //stage 2

                Console.WriteLine("new description :");
                string? description = Console.ReadLine();
                if (description == "")
                    description = oldItem.CallAddress;

                //s_dalCall?.Update(new(oldItem.Id, cType, address, 0, 0, DateTime.MinValue, description, null)); //stage 1
                s_dal.Call?.Update(new(oldItem.Id, cType, address, 0, 0, DateTime.MinValue, description, null)); //stage 2
            }
            else
                throw new Exception($"Object of type Call with this ID does not exists");
        }
        if (type == "Volunteer")
        {
            Console.WriteLine("Write ID to update:");
            //Volunteer? oldItem = s_dalVolunteer?.Read(int.Parse(Console.ReadLine())); //stage 1
            Volunteer? oldItem = s_dal.Volunteer?.Read(int.Parse(Console.ReadLine())); //stage 2
            if (oldItem != null)
            {
                Console.WriteLine("Write new values for update:");
                Console.WriteLine("new name:");
                string? fullName = Console.ReadLine();
                if (fullName == null)
                    fullName = oldItem.FullName;

                Console.WriteLine("new phone number:");
                string? phoneNumber = Console.ReadLine();
                if (phoneNumber == null)
                    phoneNumber = oldItem.PhoneNumber;

                Console.WriteLine("new email:");
                string? email = Console.ReadLine();
                if (email =="")
                    email = oldItem.Email;

                Console.WriteLine("press 0 for manager, " +
                    "press 1 for volunteer:");

                string? input = Console.ReadLine();
                DO.Role job;
                if (input=="")
                    job = oldItem.Job;
                else
                    job = Enum.Parse<DO.Role>(input);
           
                Console.WriteLine("active?");
                string? tempActive = Console.ReadLine();
                bool active = false;
                if (tempActive != "")
                {
                    if (tempActive == "true")
                        active = true;
                }
                else
                    active = oldItem.Active;

                Console.WriteLine("press 0 for air, " +
                    "press 1 for walking, " +
                    "press 2 for car");

                input = Console.ReadLine();
                DO.RangeType distance;
                if (input == "")
                    distance = oldItem.Distance;
                else
                    distance = Enum.Parse<DO.RangeType>(input);
                                    
                Console.WriteLine("new address:");
                string? add = Console.ReadLine() ?? oldItem.VolAddress;

                double? latitude = oldItem.Latitude;
                double? longitude = oldItem.Longitude;
                //double latitude = double.Parse(Console.ReadLine());
                //double longitude = double.Parse(Console.ReadLine());

                Console.WriteLine("new maximom distance:");
                input = Console.ReadLine();
                double? dis;
                if (input == "")
                    dis = oldItem.MaxDistance;
                else
                    dis = double.Parse(input);
                   

                //s_dalVolunteer?.Update(new(oldItem.Id, fullName, phoneNumber, email, job, active, distance, add, latitude, longitude, dis)); //stage 1
                s_dal.Volunteer?.Update(new(oldItem.Id, fullName, phoneNumber, email, job, active, distance, add, latitude, longitude, dis)); //stage 2
            }
            else
                throw new Exception($"Object of type Volunteer with this ID does not exists");
        }
    }
    private void printA()
    {
        //List<Assignment>? aList = s_dalAssignment?.ReadAll(); //stage 1
        IEnumerable<Assignment>? aList = s_dal.Assignment?.ReadAll(); //stage 2
        if (aList != null)
            foreach (var item in aList)
                Console.WriteLine(item);
    }
    private void printC()
    {
        //List<Call>? cList = s_dalCall?.ReadAll(); //stage 1
        IEnumerable<Call>? cList = s_dal.Call?.ReadAll(); //stage 2
        if (cList != null)
            foreach (var item in cList)
                Console.WriteLine(item);
    }
    private void printV()
    {
        //List<Volunteer>? vList = s_dalVolunteer?.ReadAll(); //stage 1
        IEnumerable<Volunteer>? vList = s_dal.Volunteer?.ReadAll(); //stage 2
        if (vList != null)
            foreach (var item in vList)
                Console.WriteLine(item);
    }
}