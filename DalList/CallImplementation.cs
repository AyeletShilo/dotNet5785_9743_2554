
namespace Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Net.Mail;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {

        int NewId = Config.nextCallId;
        Call newItem = new(NewId, item.CallType, item.CallAddress, item.Latitude, item.Longitude, item.OpenTime, item.Description, item.MaxTime);
        DataSource.Calls?.Add(newItem);
    }

    public void Delete(int id)
    {
        Call item = DataSource.Calls?.Find(x => x.Id == id) ?? throw new DalDoesNotExistException($"Object of type Call with ID={id} does not exists");
        bool? x = DataSource.Calls?.Remove(item);
    }

    public void DeleteAll()
    {
        DataSource.Calls?.Clear();
    }

    public Call? Read(int id)
    {
        //Call? item = DataSource.Calls?.Find(x => x.Id == id); //stage 1
        Call? item = DataSource.Calls?.FirstOrDefault(x => x.Id == id); //stage 2
        return item;
    }

    public Call? Read(Func<Call, bool> filter) //stage 2
    {
        Call? item = DataSource.Calls?.FirstOrDefault(filter);
        return item;
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) //stage 2
       => filter == null
            ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);

    public void Update(Call item)
    {
        Delete(item.Id);
        DataSource.Calls?.Add(item);
    }
}