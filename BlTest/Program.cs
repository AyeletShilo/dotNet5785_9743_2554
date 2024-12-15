using BO;
using DalApi;
using DalTest;
using DO;
using Helpers;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    enum choiceMain { exit, call, volunteer, admin };

    enum CallChoice { exit, create, update, delete, getClosedCalls, getOpenedCalls, read, readAll, howManyCalls, callToTreatment, updateCancelTreatment, updateEndTreatment };
    enum VolunteerChoice { exit, readAll, read, update, delete, create};
    private static void Main(string[] args)
    {
        bool stop = false;
        while (stop is not true)
        {
            Console.WriteLine(@"
Press 1 to display submenu for ICall,
Press 2 to display submenu for IVolunteer,
Press 3 to display submenu for IAdmin,
Press 0 to exit"
        );

            string input = Console.ReadLine();

            if (Enum.TryParse(typeof(choiceMain), input, true, out object? result) && result is choiceMain c)
                switch (c)
                {
                    case choiceMain.call:
                        CallSubMenu();
                        break;
                    case choiceMain.volunteer:
                        VolunteerSubMenu();
                        break;
                    //case choiceMain.admin:
                    //    AdminSubMenu();
                    //    break;
                    default:
                        stop = true;
                        break;
                }

        }
    }
    private static void CallSubMenu()
    {
        bool stopC = false;
        while (stopC != true)
        {
            Console.WriteLine(@"
Press 1 to create new Call,
Press 2 to update an existing Call,
Press 3 to delete an existing Call,
Ptess 4 to print closed calls,
Press 5 to print opened calls,
Press 6 to print existing call,
Press 7 to print all calls,
Press 8 to print calls amounts,
Press 9 to choose a call for treatment,
press 10 to update call to canceld call,
Press 11 to update call to closed call,
Press 0 to exit"
        );
            string inputB = Console.ReadLine();

            if (Enum.TryParse(typeof(CallChoice), inputB, true, out object? result) && result is CallChoice c)
                switch (c)
                {
                    case CallChoice.create:
                        try
                        {
                            BO.Call NewCall = CreateCall();
                            s_bl.Call.Create(NewCall);
                        }
                        catch (BO.BlIntegrityOfValuesException ex)
                        {
                            Console.WriteLine("BlIntegrityOfValuesException");
                            Console.WriteLine(ex);
                        }
                        break;
                    case CallChoice.update:
                        try
                        {
                            BO.Call UpdateCall = CreateCall();
                            s_bl.Call.Update(UpdateCall);
                        }
                        catch (BO.BlDoesNotExistException ex1)
                        {
                            Console.WriteLine("BlDoesNotExistException");
                            Console.WriteLine(ex1);
                        }
                        catch (BlIntegrityOfValuesException ex2)
                        {
                            Console.WriteLine("BlIntegrityOfValuesException");
                            Console.WriteLine(ex2);
                        }
                        break;
                    case CallChoice.delete:
                        try
                        {
                            Console.WriteLine("ID for delete:\n");
                            int id = int.Parse(Console.ReadLine()!);
                            s_bl.Call.Delete(id);
                        }
                        catch (BO.BlDoesNotExistException ex1)
                        {
                            Console.WriteLine("BlDoesNotExistException");
                            Console.WriteLine(ex1);
                        }
                        break;
                    case CallChoice.getClosedCalls:
                        Console.WriteLine("ID for calls:\n");
                        int callSid = int.Parse(Console.ReadLine()!);

                        Console.WriteLine("Call type for calls:\n");
                        string? type = Console.ReadLine();
                        BO.CallType? callType= type!=null? Enum.Parse<BO.CallType>(type) : null;

                        Console.WriteLine("value to sort calls:\n");
                        string? sort = Console.ReadLine();
                        BO.CloseCallData? callSort = sort != null ? Enum.Parse<BO.CloseCallData>(sort) : null;

                        IEnumerable<BO.ClosedCallInList> closedCall=s_bl.Call.GetClosedCalls(callSid, callType,callSort);
                        foreach (BO.ClosedCallInList call in closedCall)
                            Console.WriteLine(call + "\n");

                        break;
                    case CallChoice.getOpenedCalls:
                        Console.WriteLine("ID for calls:\n");
                        int callId = int.Parse(Console.ReadLine()!);

                        Console.WriteLine("Call type for calls:\n");
                        string? openType = Console.ReadLine();
                        BO.CallType? openCallType = openType != null ? Enum.Parse<BO.CallType>(openType) : null;

                        Console.WriteLine("value to sort calls:\n");
                        string? openSort = Console.ReadLine();
                        BO.OpenCallData? openCallSort = openSort != null ? Enum.Parse<BO.OpenCallData>(openSort) : null;

                        IEnumerable<BO.OpenCallInList> OpenCalls = s_bl.Call.GetOpenedCalls(callId, openCallType, openCallSort);
                        foreach (BO.OpenCallInList call in OpenCalls)
                            Console.WriteLine(call + "\n");
                          
                        break;
                    case CallChoice.read:
                        try
                        {
                            Console.WriteLine("ID for print:\n");
                            int id = int.Parse(Console.ReadLine()!);
                            Console.WriteLine(s_bl.Call.Read(id) + "\n");
                            
                        }
                        catch (BO.BlDoesNotExistException ex1)
                        {
                            Console.WriteLine("BlDoesNotExistException");
                            Console.WriteLine(ex1);
                        }
                        break;
                    case CallChoice.readAll:
                      
                        Console.WriteLine("Call type for calls:\n");
                        string? filter = Console.ReadLine();
                        BO.CallData? callFilter = filter != null ? Enum.Parse<BO.CallData>(filter) : null;

                        Console.WriteLine("object to sort calls:\n");
                        string? objectSort = Console.ReadLine();
                        object? objectCallsSort = objectSort != null ? object.Parse(objectSort) : null; //?

                        Console.WriteLine("value to sort calls:\n");
                        string? Sort = Console.ReadLine();
                        BO.CallData? callsSort = Sort != null ? Enum.Parse<BO.CallData>(Sort) : null;

                        IEnumerable<BO.CallInList> Calls = s_bl.Call.ReadAll(callFilter, callsSort, objectCallsSort);
                        foreach (BO.CallInList call in Calls)
                            Console.WriteLine(call + "\n");
                        break;
                    case CallChoice.howManyCalls:
                        //IEnumerable<IGrouping<int open,int inTreatment,int closed,int expired,int openInRisk>> howMany
                           
                        break;
                    case CallChoice.callToTreatment:
                        try
                        {
                            Console.WriteLine("ID for calll treatment:\n");
                            int CallId = int.Parse(Console.ReadLine()!);
                            Console.WriteLine("ID for volunteer:\n");
                            int VolId = int.Parse(Console.ReadLine()!);
                            s_bl.Call.CallToTreatment(VolId, CallId);
                        }
                        catch(BO.BlDoesAlreadyExistException ex1)
                        {
                            Console.WriteLine("BO.BlDoesAlreadyExistException \n" + ex1);
                        }
                        catch(BO.BlDoesNotExistException ex2)
                        {
                            Console.WriteLine("BO.BlDoesNotExistException \n" + ex2);
                        }
                        break;
                    case CallChoice.updateCancelTreatment:
                        try
                        {
                            Console.WriteLine("ID for call assignment:\n");
                            int assignmentId = int.Parse(Console.ReadLine()!);
                            Console.WriteLine("ID for volunteer:\n");
                            int VolId = int.Parse(Console.ReadLine()!);
                            s_bl.Call.UpdateCancelTreatment(VolId, assignmentId);
                        }
                        catch (BO.BlCantUpdateException ex1)
                        {
                            Console.WriteLine("BO.BlCantUpdateException");
                            Console.WriteLine(ex1 + "\n");
                        }
                        catch (BO.BlDoesNotExistException ex2)
                        {
                            Console.WriteLine("BO.BlDoesNotExistException");
                            Console.WriteLine(ex2 + "\n");
                        }
                        break;
                    case CallChoice.updateEndTreatment:
                        try
                        {
                            Console.WriteLine("ID for call assignment:\n");
                            int assignmentId = int.Parse(Console.ReadLine()!);
                            Console.WriteLine("ID for volunteer:\n");
                            int VolId = int.Parse(Console.ReadLine()!);
                            s_bl.Call.UpdateEndTreatment(VolId, assignmentId);
                        }
                        catch (BO.BlCantUpdateException ex1)
                        {
                            Console.WriteLine("BO.BlCantUpdateException \n" + ex1);
                        }
                        catch (BO.BlDoesNotExistException ex2)
                        {
                            Console.WriteLine("BO.BlDoesNotExistException \n" + ex2);
                        }
                        break;
                    default:
                        stopC = true;
                        break;
                }
        }
    }

    private static BO.Call CreateCall()
    {

        Console.WriteLine(
"press 0 for shopping," +
"press 1 for cleaning," +
"press 2 for repairing," +
"press 3 for technologyHelp," +
"press 4 for talking");

        string input = Console.ReadLine()!;
        BO.CallType cType = Enum.Parse<BO.CallType>(input);

        Console.WriteLine("new description:");
        string? description = Console.ReadLine();

        Console.WriteLine("new address:");
        string address = Console.ReadLine()!;

        double latitude = default;
        double longitude = default;

        DateTime openTime = s_bl.Admin.GetClock();

        Console.WriteLine("new max time:");
        string? newMaxTime = Console.ReadLine();
        DateTime? maxTime = newMaxTime is not null ? DateTime.Parse(newMaxTime) : null;
       
        BO.CallStatus callStatus = BO.CallStatus.Opened;

        BO.Call newCall= new { Id=0, CallType= cType, Description= description, CallAddress= address, Latitude=latitude, Longitude=longitude, OpenTime=openTime, MaxCloseTime=maxTime, Status = callStatus, CallAssignments = (List<BO.CallAssignInList>?)null };
        return newCall;
    }

    private static void VolunteerSubMenu()
    {
        bool stopV = false;
        while (stopV != true)
        {
            Console.WriteLine(@"
Press 1 to print all calls,
Press 2 to print an existing call,
Press 3 to update an existing call,
Press 4 to delete an existing call,
Press 5 to create new call,
Press 0 to exit"
        );
            string inputV = Console.ReadLine();

            if (Enum.TryParse(typeof(VolunteerChoice), inputV, true, out object? result) && result is VolunteerChoice c)
                switch (c)
                {
                    case VolunteerChoice.readAll:

                        Console.WriteLine("Press 1 for active volunteers," +
                            "Press 0 for inactive volunteers\n");
                        string? active = Console.ReadLine();
                        bool? isActive = active is not null ? bool.Parse(active) : null;
                        Console.WriteLine("Value for sort:\n");
                        string? isSort = Console.ReadLine();
                        BO.VolunteerData? sort = isSort is not null ? Enum.Parse<BO.VolunteerData>(isSort) : null;

                        IEnumerable<BO.VolunteerInList> volunteerList = s_bl.Volunteer.ReadAll(isActive, sort);
                        foreach (BO.VolunteerInList vol in volunteerList)
                            Console.WriteLine(vol + "\n");
                        break;
                    case VolunteerChoice.read:
                        try
                        {
                            Console.WriteLine("ID for call:\n");
                            int id = int.Parse(Console.ReadLine()!);
                            BO.Volunteer volunteer = s_bl.Volunteer.Read(id);
                            Console.WriteLine(volunteer + "\n");
                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine("BO.BlDoesNotExistException \n" + ex);
                        }

                        break;
                    case VolunteerChoice.update:

                        try
                        {
                            Console.WriteLine("ID for volunteer:\n");
                            int id = int.Parse(Console.ReadLine()!);
                            BO.Volunteer volunteer = CreateVolunteer();
                            s_bl.Volunteer.Update(id, volunteer);
                        }
                        catch(BO.BlNullPropertyException ex1)
                        {
                            Console.WriteLine("BlNullPropertyException: \n" + ex1);
                        }
                        catch(BO.BlIntegrityOfValuesException ex2)
                        {
                            Console.WriteLine("BO.BlIntegrityOfValuesException: \n" + ex2);
                        }
                        catch(BO.BlDoesNotExistException ex3)
                        {
                            Console.WriteLine("BO.BlDoesNotExistException \n" + ex3);
                        }
                        break;
                    case VolunteerChoice.delete:

                        try
                        {
                            Console.WriteLine(" volunteer ID to delete:\n ");
                            int id = int.Parse(Console.ReadLine()!);
                            s_bl.Volunteer.Delete(id);
                        }
                        catch (BO.BlDoesNotExistException ex1)
                        {
                            Console.WriteLine("BO.BlDoesNotExistException \n" + ex1);
                        }
                        catch(BO.BlCannotBeDeletedException ex2)
                        {
                            Console.WriteLine("BlCannotBeDeletedException \n" + ex2);
                        }
                        break;
                    case VolunteerChoice.create:

                        try
                        {
                            BO.Volunteer volunteer = CreateVolunteer();
                            s_bl.Volunteer.Create(volunteer);
                        }
                        catch (BO.BlIntegrityOfValuesException ex2)
                        {
                            Console.WriteLine("BO.BlIntegrityOfValuesException: \n" + ex2);
                        }
                        break;
                    default:
                        stopV = true;
                        break;   
                }
        }
    }

    private static BO.Volunteer CreateVolunteer()
    {

        Console.WriteLine("Volunteer name:\n");
        string name = Console.ReadLine()!;

        Console.WriteLine("Volunteer phone number: \n");
        string phoneNumber = Console.ReadLine()!;

        Console.WriteLine("Volunteer email:\n");
        string email = Console.ReadLine()!;

        Console.WriteLine("Volunteer address:\n");
        string? address = Console.ReadLine();

        double latitude = default;
        double longitude = default;

        Console.WriteLine(
            "Press 0 for manager," +
            "Press 1 for donater\n");
        BO.Role job = Enum.Parse<BO.Role> (Console.ReadLine()!);

        Console.WriteLine("" +
            "Press 0 for inactive volunteer," +
            "Press 1 for active volunteer");
        bool isActive = bool.Parse(Console.ReadLine()!);

        Console.WriteLine("Volunteer max distance:");
        string? isMaxDis = Console.ReadLine();
        double? maxDis = isMaxDis is not null ? double.Parse(isMaxDis) : null;

        Console.WriteLine(
           "Press 0 for air distance," +
           "Press 1 for walking distance," +
           "Press 2 for car distance\n");
        BO.DisType distance = Enum.Parse<BO.DisType>(Console.ReadLine()!);

        BO.Volunteer newVol = new
        {
            Id = 0,
            FullName = name,
            PhoneNumber = phoneNumber,
            Email = email,
            Address = address,
            Latitude = latitude,
            Longitude = longitude,
            Job = job,
            IsActive = isActive,
            MaxDis = maxDis,
            Distance = distance,
            HandleCalls = 0,
            CancelCalls = 0,
            ExpiredCalls = 0,
            InCall = null
        };
        return newVol;
       
    }
         
}


