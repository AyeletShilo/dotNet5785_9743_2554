
namespace Dal;

internal class Config
{
    internal const int startNextCallId = 1;
    private static int NextCallId = startNextCallId;
    internal static int nextCallId { get => NextCallId++; }


    internal const int startNextAssignmentId = 1;
    private static int NextAssignmentId = startNextAssignmentId;
    internal static int nextAssignmentId { get => NextAssignmentId++; }

    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal TimeSpan riskRange { get; set; }

   // private TimeSpan RiskRange= TimeSpan.Zero;
    internal static void Reset()
    {
        NextCallId = startNextCallId;
        NextAssignmentId = startNextAssignmentId;
        Clock = DateTime.Now;
    }

}
