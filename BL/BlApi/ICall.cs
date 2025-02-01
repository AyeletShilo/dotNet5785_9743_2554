using BO;

namespace BlApi;

public interface ICall : IObservable //stage 5
{
    void Create(Call callToAdd);
    IEnumerable<CallInList> ReadAll(CallData? filter = null, CallData? sort = null, Object? value = null);
    Call Read(int callId);
    void Update(Call callToUpdate);
    void Delete(int callId);
    int[] HowManyCalls();
    IEnumerable<ClosedCallInList> GetClosedCalls(int volId, CallType? filter = null, CloseCallData? sort = null);
    IEnumerable<OpenCallInList> GetOpenedCalls(int volId, CallType? filter = null, OpenCallData? sort = null);
    void UpdateEndTreatment(int volId, int assignmentId);
    void UpdateCancelTreatment(int volId, int assignmentId);
    void CallToTreatment(int volId, int callId);
    void GetAssignmentToEnd(int volId, int callId);
    void GetAssignmentToCancel(int volId, int callId);
    Task<double?[]> CheckedAddress(string Address);
}
