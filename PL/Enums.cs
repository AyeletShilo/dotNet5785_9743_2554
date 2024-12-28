using System.Collections;

namespace PL;

internal class FilterByCall : IEnumerable //?
{
    static readonly IEnumerable<BO.CallInTreatment> s_enums =
        (Enum.GetValues(typeof(BO.CallInTreatment)) as IEnumerable<BO.CallInTreatment>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}