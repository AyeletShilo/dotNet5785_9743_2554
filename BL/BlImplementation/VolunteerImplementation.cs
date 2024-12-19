namespace BlImplementation;
using System.Collections.Generic;
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
        try
        {
            VolunteerManager.CheckFormat(boVolunteer);
            VolunteerManager.CheckLogic(boVolunteer);

            double[] AddressCoordinate = CallManager.GetCoordinates(boVolunteer.Address);

            DO.Volunteer doVolunteer =
              new(boVolunteer.Id, boVolunteer.FullName, boVolunteer.PhoneNumber, boVolunteer.Email, (DO.Role)boVolunteer.Job, boVolunteer.IsActive, (DO.RangeType)boVolunteer.Distance,
              boVolunteer.Address, AddressCoordinate[0], AddressCoordinate[1], boVolunteer.MaxDis);


            _dal.Volunteer.Create(doVolunteer); //can throw Ex
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
        var idVolunteer = Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");
        try
        {
            if (idVolunteer.HandleCalls != 0 || idVolunteer.InCall != null)
                throw new BO.BlCannotBeDeletedException($"Volunteer with ID={id}cannot be deleted");

            _dal.Volunteer.Delete(id); //can throw Ex
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
    /// returns volunteer gob accordding his Id
    /// </summary>
    /// <param name="id">volunteer Id</param>
    /// <returns>volunteer gob</returns>
    /// <exception cref="BO.BlDoesNotExistException">Throws an exception when the volunteer you want does not exist in the database</exception>
    public BO.Role GetMyJob(int id)
    {
        BO.Volunteer result = Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");
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
            DO.Volunteer doVolunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist"); //can throw Ex
            Func<DO.Assignment, bool>? predicate = assignment => assignment.VolunteerId == id;
            IEnumerable<DO.Assignment> volAssignments = _dal.Assignment.ReadAll(predicate);
            return new()
            {
                Id = id,
                FullName = doVolunteer.FullName,
                PhoneNumber = doVolunteer.PhoneNumber,
                Email = doVolunteer.Email,
                Address = doVolunteer.VolAddress,
                Latitude = doVolunteer.Latitude,
                Longitude = doVolunteer.Longitude,
                Job = (BO.Role)doVolunteer.Job,
                IsActive = doVolunteer.Active,
                MaxDis = doVolunteer.MaxDistance,
                Distance = (BO.DisType)doVolunteer.Distance,
                HandleCalls = volAssignments.Count(a => a.EndTreatment == DO.AssignmentEnum.TakenCare),
                CancelCalls = volAssignments.Count(a => (a.EndTreatment == DO.AssignmentEnum.CancelAdmin || a.EndTreatment == DO.AssignmentEnum.SelfCancel)),
                ExpiredCalls = volAssignments.Count(a => (a.EndTreatment == DO.AssignmentEnum.CancelExpired)),
                InCall = VolunteerManager.VolCall(id, doVolunteer.VolAddress) //הכתובת של המתנדב יכולה להיהות NULL??
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
    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive = null, BO.VolunteerData? sort = null)
    {
        try
        {
            IEnumerable<DO.Volunteer> OldVolunteer = _dal.Volunteer.ReadAll(null); //can throw Ex
            IEnumerable<BO.VolunteerInList> volunteerInLists = VolunteerManager.ToVolunteerInList(OldVolunteer);

            if (isActive != null)
            {
                volunteerInLists = volunteerInLists.Where(volunteer => volunteer.IsActive == isActive);
            }
            if (sort == null)
                return volunteerInLists.OrderBy(v => v.Id);
            else
            {
                string sortProperty = VolunteerManager.GetPropertyName(sort.Value);
                volunteerInLists = volunteerInLists.OrderBy(c => c.GetType().GetProperty(sortProperty)?.GetValue(c));
                return volunteerInLists;
            }
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
    /// <exception cref="BO.BlCantUpdateException">Throws an exception when the vilunteer how want to update not allowed to do this </exception>
    /// <exception cref="BO.BlDoesNotExistException">Throws an exception when the vilunteer you want to update does not exist in the database</exception>
    public void Update(int id, BO.Volunteer volToUpdate)
    {
        //BO.Volunteer isManager = Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist"); //חריגה לשכבה מעל
        try
        {
            if (id == volToUpdate.Id || GetMyJob(id) == BO.Role.Manager)
            {
                VolunteerManager.CheckFormat(volToUpdate); //can throw Ex
                VolunteerManager.CheckLogic(volToUpdate); //can throw Ex

                double[] AddressCordinate = CallManager.GetCoordinates(volToUpdate.Address); //לסדר חריגות

                DO.Volunteer? oldVolunteer = _dal.Volunteer.Read(volToUpdate.Id);
                if ((oldVolunteer.Job != (DO.Role)volToUpdate.Job) && GetMyJob(id) != BO.Role.Manager)
                    throw new BO.BlCantUpdateException($"Volunteer with ID:{oldVolunteer.Id} not allowed to update this");

                DO.Volunteer doVolunteer = new(volToUpdate.Id, volToUpdate.FullName, volToUpdate.PhoneNumber, volToUpdate.Email, (DO.Role)volToUpdate.Job, volToUpdate.IsActive, (DO.RangeType)volToUpdate.Distance,
             volToUpdate.Address, AddressCordinate[0], AddressCordinate[1], volToUpdate.MaxDis);

                _dal.Volunteer.Update(doVolunteer);//can throw Ex
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
}