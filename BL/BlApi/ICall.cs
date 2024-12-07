using BO;

namespace BlApi;

public interface ICall
{
    void Create(Call CallToAdd);
    IEnumerable<CallInList> ReadAll(CallListStatus? filter = null, CallListStatus? sort = null, Object? value = null);
    Call Read(int id);
    void Updete(Call CallToUpdate);
    void Delete(int id);
    CallStatus[] HowManyCalls();
    IEnumerable<ClosedCallInList> GetClosedCalls(int id, CallType? sort = null/*To add Enum*/);
    IEnumerable<OpenCallInList> GetOpenedCalls(int id, CallType? sort = null/*To add Enum*/);
    void UpdateEndTreatment(int id, int assignmentId);
    void UpdateCancelTreatment(int id, int assignmentId);
    void CallToTreatment(int id, int assignmentId);

}
