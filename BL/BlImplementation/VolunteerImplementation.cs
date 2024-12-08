 namespace BlImplementation;
using BlApi;
//using BO;
using System.Collections.Generic;
using Helpers;


internal class VolunteerImplementation : IVolunteer
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
        throw new NotImplementedException();
    }

    public BO.Role GetMyJob(int id)
    {

    }

    public BO.Volunteer? Read(int id)
    {
        var doVolunteer = _dal.Volunteer.Read(id) ??
throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
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
            Distance = doVolunteer.Distance
        };

    }

    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive = null, BO.VolunteerData? sort = null)
    {
        IEnumerable<BO.VolunteerInList> volunteerInLists;
        if (isActive == null)
            volunteerInLists = (IEnumerable<BO.VolunteerInList>)_dal.Volunteer.ReadAll(null);
        else
        {
            Func<DO.Volunteer, bool>? predicate = volunteer => volunteer.Active == true;
            volunteerInLists = (IEnumerable<BO.VolunteerInList>)_dal.Volunteer.ReadAll(predicate);
        }
        if (sort == null)
            return volunteerInLists.OrderBy(v => v.Id);
        else
        {
            string sortParameter = sort.ToString();
            return volunteerInLists.OrderBy(v => v.(BO.VolunteerData)sort);
        }

    }

    public void Update(int id, BO.Volunteer volToUpdate)
    {
        if(id== volToUpdate.Id || Read(volToUpdate.Id).Job == BO.Role.Manager)
        {
            
           VolunteerManager.CheckLogic(volToUpdate);
           VolunteerManager.CheckFormat(volToUpdate);
            
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
    }
}
