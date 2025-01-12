using System.Collections;

namespace PL;

internal class FilterByCall : IEnumerable 
{
    static readonly IEnumerable<BO.CallInTreatment> s_enums =
        (Enum.GetValues(typeof(BO.CallInTreatment)) as IEnumerable<BO.CallInTreatment>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class SortVolunteer : IEnumerable 
{
    static readonly IEnumerable<BO.VolunteerData> s_enums =
        (Enum.GetValues(typeof(BO.VolunteerData)) as IEnumerable<BO.VolunteerData>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class RoleOpt : IEnumerable //?
{
    static readonly IEnumerable<BO.Role> s_enums =
        (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class FilterCall : IEnumerable 
{
    static readonly IEnumerable<BO.CallListStatus> s_enums =
        (Enum.GetValues(typeof(BO.CallListStatus)) as IEnumerable<BO.CallListStatus>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class SortCall : IEnumerable
{
    static readonly IEnumerable<BO.CallData> s_enums =
        (Enum.GetValues(typeof(BO.CallData)) as IEnumerable<BO.CallData>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class StatusCall : IEnumerable 
{
    static readonly IEnumerable<BO.CallStatus> s_enums =
        (Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class TypeCall : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums =
        (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class CloseCallData : IEnumerable
{
    static readonly IEnumerable<BO.CloseCallData> s_enums =
        (Enum.GetValues(typeof(BO.CloseCallData)) as IEnumerable<BO.CloseCallData>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class OpenCallData : IEnumerable
{
    static readonly IEnumerable<BO.OpenCallData> s_enums =
        (Enum.GetValues(typeof(BO.OpenCallData)) as IEnumerable<BO.OpenCallData>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}