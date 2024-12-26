using System.Collections;

namespace PL;

internal class VolunteerDataToPickUp : IEnumerable //?
{
    static readonly IEnumerable<BO.VolunteerData> s_enums =
        (Enum.GetValues(typeof(BO.VolunteerData)) as IEnumerable<BO.VolunteerData>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}