using System.Collections;

namespace PL;

internal class FilterByCall : IEnumerable //?
{
    static readonly IEnumerable<BO.CallInTreatment> s_enums =
        (Enum.GetValues(typeof(BO.CallInTreatment)) as IEnumerable<BO.CallInTreatment>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class RoleOpt : IEnumerable //?
{
    static readonly IEnumerable<BO.Role> s_enums =
        (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}