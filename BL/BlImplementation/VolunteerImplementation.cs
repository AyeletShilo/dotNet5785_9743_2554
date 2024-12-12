namespace BlImplementation;
using BlApi;
//using BO;
using System.Collections.Generic;
using Helpers;
using DalApi;


internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    public void Create(BO.Volunteer boVolunteer)
    {
        VolunteerManager.CheckLogic(boVolunteer);
        VolunteerManager.CheckFormat(boVolunteer);

        DO.Volunteer doVolunteer =
          new(boVolunteer.Id, boVolunteer.FullName, boVolunteer.PhoneNumber, boVolunteer.Email, (DO.Role)(boVolunteer.Job), boVolunteer.IsActive, (DO.RangeType)boVolunteer.Distance,
          boVolunteer.Address, boVolunteer.Latitude, boVolunteer.Longitude, boVolunteer.MaxDis);
        try
        {
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }

    public void Delete(int id)
    {
        var idVolunteer = Read(id);
        try
        {
            if (idVolunteer.HandleCalls != 0 || idVolunteer.InCall != null)
                throw new BO.BLCannotBeDeletedException($"Volunteer with ID={id}cannot be deleted");

            _dal.Volunteer.Delete(id);
        }
        catch (BO.BLCannotBeDeletedException ex)
        {
            throw new BO.BLCannotBeDeletedException($"Volunteer with ID={id} cannot be deleted", ex);
        }
        catch (BO.BLCannotBeDeletedException ex)
        {
            throw new BO.BLCannotBeDeletedException($"Volunteer with ID={id} does Not exist", ex);
        }

    }

    public BO.Role GetMyJob(int id)
    {
        return Read(id).Job;
    }

    public BO.Volunteer? Read(int id)
    {
        var doVolunteer = _dal.Volunteer.Read(id) ?? throw new BO.BLCannotBeDeletedException($"Volunteer with ID={id} does Not exist");
        return new()
        {
            Id = id,
            FullName = doVolunteer.Name,
            PhoneNumber = doVolunteer.PhoneNumber,
            Email = doVolunteer.Email,
            Address = doVolunteer.Address,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            Job = doVolunteer.Job,
            IsActive = doVolunteer.IsActive,
            MaxDis = doVolunteer.MaxDistance,
            Distance = doVolunteer.Distance,
            InCall = VolunteerManager.VolCall(id, doVolunteer.Address)
        };

    }

    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive = null, BO.VolunteerData? sort = null)
    {
        IEnumerable<DO.Volunteer> OldVolunteer = _dal.Volunteer.ReadAll(null);
        IEnumerable<BO.VolunteerInList> volunteerInLists = VolunteerManager.ToVolunteerInList(OldVolunteer);

        if (isActive != null)
        {
            //Func<DO.Volunteer, bool>? predicate = volunteer => volunteer.Active == true;
            volunteerInLists = volunteerInLists.Where(volunteer => volunteer.IsActive == true);
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
        BO.Volunteer isManager = Read(volToUpdate.Id);
        try
        {
            if (id == volToUpdate.Id || isManager.Job == BO.Role.Manager)
            {

                VolunteerManager.CheckLogic(volToUpdate);
                VolunteerManager.CheckFormat(volToUpdate);

                var oldVolunteer = _dal.Volunteer.Read(volToUpdate.Id);
                if ((oldVolunteer.Job != (DO.Role)volToUpdate.Job) && isManager.Job != BO.Role.Manager)
                    throw new BO.CantUpdateException("Danater cant update this");

                DO.Volunteer doVolunteer = new(volToUpdate.Id, volToUpdate.FullName, volToUpdate.PhoneNumber, volToUpdate.Email, (DO.Role)(volToUpdate.Job), volToUpdate.IsActive, (DO.RangeType)volToUpdate.Distance,
             volToUpdate.Address, volToUpdate.Latitude, volToUpdate.Longitude, volToUpdate.MaxDis);
                try
                {
                    _dal.Volunteer.Update(doVolunteer);
                }
                catch (DO.BlDoesNotExistException ex)
                {
                    throw new BO.BlDoesNotExistException($"Volunteer with ID={volToUpdate.Id} does Not exists", ex);
                }
            }
            else
                throw new BO.CantUpdateException("Danater cant update this");
        }
        catch (BO.CantUpdateException ex)
        {
            throw new BO.CantUpdateException("Danater cant update this", ex);
        }
    }
}
        
