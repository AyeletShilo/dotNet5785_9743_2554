namespace DalTest;

using Dal;
using DalApi;
using DO;
using System.Net.Mail;

enum choices { exit, assignment, call, volunteer, init, print, congfi, reset };
enum choicesA { exit, create, read, readAll, update, delete, deleteAll };
enum choicesB { exit, minute, hour, day, month, year, clock, update, read, delete };
enum choicesC { clock, risk };



internal class Program
{
    private static ICall? s_dalCall = new CallImplementation();
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
    private static IAssignment? s_dalAssignment = new AssignmentImplementation();
    private static IConfig? s_dalConfig = new ConfigImplementation();

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
                if (Enum.TryParse(typeof(choices), input, true, out object? result) && result is choices c)
                    switch (c)
                {
                    case choices.assignment:
                        subMenu("Assignment");
                        break;
                    case choices.call:
                        subMenu("Call");
                        break;
                    case choices.volunteer:
                        subMenu("Volunteer");
                        break;
                    case choices.init:
                        Initialization.Do(s_dalAssignment, s_dalCall, s_dalVolunteer, s_dalConfig);
                        break;
                    case choices.print:
                        printA();
                        Console.WriteLine("\n");
                        printC();
                        Console.WriteLine("\n");
                        printV();
                        Console.WriteLine("\n");
                        break;
                    case choices.congfi:
                        subConfig();
                        break;
                    case choices.reset:
                        s_dalAssignment?.DeleteAll();
                        s_dalCall?.DeleteAll();
                        s_dalVolunteer?.DeleteAll();
                        s_dalConfig?.Reset();
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

            if (Enum.TryParse(typeof(choicesA), inputB, true, out object? result) && result is choicesA c)
                switch (c)
            {
                
                case choicesA.create:
                    createA(type);
                    break;
                case choicesA.read:
                    Console.WriteLine("Write ID to read:");
                    readA(type);
                    break;
                case choicesA.readAll:
                    if (type == "Assignment")
                        printA();
                    if (type == "Call")
                        printC();
                    if (type == "Volunteer")
                        printV();
                    break;
                case choicesA.update:
                    updateA(type);
                    break;
                case choicesA.delete:
                    Console.WriteLine("Write ID to delete:");
                    int num = int.Parse(Console.ReadLine());
                    try
                    {
                        if (type == "Assignment")
                            s_dalAssignment?.Delete(num);
                        if (type == "Call")
                            s_dalCall?.Delete(num);
                        if (type == "Volunteer")
                            s_dalVolunteer?.Delete(num);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                    break;
                case choicesA.deleteAll:
                    if (type == "Assignment")
                        s_dalAssignment?.DeleteAll();
                    if (type == "Call")
                        s_dalCall?.DeleteAll();
                    if (type == "Volunteer")
                        s_dalVolunteer?.DeleteAll();
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

            if (Enum.TryParse(typeof(choicesB), inputC, true, out object? result) && result is choicesB b)
            {
                switch (b)
                {
                    case choicesB.minute:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1);
                        break;
                    case choicesB.hour:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
                        break;
                    case choicesB.day:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddDays(1);
                        break;
                    case choicesB.month:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddMonths(1);
                        break;
                    case choicesB.year:
                        s_dalConfig.Clock = s_dalConfig.Clock.AddYears(1);
                        break;
                    case choicesB.clock:
                        Console.WriteLine(s_dalConfig?.Clock);
                        break;
                    case choicesB.update:
                        Console.WriteLine(
                        "Press 0 for clock, " +
                        "Press 1 for risk range");
                        string inputD = Console.ReadLine();
                        if (Enum.TryParse(typeof(choicesC), inputD, true, out object? result2) && result2 is choicesC d)
                        {
                            Console.WriteLine("Write update value:");
                            switch (d)
                            {
                                case choicesC.clock:
                                    s_dalConfig.Clock = DateTime.Parse(Console.ReadLine());
                                    break;
                                case choicesC.risk:
                                    s_dalConfig.RiskRang = TimeSpan.Parse(Console.ReadLine());
                                    break;
                            }
                        }
                        break;
                    case choicesB.read:
                        Console.WriteLine("Press 0 for clock,"+
                        "Press 1 for risk range");
                        string inputE = Console.ReadLine();
                        if (Enum.TryParse(typeof(choicesC), inputE, true, out object? result3) && result3 is choicesC e)
                            switch (e)
                            {
                            case choicesC.clock:
                                Console.WriteLine(s_dalConfig.Clock);
                                break;
                            case choicesC.risk:
                                Console.WriteLine(s_dalConfig.RiskRang);
                                break;
                            }
                        break;
                    case choicesB.delete:
                        s_dalConfig?.Reset();
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
            DateTime closeTime = DateTime.Now + s_dalConfig.RiskRang;

            Console.WriteLine("press 0 for TakenCare"+
                    "press 1 for SelfCancel,"+
                    "press 2 for CancelAdmin," +
                    "press 3 for CancelExpired");

            string input = Console.ReadLine();
            DO.AssignmentEnum? finishType = Enum.Parse<DO.AssignmentEnum>(input);

            s_dalAssignment.Create(new(0,callId,volunteerId,openTime,closeTime,finishType));

        }
        if (type == "Call")
        {
            Console.WriteLine(
                "press 1 for shopping,"+
                "press 2 for cleaning,"+
                "press 3 for repairing,"+
                "press 4 for technologyHelp,"+
                "press 5 for talking");

            string input = Console.ReadLine();
            DO.TypeOfCall cType = Enum.Parse<DO.TypeOfCall>(input);


            Console.WriteLine("new address:");
            string address = Console.ReadLine()!;

            Console.WriteLine("latitude and longitude:");
            double latitude = double.Parse(Console.ReadLine());
            double longitude = double.Parse(Console.ReadLine());

            DateTime? tempOpen = s_dalConfig?.Clock;
            DateTime openTime = (DateTime)tempOpen!;

            DateTime maxTime = openTime + s_dalConfig.RiskRang;

            Console.WriteLine("new description:");
            string description = Console.ReadLine();

            s_dalCall?.Create(new(0, cType, address, latitude, longitude, openTime, description, maxTime));
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

            Console.WriteLine("press 0 for manager, "+
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

            s_dalVolunteer?.Create(new(id, fullName, phoneNumber, email, job, active, distance, add, 0, 0, dis));


        }
    }
    private void readA(string type)
    {
        int num = int.Parse(Console.ReadLine());
        if (type == "Assignment")
            Console.WriteLine(s_dalAssignment?.Read(num));
        if (type == "Call")
            Console.WriteLine(s_dalCall?.Read(num));
        if (type == "Volunteer")
            Console.WriteLine(s_dalVolunteer?.Read(num));
    }
    private void updateA(string type)
    {
        if (type == "Assignment")
        {
            Assignment? oldItem = s_dalAssignment?.Read(int.Parse(Console.ReadLine()));

            Console.WriteLine("Write new values for update:");
            Console.WriteLine("new callId:");
            int callId= int.Parse(Console.ReadLine());
            if (callId == null)
                callId = oldItem.CallId;

            Console.WriteLine("new volunteerId:");
            int volunteerId = int.Parse(Console.ReadLine());
            if (volunteerId == null)
                volunteerId = oldItem.VolunteerId;
            
            DateTime openTime = DateTime.Now;
            DateTime closeTime = DateTime.Now+ s_dalConfig.RiskRang;

            Console.WriteLine("press 0 for TakenCare, " +
                    "press 1 for SelfCancel, " +
                    "press 2 for CancelAdmin, " +
                    "press 3 for CancelExpired");

            string input = Console.ReadLine();
            DO.AssignmentEnum? finishType = Enum.Parse<DO.AssignmentEnum>(input);
            if (finishType == null)
                finishType = oldItem.EndTreatment;

            s_dalAssignment.Update(new(0, callId, volunteerId, openTime, closeTime, finishType));

        }
        if (type == "Call")
        {
            Call? oldItem = s_dalCall?.Read(int.Parse(Console.ReadLine()));
            if (oldItem != null)
            {
                Console.WriteLine("Write new values for update:");
                Console.WriteLine(
                    "press 1 for shopping, " +
                    "press 2 for cleaning, " +
                    "press 3 for repairing, " +
                    "press 4 for technologyHelp," +
                    "press 5 for talking");

                string input = Console.ReadLine();
                DO.TypeOfCall cType = Enum.Parse<DO.TypeOfCall>(input);
                if (cType == null)
                    cType = oldItem.CallType;

                Console.WriteLine("new address:");
                string? address = Console.ReadLine();
                if (address == null)
                    address = oldItem.CallAddress;

                double latitude = double.Parse(Console.ReadLine());
                double longitude = double.Parse(Console.ReadLine());
                

                DateTime? openTime = DateTime.Now;
                DateTime? maxTime = DateTime.Now + s_dalConfig.RiskRang;

                Console.WriteLine("new description :");
                string? description = Console.ReadLine();
                if (description == null)
                    description = oldItem.CallAddress;

                s_dalCall?.Update(new(oldItem.Id, cType, address, 0, 0, DateTime.MinValue, description, null));

            }
            else
                throw new Exception($"Object of type Call with this ID does not exists");
        }
        if (type == "Volunteer")
        {
            Console.WriteLine("Write ID to update:");
            Volunteer? oldItem = s_dalVolunteer?.Read(int.Parse(Console.ReadLine()));
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
                if (email == null)
                    email = oldItem.Email;

                Console.WriteLine("press 0 for manager, " +
                    "press 1 for volunteer:");

                string input = Console.ReadLine();
                DO.Role? tempRole = Enum.Parse<DO.Role>(input);
                DO.Role job;
                if (tempRole is null)
                    job = oldItem.Job;
                else
                    job = (DO.Role)tempRole;

                Console.WriteLine("active?");
                string? tempActive = Console.ReadLine();
                bool active = false;
                if (tempActive != null)
                    if (tempActive == "true")
                        active = true;
                    else
                        active = oldItem.Active;

                Console.WriteLine("press 0 for air, "+
                    "press 1 for walking, "+
                    "press 2 for car");


                input = Console.ReadLine();
                DO.RangeType? temp = Enum.Parse<DO.RangeType>(input);
                DO.RangeType distance;
                if (temp is null)
                    distance = oldItem.Distance;
                else
                    distance = (DO.RangeType)temp;

                Console.WriteLine("new address:");
                string? add = Console.ReadLine() ?? oldItem.VolAddress;

                double latitude = double.Parse(Console.ReadLine());
                double longitude = double.Parse(Console.ReadLine());

                Console.WriteLine("new maximom distance:");
                double? dis = double.Parse(Console.ReadLine());
                if (dis == null)
                    dis = oldItem.MaxDistance;

                s_dalVolunteer?.Update(new(oldItem.Id, fullName, phoneNumber, email, job, active, distance, add, latitude, longitude, dis));
            }
            else
                throw new Exception($"Object of type Volunteer with this ID does not exists");
        }
    }
    private void printA()
    {
        List<Assignment>? aList = s_dalAssignment?.ReadAll();
        if (aList != null)
            foreach (var item in aList)
                Console.WriteLine(item);
    }
    private void printC()
    {
        List<Call>? cList = s_dalCall?.ReadAll();
        if (cList != null)
            foreach (var item in cList)
                Console.WriteLine(item);
    }
    private void printV()
    {
        List<Volunteer>? vList = s_dalVolunteer?.ReadAll();
        if (vList != null)
            foreach (var item in vList)
                Console.WriteLine(item);
    }
}