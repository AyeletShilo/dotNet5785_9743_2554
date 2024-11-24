using DO;
namespace DalApi;

public interface ICrud<T> where T : class
{
    void Create(T item); //Creates new entity object in DAL
    T? Read(int id); //Reads entity object by its ID 
    T? Read(Func<T, bool> filter); // stage 2 - You will receive a pointer to a boolean function, which will operate on one of the members of type T and return the first object on which the function returns True.
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // stage 2 -Return the list of all objects in the list for which the function returns True.
    void Update(T item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}