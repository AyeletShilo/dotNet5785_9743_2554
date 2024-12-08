using BO;

namespace BlApi;

public interface ICall
{
    void Create(Call CallToAdd);
    IEnumerable<CallInList> ReadAll(CallData? filter = null, CallData? sort = null, Object? value = null);
    Call Read(int id);
    void Update(Call CallToUpdate);
    void Delete(int id);
    CallStatus[] HowManyCalls();
    IEnumerable<ClosedCallInList> GetClosedCalls(int id, CallType? filter = null, CloseCallData? sort = null);
    IEnumerable<OpenCallInList> GetOpenedCalls(int id, CallType? filter = null, OpenCallData? sort = null);
    void UpdateEndTreatment(int id, int assignmentId);
    void UpdateCancelTreatment(int id, int assignmentId);
    void CallToTreatment(int id, int assignmentId);

}
