namespace BlImplementation;

using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Creates a new volunteer in the database
    /// </summary>
    /// <param name="boVolunteer">Values ​​for new call</param>
    /// <exception cref="BO.BlDoesAlreadyExistException">Throws an exception when the call you want to create already exists in the database.</exception>
    public void Create(BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        try
        {
            VolunteerManager.CheckFormat(boVolunteer);
            VolunteerManager.CheckLogic(boVolunteer);

            double[] AddressCoordinate;
            if (boVolunteer.Address != "" && boVolunteer.Address != null)
                AddressCoordinate = CallManager.GetCoordinates(boVolunteer.Address);
            else
            {
                AddressCoordinate = new double[2];
                AddressCoordinate[0] = 0;
                AddressCoordinate[1] = 0;
            }

            DO.Volunteer doVolunteer =
              new(boVolunteer.Id, boVolunteer.FullName, boVolunteer.PhoneNumber, boVolunteer.Email, boVolunteer.Password, (DO.Role)boVolunteer.Job, boVolunteer.IsActive, (DO.RangeType)boVolunteer.Distance,
              boVolunteer.Address, AddressCoordinate[0], AddressCoordinate[1], boVolunteer.MaxDis);

            lock (AdminManager.BlMutex)
                _dal.Volunteer.Create(doVolunteer); //can throw Ex
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5    
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BlDoesAlreadyExistException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    /// <summary>
    /// Delete volunteer from database
    /// </summary>
    /// <param name="id">The ID number of the volunteer how want to delete</param>
    /// <exception cref="BO.BlDoesNotExistException">Throws an exception when the call you want to delete does not exist in the database</exception>
    /// <exception cref="BO.BlCannotBeDeletedException">An exception is thrown when the volunteer cannot be deleted.</exception>
    public void Delete(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        var idVolunteer = Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");
        try
        {
            if (idVolunteer.HandleCalls != 0 || idVolunteer.InCall != null)
                throw new BO.BlCannotBeDeletedException($"Volunteer with ID={id}cannot be deleted");
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Delete(id); //can throw Ex
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5    
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist", ex);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    /// <summary>
    /// returns volunteer gob according his Id
    /// </summary>
    /// <param name="id">volunteer Id</param>
    /// <returns>volunteer gob</returns>
    /// <exception cref="BO.BlDoesNotExistException">Throws an exception when the volunteer you want does not exist in the database</exception>
    public BO.Role GetMyJob(int id, string? password)
    {
        BO.Volunteer result = Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.");
        if (password != result.Password)
            throw new BO.BlPasswordException($"Volunteer password is incorrect");
        return result.Job;
    }

    /// <summary>
    /// Return call values ​​from the database 
    /// </summary>
    /// <param name="id">call ID</param>
    /// <returns>call values ​​from the database </returns>
    /// <exception cref="BO.BlDoesNotExistException">Throws an exception when the volunteer you want does not exist in the database</exception>
    public BO.Volunteer? Read(int id)
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

    /// <summary>
    /// Returns a list of volunteers filtered and sorted according to the parameters received.
    /// </summary>
    /// <param name="isActive">Parameter for filtering the list</param>
    /// <param name="sort">Parameter for sorting the list</param>
    /// <returns>list of volunteers</returns>
    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive = null, BO.VolunteerData? sort = null, BO.CallInTreatment? filter = BO.CallInTreatment.None)
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
                });
            return volunteerInList;
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    /// <summary>
    /// Updating an existing volunteer
    /// </summary>
    /// <param name="id">volunteer Id</param>
    /// <param name="volToUpdate">Updated values ​​of the volunteer</param>
    /// <exception cref="BO.BlCantUpdateException">Throws an exception when the volunteer how want to update not allowed to do this </exception>
    /// <exception cref="BO.BlDoesNotExistException">Throws an exception when the volunteer you want to update does not exist in the database</exception>
    public void Update(int id, BO.Volunteer volToUpdate)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        try
        {
            DO.Volunteer? updateVol;
            DO.Volunteer? oldVolunteer;
            lock (AdminManager.BlMutex)
                updateVol = _dal.Volunteer.Read(id)!;
            if (id == volToUpdate.Id || GetMyJob(id, updateVol.Password) == BO.Role.Manager)
            {
                VolunteerManager.CheckFormat(volToUpdate); //can throw Ex
                VolunteerManager.CheckLogic(volToUpdate); //can throw Ex

                double[] addressCordinate;
                if (volToUpdate.Address != "" && volToUpdate.Address != null)
                    addressCordinate = CallManager.GetCoordinates(volToUpdate.Address);
                else
                {
                    addressCordinate = new double[2];
                    addressCordinate[0] = 0;
                    addressCordinate[1] = 0;
                }

                lock (AdminManager.BlMutex)
                   oldVolunteer = _dal.Volunteer.Read(volToUpdate.Id);
                if ((oldVolunteer.Job != (DO.Role)volToUpdate.Job) && GetMyJob(id, updateVol.Password) != BO.Role.Manager)
                    throw new BO.BlCantUpdateException($"Volunteer with ID:{oldVolunteer.Id} not allowed to update this");

                DO.Volunteer doVolunteer = new(volToUpdate.Id, volToUpdate.FullName, volToUpdate.PhoneNumber, volToUpdate.Email, volToUpdate.Password, (DO.Role)volToUpdate.Job, volToUpdate.IsActive, (DO.RangeType)volToUpdate.Distance,
             volToUpdate.Address, addressCordinate[0], addressCordinate[1], volToUpdate.MaxDis);
                lock (AdminManager.BlMutex)
                    _dal.Volunteer.Update(doVolunteer);//can throw Ex
                VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id);  //stage 5
                VolunteerManager.Observers.NotifyListUpdated();  //stage 5

            }

            else
                throw new BO.BlCantUpdateException("You are not allowed to update this");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volToUpdate.Id} does Not exists", ex);
        }

        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    public void UpdateAddress(BO.Volunteer vol, string? newAdd)
    {
        BO.Volunteer copyVol = vol;
        if (newAdd == "")
            copyVol.Address = newAdd;
        Update(vol.Id, copyVol);
    }

    public double? Dis(string? volAddress, string callAddress)
    {
        if (volAddress == null)
            return null;
        return VolunteerManager.CalculateDis(volAddress, callAddress);
    }

    public string MakeStrongPassword()
    {
        Random s_rand = new();
        string password = s_rand.Next(10000, 100000).ToString();
        char A = (char)(s_rand.Next(65, 90));
        char a = (char)(s_rand.Next(97, 122));
        int index = s_rand.Next(0, 20);
        string sign = "!@#$%^*(),.?\"':{}|<>";
        password= password + sign[index] + A + a;
        return new string(password.OrderBy(x => s_rand.Next()).ToArray());
    }
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

}