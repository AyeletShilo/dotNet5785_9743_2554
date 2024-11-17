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
            Console.WriteLine(  $@"Press 1 to display submenu for Assignment,
Press 2 to display submenu for Call,
Press 3 to display submenu for Volunteer,
Press 4 to perform an initialization,
Press 5 to show all data objects,
Press 6 to display submenu for configuration,
Press 7 to reset the database and the configuration,
Press 0 to exit\n"
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
            Console.WriteLine(@"Press 1 to create new object,
Press 2 to print an object,
Press 3 to print all the objects,
Press 4 to update an existing object,
Press 5 to delete an existing object
Press 6 to delete all the objects,
Press 0 to exit\n"
        );
            string inputB = Console.ReadLine();

            if (Enum.TryParse(typeof(choicesA), inputB, true, out object? result) && result is choicesA c)
                switch (c)
            {
                
                case choicesA.create:
                    createA(type);
                    break;
                case choicesA.read:
                    Console.WriteLine("Write ID to read:\n");
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
                    Console.WriteLine("Write ID to delete:\n");
                    int num = (int)Console.Read();
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
Press 0 to exit\n");

            string inputC = Console.ReadLine();

            if (Enum.TryParse(typeof(choicesB), inputC, true, out object? result) && result is choicesB b)
            {
                switch (b)
                {

                    case choicesB.minute:
                        s_dalConfig.Clock = new(s_dalConfig.Clock.Minute + 1);
                        break;
                    case choicesB.hour:
                        s_dalConfig.Clock = new(s_dalConfig.Clock.Hour + 1);
                        break;
                    case choicesB.day:
                        s_dalConfig.Clock = new(s_dalConfig.Clock.Day + 1);
                        break;
                    case choicesB.month:
                        s_dalConfig.Clock = new(s_dalConfig.Clock.Month + 1);
                        break;
                    case choicesB.year:
                        s_dalConfig.Clock = new(s_dalConfig.Clock.Year + 1);
                        break;
                    case choicesB.clock:
                        Console.WriteLine(s_dalConfig?.Clock);
                        break;
                    case choicesB.update:
                        Console.WriteLine("Press 1 for next call id,"+
                        "Press 2 next Assignment Id,"+
                        "Press 3 for clock," +
                        "Press 4 for risk range\n");

                        choicesC c = (choicesC)Console.Read();
                        Console.WriteLine("Write update value:\n");
                        switch (c)
                        {
                            case choicesC.clock:
                                int newClock = Console.Read();
                                s_dalConfig.Clock = new(newClock);
                                break;
                            case choicesC.risk:
                                int newRisk = Console.Read();
                                s_dalConfig.RiskRang = new(newRisk);
                                break;
                        }
                        break;
                    case choicesB.read:
                        Console.WriteLine("Press 1 for clock,"+
                        "Press 2 for risk range\n");
                        choicesC d;
                        d = (choicesC)Console.Read();

                        switch (d)
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

            Console.WriteLine("new callId:\n");
            int callId = (int)Console.Read();

            Console.WriteLine("new volunteerId:\n");
            int volunteerId = (int)Console.Read();

            DateTime openTime = DateTime.Now;
            DateTime closeTime = DateTime.Now + s_dalConfig.RiskRang;

            Console.WriteLine("press 0 for TakenCare"+
                    "press 1 for SelfCancel,"+
                    "press 2 for CancelAdmin," +
                    "press 3 for CancelExpired\n");

            string input = Console.ReadLine();
            DO.AssignmentEnum? finishType = Enum.Parse<DO.AssignmentEnum>(input);
        }
        if (type == "Call")
        {
            Console.WriteLine("new call type:" +
                "press 1 for shopping"+
                "press 2 for cleaning"+
                "press 3 for repairing"+
                "press 4 for thechnologyHelp,"+
                "press 5 for talking");

            string input = Console.ReadLine();
            DO.TypeOfCall cType = Enum.Parse<DO.TypeOfCall>(input);


            Console.WriteLine("new address:\n");
            string address = Console.ReadLine()!;

            double latitude = (double)Console.Read();
            double longitude = (double)Console.Read();

            DateTime? tempOpen = s_dalConfig?.Clock;
            DateTime openTime = (DateTime)tempOpen!;

            DateTime maxTime = openTime + s_dalConfig.RiskRang;

            Console.WriteLine("new description :\n");
            string description = Console.ReadLine();

            Call NewItem = new(0, cType, address, 0, 0, openTime, description, maxTime);
            s_dalCall?.Create(NewItem);
        }
        if (type == "Volunteer")
        {
            Console.WriteLine("\nnew Id:");
            int id = Console.Read();

            Console.WriteLine("new name:\n");
            string fullName = Console.ReadLine()!;

            Console.WriteLine("new phone number:\n");
            string phoneNumber = Console.ReadLine()!;

            Console.WriteLine("new email:\n");
            string email = Console.ReadLine()!;

            Console.WriteLine("press 0 for manager"+
                "press 1 for volunteer:\n");

            string input = Console.ReadLine();
            DO.Role job = Enum.Parse<DO.Role>(input);

            Console.WriteLine("active?\n");
            string tempActive = Console.ReadLine()!;
            bool active = false;
            if (tempActive == "true")
                active = true;

            Console.WriteLine("press 0 for air" +
                               "press 1 for walking" +
                               "press 2 for car\n");

            input = Console.ReadLine();
            DO.RangeType distance = Enum.Parse<DO.RangeType>(input);

            Console.WriteLine("new address:\n");
            string? add = Console.ReadLine();

            Console.WriteLine("new maximom distance:\n");
            double dis = (double)Console.Read();

            Volunteer NewItem = new(id, fullName, phoneNumber, email, job, active, distance, add, 0, 0, dis);
            s_dalVolunteer?.Create(NewItem);


        }
    }
    private void readA(string type)
    {
        int num = (int)Console.Read();
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
            Assignment? oldItem = s_dalAssignment?.Read((int)Console.Read());

            Console.WriteLine("Write new values for update:\n");
            Console.WriteLine("new callId:\n");
            int callId= (int)Console.Read();
            if (callId == null)
                callId = oldItem.CallId;

            Console.WriteLine("new volunteerId:\n");
            int volunteerId = (int)Console.Read();
            if (volunteerId == null)
                volunteerId = oldItem.VolunteerId;
            
            DateTime openTime = DateTime.Now;
            DateTime closeTime = DateTime.Now+ s_dalConfig.RiskRang;

            Console.WriteLine("press 0 for TakenCare" +
                    "press 1 for SelfCancel," +
                    "press 2 for CancelAdmin," +
                    "press 3 for CancelExpired\n");

            string input = Console.ReadLine();
            DO.AssignmentEnum? finishType = Enum.Parse<DO.AssignmentEnum>(input);
            if (finishType == null)
                finishType = oldItem.EndTreatment;

            s_dalAssignment.Update(new(0, callId, volunteerId, openTime, closeTime, finishType));

        }
        if (type == "Call")
        {
            Call? oldItem = s_dalCall?.Read((int)Console.Read());
            if (oldItem != null)
            {
                Console.WriteLine("Write new values for update:\n");
                Console.WriteLine("new call type:" +
                    "press 1 for shopping" +
                    "press 2 for cleaning" +
                    "press 3 for repairing" +
                    "press 4 for thechnologyHelp," +
                    "press 5 for talking");

                string input = Console.ReadLine();
                DO.TypeOfCall cType = Enum.Parse<DO.TypeOfCall>(input);
                if (cType == null)
                    cType = oldItem.CallType;

                Console.WriteLine("new address:\n");
                string? address = Console.ReadLine();
                if (address == null)
                    address = oldItem.CallAddress;

                double latitude = (double)Console.Read();
                double longitude = (double)Console.Read();
                

                DateTime? openTime = DateTime.Now;
                DateTime? maxTime = DateTime.Now + s_dalConfig.RiskRang;

                Console.WriteLine("new description :\n");
                string? description = Console.ReadLine();
                if (description == null)
                    description = oldItem.CallAddress;

                Call NewItem = new(oldItem.Id, cType, address, 0, 0, DateTime.MinValue, description, null);
                s_dalCall?.Update(NewItem);

            }
            else
                throw new Exception($"Object of type Call with this ID does not exists");
        }
        if (type == "Volunteer")
        {
            Volunteer? oldItem = s_dalVolunteer?.Read((int)Console.Read());
            if (oldItem != null)
            {
                Console.WriteLine("Write new values for update:\n");
                Console.WriteLine("new name:\n");
                string? fullName = Console.ReadLine();
                if (fullName == null)
                    fullName = oldItem.FullName;

                Console.WriteLine("new phone number:\n");
                string? phoneNumber = Console.ReadLine();
                if (phoneNumber == null)
                    phoneNumber = oldItem.PhoneNumber;

                Console.WriteLine("new email:\n");
                string? email = Console.ReadLine();
                if (email == null)
                    email = oldItem.Email;

                Console.WriteLine("press 0 for manager" +
                    "press 1 for volunteer:\n");

                string input = Console.ReadLine();
                DO.Role? tempRole = Enum.Parse<DO.Role>(input);
                DO.Role job;
                if (tempRole is null)
                    job = oldItem.Job;
                else
                    job = (DO.Role)tempRole;

                Console.WriteLine("active?\n");
                string? tempActive = Console.ReadLine();
                bool active = false;
                if (tempActive != null)
                    if (tempActive == "true")
                        active = true;
                    else
                        active = oldItem.Active;

                Console.WriteLine("press 0 for air"+
                    "press 1 for walking"+
                    "press 2 for car\n");


                input = Console.ReadLine();
                DO.RangeType? temp = Enum.Parse<DO.RangeType>(input);
                DO.RangeType distance;
                if (temp is null)
                    distance = oldItem.Distance;
                else
                    distance = (DO.RangeType)temp;

                Console.WriteLine("new address:\n");
                string? add = Console.ReadLine() ?? oldItem.VolAddress;

                double latitude = (double)Console.Read();
                double longitude = (double)Console.Read();

                Console.WriteLine("new maximom distance:\n");
                double? dis = (double)Console.Read();
                if (dis == null)
                    dis = oldItem.MaxDistance;

                Volunteer NewItem = new(oldItem.Id, fullName, phoneNumber, email, job, active, distance, add, 0, 0, dis);
                s_dalVolunteer?.Update(NewItem);

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



