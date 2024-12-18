namespace BlImplementation;
using System.Collections.Generic;
using Helpers;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    public void Create(BO.Volunteer boVolunteer)
    {
        try
        {
            VolunteerManager.CheckFormat(boVolunteer); //חריגה נזרקת
            VolunteerManager.CheckLogic(boVolunteer); //חריגה נזרקת

            double[] AddressCoordinate = CallManager.GetCoordinates(boVolunteer.Address); //לסדר חריגות

            DO.Volunteer doVolunteer =
              new(boVolunteer.Id, boVolunteer.FullName, boVolunteer.PhoneNumber, boVolunteer.Email, (DO.Role)boVolunteer.Job, boVolunteer.IsActive, (DO.RangeType)boVolunteer.Distance,
              boVolunteer.Address, AddressCoordinate[0], AddressCoordinate[1], boVolunteer.MaxDis);


            _dal.Volunteer.Create(doVolunteer); //חריגה תזרק משכבת הנתונים
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BlDoesAlreadyExistException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        catch (BO.BlIntegrityOfValuesException ex1)
        {
            throw ex1;
        }
    }

    public void Delete(int id)
    {
        var idVolunteer = Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist"); //חריגה של NULL או של איבר שלא קיים? והאם זה ימנע מDELETET לשלוח חריגה?
        try
        {
            if (idVolunteer.HandleCalls != 0 || idVolunteer.InCall != null)
                throw new BO.BlCannotBeDeletedException($"Volunteer with ID={id}cannot be deleted"); //החריגה תזרק לשכבה מעל

            _dal.Volunteer.Delete(id); //חריגה תזרק משכבת הנתונים
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist", ex);
        }
    }

    public BO.Role GetMyJob(int id)
    {
        BO.Volunteer result = Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");
        return result.Job;
    }

    public BO.Volunteer? Read(int id)
    {
        DO.Volunteer doVolunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
        Func<DO.Assignment, bool>? predicate = assignment => assignment.VolunteerId == id;
        IEnumerable<DO.Assignment> VolAssigments = _dal.Assignment.ReadAll(predicate);
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
            HandleCalls = VolAssigments.Count(a => a.EndTreatment == DO.AssignmentEnum.TakenCare),
            CancelCalls = VolAssigments.Count(a => (a.EndTreatment == DO.AssignmentEnum.CancelAdmin || a.EndTreatment == DO.AssignmentEnum.SelfCancel)),
            ExpiredCalls = VolAssigments.Count(a => (a.EndTreatment == DO.AssignmentEnum.CancelExpired)),
            InCall = VolunteerManager.VolCall(id, doVolunteer.VolAddress) //הכתובת של המתנדב יכולה להיהות NULL??
        };
    }

    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive = null, BO.VolunteerData? sort = null)
    {
        IEnumerable<DO.Volunteer> OldVolunteer = _dal.Volunteer.ReadAll(null);
        IEnumerable<BO.VolunteerInList> volunteerInLists = VolunteerManager.ToVolunteerInList(OldVolunteer);

        if (isActive != null)
        {
            //Func<DO.Volunteer, bool>? predicate = volunteer => volunteer.Active == true;
            volunteerInLists = volunteerInLists.Where(volunteer => volunteer.IsActive == isActive);
        }
        if (sort == null)
            return volunteerInLists.OrderBy(v => v.Id);
        else
        {
            //string sortParameter = sort.ToString();
            //return volunteerInLists.OrderBy(v => v.sort); 
            string sortProperty = VolunteerManager.GetPropertyName(sort.Value);
            volunteerInLists = volunteerInLists.OrderBy(c => c.GetType().GetProperty(sortProperty)?.GetValue(c));
            return volunteerInLists;
        }
    }


    public void Update(int id, BO.Volunteer volToUpdate)
    {
        BO.Volunteer isManager = Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist"); //חריגה לשכבה מעל
        try
        {
            if (id == volToUpdate.Id || isManager.Job == BO.Role.Manager)
            {
                VolunteerManager.CheckFormat(volToUpdate); //תיזרק חריגה
                VolunteerManager.CheckLogic(volToUpdate); //תיזרק חריגה

                double[] AddressCordinate = CallManager.GetCoordinates(volToUpdate.Address); //לסדר חריגות

                DO.Volunteer? oldVolunteer = _dal.Volunteer.Read(volToUpdate.Id);
                if ((oldVolunteer.Job != (DO.Role)volToUpdate.Job) && isManager.Job != BO.Role.Manager)
                    throw new BO.BlCantUpdateException($"Volunteer with ID:{oldVolunteer.Id} not allowed to update this");

                DO.Volunteer doVolunteer = new(volToUpdate.Id, volToUpdate.FullName, volToUpdate.PhoneNumber, volToUpdate.Email, (DO.Role)volToUpdate.Job, volToUpdate.IsActive, (DO.RangeType)volToUpdate.Distance,
             volToUpdate.Address, AddressCordinate[0], AddressCordinate[1], volToUpdate.MaxDis);

                _dal.Volunteer.Update(doVolunteer);//שאלה על החריגה של הNULL
            }

            else
                throw new BO.BlCantUpdateException("You are not allowed to update this");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volToUpdate.Id} does Not exists", ex);
        }
    }
}