namespace PL;

internal class VolunteerDataToPickUp : IEnumerable //?
{
    static readonly IEnumerable s_enums =
        Enum.GetValues(typeof(BO.VolunteerData)).Cast<BO.VolunteerData>();

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

