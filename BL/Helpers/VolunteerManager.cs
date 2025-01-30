//using BlApi;
using BO;
using DalApi;
using System.Text.RegularExpressions;


namespace Helpers;

internal static class VolunteerManager
{
    private static IDal _dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// Checks that all values ​​are correctly formatted.
    /// </summary>
    /// <param name="volToCheck">The volunteer whose values ​​are being tested</param>
    /// <exception cref="BO.BlIntegrityOfValuesException">Throws an exception if there is a problem with the format of one of the values.</exception>
    internal static void CheckFormat(BO.Volunteer volToCheck)
    {
        bool? isEmail = checkEmail(volToCheck.Email) ?? throw new BO.BlIntegrityOfValuesException("Error in email format");
        if (!int.TryParse(volToCheck.PhoneNumber, out int number))
            throw new BO.BlIntegrityOfValuesException("Error in PhoneNumber format");
        if (volToCheck.PhoneNumber.Length != 10 || volToCheck.PhoneNumber[0] != '0' || volToCheck.PhoneNumber[1] != '5')
            throw new BO.BlIntegrityOfValuesException("Error in PhoneNumber format");
        if (volToCheck.Password is not null && volToCheck.Password.Length < 8)
            throw new BO.BlIntegrityOfValuesException("Error in Password format");
        if (volToCheck.MaxDis < 0)
            throw new BO.BlIntegrityOfValuesException("Error in Max Distance format");
        if (volToCheck.Id < 10000000 || volToCheck.Id > 999999999)
            throw new BO.BlIntegrityOfValuesException("Error in ID format");
        if (checkId(volToCheck.Id) == false)
            throw new BO.BlIntegrityOfValuesException("Error in ID integrity");
        if (checkPass(volToCheck.Password) == false)
            throw new BO.BlIntegrityOfValuesException("Password is not strong");
    }

    /// <summary>
    /// Check that the email address received is correct
    /// </summary>
    /// <param name="email">The email address received</param>
    /// <returns>A boolean variable that evaluates to true when the email is correct.</returns>
    private static bool? checkEmail(string email)
    {
        if (email.Contains("@gmail.com") == false && email.Contains("@walla.co.il") == false && email.Contains("@g.jct.ac.il") == false)
            return null;
        if (email[0] == '.' || email[email.Length - 1] == '.')
            return null;
        if (email.Contains("..") == true || email.Contains(" ") == true)
            return null;

        return true;
    }


    /// <summary>
    /// Checking that the volunteer's values ​​are logically correct
    /// </summary>
    /// <param name="doVolunteer">The volunteer whose values ​​are being tested</param>
    /// <exception cref="BO.BlIntegrityOfValuesException">>Throws an exception if there is a problem with the logic of one of the values</exception>
    internal static async Task updateCoordinates(DO.Volunteer doVolunteer)
    {
        //if (checkId(volToCheck.Id) == false)
        //    throw new BO.BlIntegrityOfValuesException("Error in ID integrity");

        //if (checkPass(volToCheck.Password) == false)
        //    throw new BO.BlIntegrityOfValuesException("Password is not strong");

        if (doVolunteer.VolAddress != "" && doVolunteer.VolAddress != null)
        {

            double?[] AddressCoordinate = await CallManager.GetCoordinates(doVolunteer.VolAddress);

            if (AddressCoordinate is null)
                throw new BO.BlIntegrityOfValuesException("Wrong Address. No coordinates found for the given address.");

            lock (AdminManager.BlMutex)
                _dal.Volunteer.Update(new
                    (doVolunteer.Id, doVolunteer.FullName,
                    doVolunteer.PhoneNumber, doVolunteer.Email,
                    doVolunteer.Password, doVolunteer.Job,
                    doVolunteer.Active, doVolunteer.Distance,
                    doVolunteer.VolAddress, (double)AddressCoordinate[0]!,
                    (double)AddressCoordinate[1]!, doVolunteer.MaxDistance));
        }

    }

    private static bool checkPass(string password)
    {
        bool isA = password.Any(char.IsUpper);
        bool isa = password.Any(char.IsLower);
        bool isdigit = password.Any(char.IsDigit);
        bool isSign = Regex.IsMatch(password, @"[!@#$%^&*;(),.?""':{}|<>]");
        bool isSame = (password.Distinct().Count() <= 4) ? false : true;

        bool isAbc = true;
        for (int i = 0; i < 6 && isAbc == true; i++)
        {
            char first = password[i];
            char second = password[i + 1];
            char third = password[i + 2];

            if (second == first + 1 && third == second + 1)
            {
                isAbc = false;
            }

            if (second == first - 1 && third == second - 1)
            {
                isAbc = false;
            }
        }

        return isA && isa && isa && isdigit && isSign && isSame && isAbc;
    }



    /// <summary>
    /// Check that the ID received is correct
    /// </summary>
    /// <param name="id">id for checking</param>
    /// <returns>A boolean variable that evaluates to true when the ID is correct.</returns>
    private static bool checkId(int id)
    {
        int sum = 0, i = 0;
        string idString = id.ToString();
        if (idString.Length == 8)
            i++;
        for (; i < 8; i++)
        {
            int digit = idString[i] - '0'; // char to number
            int multiplier = (i % 2 == 0) ? 1 : 2; // even or odd
            int product = digit * multiplier;
            sum += (product > 9) ? product - 9 : product; // sum
        }
        if ((id % 10 != ((10 - (sum % 10)) % 10)))
            return false;
        return true;
    }

    // <summary>
    /// Gets a volunteer from the database and returns the call he handle.
    /// </summary>
    /// <param name="id"> volunteer id</param>
    /// <param name="address">volunteer Address</param>
    /// <returns>returns the call the volunteer handle</returns>
    /// <exception cref="BO.BlNullPropertyException">Throws an exception when the  call does not exist </exception>
    internal static BO.CallInProgress? VolCall(int id, string? address)
    {
        DO.Assignment? currentAss;
        Func<DO.Assignment, bool> func = item => item.VolunteerId == id;
        lock (AdminManager.BlMutex)
            currentAss = _dal.Assignment.Read(func);
        if (currentAss != null && currentAss.EndTime == null)/*?? throw new BO.BlNullPropertyException($"Assignment does not exist");*/
        {
            DO.Call currentCall;
            lock (AdminManager.BlMutex)
                currentCall = _dal.Call.Read(currentAss.CallId) ?? throw new BO.BlNullPropertyException($"Call with ID: {currentAss.CallId} does not exist");

            return new()
            {
                Id = currentAss.Id,
                CallId = currentCall.Id,
                CallType = (BO.CallType)currentCall.CallType,
                Description = currentCall.Description,
                CallAddress = currentCall.CallAddress,
                OpenTime = currentCall.OpenTime,
                MaxCloseTime = currentCall.MaxTime,
                EntryTime = currentAss.InterTime,
                VolDistance = CalculateDis(address, currentCall.CallAddress),
                Status = (currentCall.MaxTime is null || (currentCall.MaxTime - AdminManager.Now) < _dal.Config.RiskRange) ? Status.InTreatment : Status.InRiskTreatment
            };
        }
        return null;
    }


    /// <summary>
    /// Gets a volunteer from the database and returns it as a data entity of type volunteer in list
    /// </summary>
    /// <param name="oldVolunteer">volunteer from the database</param>
    /// <returns> returns it as a data entity of type volunteer in list</returns>
    internal static IEnumerable<BO.VolunteerInList> ToVolunteerInList(IEnumerable<DO.Volunteer> oldVolunteer)
    {
        IEnumerable<DO.Assignment> oldAssignments;
        lock (AdminManager.BlMutex)
            oldAssignments = _dal.Assignment.ReadAll(null);
        List<BO.VolunteerInList> volunteerInLists = new List<BO.VolunteerInList>();
        foreach (DO.Volunteer item in oldVolunteer)
        {
            DO.Assignment? volunteerAssignment = oldAssignments.LastOrDefault(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTime == null));
            int? callId = volunteerAssignment != null ? volunteerAssignment.CallId : null;
            volunteerInLists.Add(new()
            {
                Id = item.Id,
                FullName = item.FullName,
                IsActive = item.Active,
                HandleCalls = oldAssignments.Count(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTreatment == DO.AssignmentEnum.TakenCare)),
                ExpiredCalls = oldAssignments.Count(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTreatment == DO.AssignmentEnum.CancelExpired)),
                CancelCalls = oldAssignments.Count(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTreatment == DO.AssignmentEnum.SelfCancel || assignment.EndTreatment == DO.AssignmentEnum.CancelAdmin)),
                CallId = callId != null ? callId : null,
                InTreatment = callId.HasValue ? (BO.CallInTreatment)_dal.Call.Read(callId.Value)!.CallType : BO.CallInTreatment.None
            });
        };
        return volunteerInLists.AsEnumerable();
    }

    /// <summary>
    /// Calculates the distance between the 2 given addresses
    /// </summary>
    /// <param name="volAddress"> volunteer address</param>
    /// <param name="callAddress"> call address</param>
    /// <returns></returns>
    public static double CalculateDis(string? volAddress, string callAddress)
    {
        if (volAddress is null || callAddress is null) return 0.0;
        Func<DO.Volunteer, bool> volPredicate = volunteer => volunteer.VolAddress == volAddress;
        Func<DO.Call, bool> callPredicate = call => call.CallAddress == callAddress;
        DO.Call call;
        DO.Volunteer vol;
        lock (AdminManager.BlMutex)
        {
            call = _dal.Call.Read(callPredicate)!;
            vol = _dal.Volunteer.Read(volPredicate)!;
        }


        double volLatitude = (double)vol.Latitude!;
        double volLongitude = (double)vol.Longitude!;
        double callLatitude = (double)call.Latitude!;
        double callLongitude = (double)call.Longitude!;


        const double R = 6371; // רדיוס כדור הארץ בקילומטרים

        // המרת מעלות לרדיאנים
        double ToRadians(double angle) => Math.PI * angle / 180.0;

        double phi1 = ToRadians(volLatitude);
        double phi2 = ToRadians(callLatitude);

        double deltaLat = ToRadians(callLatitude - volLatitude);
        double deltaLon = ToRadians(callLongitude - volLongitude);

        // חישוב המרחק בקירוב לשטח מישורי
        double x = deltaLon * Math.Cos((phi1 + phi2) / 2);
        double y = deltaLat;

        double distance = R * Math.Sqrt(x * x + y * y); // נוסחת פיתגורס
        return Math.Round(distance, 4);
    }

    #region stage 7
    internal static BO.Volunteer? Read(int id)
    {
        try
        {
            DO.Volunteer doVolunteer;
            IEnumerable<DO.Assignment> volAssignments;
            lock (AdminManager.BlMutex)
            {
                doVolunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist"); //can throw Ex
                Func<DO.Assignment, bool>? predicate = assignment => assignment.VolunteerId == id;
                volAssignments = _dal.Assignment.ReadAll(predicate);
            }
            return new()
            {
                Id = id,
                FullName = doVolunteer.FullName,
                PhoneNumber = doVolunteer.PhoneNumber,
                Email = doVolunteer.Email,
                Address = doVolunteer.VolAddress,
                Password = doVolunteer.Password,
                Latitude = doVolunteer.Latitude,
                Longitude = doVolunteer.Longitude,
                Job = (BO.Role)doVolunteer.Job,
                IsActive = doVolunteer.Active,
                MaxDis = doVolunteer.MaxDistance != null ? Math.Round(doVolunteer.MaxDistance.Value, 4) : null,
                Distance = (BO.DisType)doVolunteer.Distance,
                HandleCalls = volAssignments.Count(a => a.EndTreatment == DO.AssignmentEnum.TakenCare),
                CancelCalls = volAssignments.Count(a => (a.EndTreatment == DO.AssignmentEnum.CancelAdmin || a.EndTreatment == DO.AssignmentEnum.SelfCancel)),
                ExpiredCalls = volAssignments.Count(a => (a.EndTreatment == DO.AssignmentEnum.CancelExpired)),
                InCall = VolunteerManager.VolCall(id, doVolunteer.VolAddress)
            };
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    internal static IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive = null, BO.VolunteerData? sort = null, BO.CallInTreatment? filter = BO.CallInTreatment.None)
    {
        try
        {
            IEnumerable<DO.Volunteer> oldVolunteer;
            lock (AdminManager.BlMutex)
                oldVolunteer = _dal.Volunteer.ReadAll(null); //can throw Ex
            IEnumerable<BO.VolunteerInList> volunteerInList = VolunteerManager.ToVolunteerInList(oldVolunteer);

            if (isActive != null)
            {
                volunteerInList = volunteerInList.Where(volunteer => volunteer.IsActive == isActive);
            }
            volunteerInList = volunteerInList.Where(v => filter != BO.CallInTreatment.None ? v.InTreatment == filter : v.InTreatment != null);
            volunteerInList = sort == null ? volunteerInList.OrderBy(v => v.Id)
                : volunteerInList.OrderBy<BO.VolunteerInList, object>(v => sort switch
                {
                    BO.VolunteerData.Id => v.Id,
                    BO.VolunteerData.FullName => v.FullName,
                    BO.VolunteerData.IsActive => v.IsActive,
                    BO.VolunteerData.HandleCalls => v.HandleCalls,
                    BO.VolunteerData.CancelCalls => v.CancelCalls,
                    BO.VolunteerData.ExpiredCalls => v.ExpiredCalls,
                    BO.VolunteerData.CallId => v.CallId,
                    BO.VolunteerData.InTreatment => v.InTreatment,
                    _=> v.Id
                });
            return volunteerInList;
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }
    #endregion

    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;

    internal static void SimulateVolunteersActivity()
    {
        
        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";//?
        try
        {
            List<BO.VolunteerInList> volunteersList= ReadAll().ToList();
            //List<DO.Volunteer> volList;
            //volunteerInList= CallManager.ReadAll(/*volunteer => volunteer.Active == true*/).ToList();

            ////read all the volunteers from dal to list.
            //lock (AdminManager.BlMutex)
            //    volList = _dal.Volunteer.ReadAll(volunteer => volunteer.Active == true).ToList();

            ////if there is any volunteer
            //if (volList.Count()!=0)
            //{
            //    //Converts volunteers from DAL to BO
            //    List<BO.VolunteerInList> volunteersList = ToVolunteerInList(volList).ToList();

                
                foreach (BO.VolunteerInList volunteer in volunteersList)
                {
                BO.Volunteer vol = Read(volunteer.Id)!;

                //lock (AdminManager.BlMutex)
                if (vol.InCall == null)
                    {
                        if (s_rand.NextDouble() < 0.2)
                            continue;
                        List<BO.OpenCallInList> opensCalls = CallManager.GetOpenedCalls(volunteer.Id, null, null).ToList();
                        if (opensCalls.Count != 0)
                        {
                            BO.OpenCallInList chosenCall = opensCalls[s_rand.Next(opensCalls.Count)];
                            CallManager.CallToTreatment(volunteer.Id, chosenCall.Id);
                        }
                    }
                    else
                    {
                        //BO.Volunteer vol = Read(volunteer.Id)!;
                        if (vol.InCall!.EntryTime < AdminManager.Now.AddDays(-7))
                        {
                            CallManager.GetAssignmentToEnd(vol.Id, vol.InCall.CallId);
                        }
                        else
                        {
                            if (s_rand.NextDouble() < 0.2)
                            {
                                CallManager.GetAssignmentToCancel(vol.Id, vol.InCall.CallId);
                            }
                        }
                    }
                }

    }
        catch(BLTemporaryNotAvailableException)
        {
            
        }
        catch
        {

        }
    }

}


