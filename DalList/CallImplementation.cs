
namespace Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Net.Mail;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {

        int NewId = DataSource.Config();//?//
        Call newItem = new(NewId, item.callType,item.callAddress,item.Latitude,item.Longitude,item.OpenTime,item.description,item.MaxTime);
        DataSource.Calls?.Add(newItem);
    }

    public void Delete(int id)
    {
        Call? item = DataSource.Calls?.Find(x => x.Id == id);
        if (item != null)
        {
            bool? x = DataSource.Calls?.Remove(item);
        }
        else
            throw new Exception("\"Object of type Call with such ID does not exist\""/*?*/);
    }

    public void DeleteAll()
    {
        DataSource.Calls?.Clear();
    }

        public Call? Read(int id)
    {

        Call? item = DataSource.Calls?.Find(x => x.Id == id);
        return item;
    }

    public List<Call> ReadAll()
    {
        List<Call>? copyList = new();
        if (DataSource.Calls != null)
            copyList.AddRange(DataSource.Calls);
        return copyList;
    }

    public void Update(Call item)
    {
        Delete(item.Id);
        DataSource.Calls?.Add(item);
    }
}
