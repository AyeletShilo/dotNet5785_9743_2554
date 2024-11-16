using Dal;
using DalApi;
using DO;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;


namespace DalTest
{
    internal class Program
    {
        enum choices { exit, assignment, call, volunteer, init, print, congfi, reset };
        enum choicesA { exit, create, read, readAll, update, delete, deleteAll };
        enum choicesB { exit, minute, hour, day, month, year, clock, update, read, delete };
        enum choicesC { call, assignment, clock, risk };

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
            Console.WriteLine(@"Press 1 to display submenu for Assignment,
            Press 2 to display submenu for Call,
            Press 3 to display submenu for Volunteer,
            Press 4 to perform an initialization,
            Press 5 to show all data objects,
            Press 6 to display submenu for configuration,
            Press 7 to reset the database and the configuration,
            Press 0 to exit\n"
            );

            choices c = (choices)Console.Read();
            
            bool stop = false;
            while (stop is not true)
            {
                try
                {
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
                            //Initialization.Do(s_dalAssignment, s_dalCall, s_dalVolunteer, s_dalConfig);
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
            Console.WriteLine(@"Press 1 to create new object,
                                Press 2 to print an object,
                                Press 3 to print all the objects,
                                Press 4 to update an existing object,
                                Press 5 to delete an existing object
                                Press 6 to delete all the objects,
                                Press 0 to exit\n"
            );

            choicesA c;
            c = (choicesA)Console.Read();

            bool stopA = false;
            while (stopA != true)
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
        private void subConfig()
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
            choicesB b;
            b = (choicesB)Console.Read();

            bool stopB = false;
            while (stopB != true)
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
                        Console.WriteLine(@"Press 1 for next call id,
                        Press 2 next Assignment Id,
                        Press 3 for clock,
                        Press 4 for risk range\n");

                        choicesC c;
                        c = (choicesC)Console.Read();
                        Console.WriteLine("Write update value:\n");
                        switch (c)
                        {
                            case choicesC.call:
                              int newCall=Console.Read();
                                //s_dalConfig.nextCallId = new(newCall);
                                break;
                            case choicesC.assignment:
                                int newAssignment=Console.Read();
                                //s_dalConfig.nextAssignmentId =new(newAssignment);
                                break;
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
                        Console.WriteLine(@"Press 1 for next call id,
                        Press 2 next Assignment Id,
                        Press 3 for clock,
                        Press 4 for risk range\n");
                        choicesC d;
                        d = (choicesC)Console.Read();

                        switch (d)
                        {
                            case choicesC.call:
                                break;
                            case choicesC.assignment:
                                break;
                            case choicesC.clock:
                                break;
                            case choicesC.risk:
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
        private void createA(string type)
        {
            //if (type == "Assignment")
            //?//
            if (type == "Call")
            {
                Console.WriteLine(@"new call type:
                press 1 for shopping
                press 2 for cleaning
                press 3 for repairing
                press 4 for thechnologyHelp,
                press 5 for talking ");


                DO.Type cType = (DO.Type)Console.Read();

                Console.WriteLine("new address:\n");
                string address = Console.ReadLine()!;

                //? double latitude=
                //? double longitude=

                DateTime? tempOpen = s_dalConfig?.Clock;
                DateTime openTime = (DateTime)tempOpen!;
                    
                DateTime maxTime = openTime + s_dalConfig.RiskRang;

                Console.WriteLine("new description :\n");
                string? description = Console.ReadLine();

                Call NewItem = new(0, cType, address, 0, 0, openTime, description, maxTime);
                s_dalCall?.Create(NewItem);



            }
            if (type == "Volunteer")
            {
                Console.WriteLine("new Id:\n");
                int id = Console.Read();

                Console.WriteLine("new name:\n");
                string fullName = Console.ReadLine()!;

                Console.WriteLine("new phone number:\n");
                string phoneNumber = Console.ReadLine()!;

                Console.WriteLine("new email:\n");
                string email = Console.ReadLine()!;

                Console.WriteLine(@"press 0 for manager
                press 1 for volunteer:\n");

                DO.Role job = (DO.Role)Console.Read();

                Console.WriteLine("active?\n");
                string tempActive = Console.ReadLine()!;
                bool active = false;
                if (tempActive == "true")
                    active = true;


                Console.WriteLine(@"press 0 for manager
                press 1 for volunteer:\n");

                DO.RangeType distance = (DO.RangeType)Console.Read();

                Console.WriteLine("new address:\n");
                string? add = Console.ReadLine();

                //double? latitude
                //double? longitude

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
            Console.WriteLine("Write ID for update:\n");
            if (type == "Assignment")
            {
                //?//
            }
            if (type == "Call")
            {
                Call? oldItem = s_dalCall?.Read((int)Console.Read());
                if (oldItem != null)
                {
                    Console.WriteLine("Write new values for update:\n");
                    Console.WriteLine(@"new call type:
                press 1 for shopping
                press 2 for cleaning
                press 3 for repairing
                press 4 for thechnologyHelp,
                press 5 for talking ");

                    DO.Type cType = (DO.Type)Console.Read();
                    if (cType == null)
                        cType = oldItem.CallType;

                    Console.WriteLine("new address:\n");
                    string? address = Console.ReadLine();
                    if (address == null)
                        address = oldItem.CallAddress;

                    //? double latitude=
                    //? double longitude=
                    //?DateTime OpenTime=
                    //? DateTime? MaxTime=

                    Console.WriteLine("new description :\n");
                    string? description = Console.ReadLine();
                    if (description == null)
                        description = oldItem.CallAddress;

                    Call NewItem = new(oldItem.Id, cType, address,0,0,DateTime.MinValue, description,null);
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

                    Console.WriteLine(@"press 0 for manager
                    press 1 for volunteer:\n");

                    DO.Role? tempRole = (DO.Role)Console.Read();
                    DO.Role job;
                    if (tempRole is null)
                        job = oldItem.Job;
                    else
                        job=(DO.Role)tempRole;

                    Console.WriteLine("active?\n");
                    string? tempActive = Console.ReadLine();
                    bool active = false;
                    if (tempActive != null)
                        if (tempActive == "true")
                            active = true;
                        else
                            active = oldItem.Active;

                    Console.WriteLine(@"press 0 for manager
                    press 1 for volunteer:\n");

                    DO.RangeType? temp = (DO.RangeType)Console.Read();
                    DO.RangeType distance;
                    if (temp is null)
                        distance = oldItem.Distance;
                    else
                        distance = (DO.RangeType)temp;

                    Console.WriteLine("new address:\n");
                    string? add = Console.ReadLine() ?? oldItem.VolAddress;
                    //if (add == null)
                    //    add = oldItem.VolAddress;


                    //double? latitude
                    //double? longitude

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
}



















