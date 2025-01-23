
using System.Runtime.CompilerServices;

namespace Dal;

internal class Config
{
    internal const int startNextCallId = 1;
    private static int NextCallId = startNextCallId;

    internal static int nextCallId 
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => NextCallId++; 
    }


    internal const int startNextAssignmentId = 1;
    private static int NextAssignmentId = startNextAssignmentId;
    internal static int nextAssignmentId 
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => NextAssignmentId++; 
    }

    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static TimeSpan RiskRange { get; set; }

    // private TimeSpan RiskRange= TimeSpan.Zero;

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        NextCallId = startNextCallId;
        NextAssignmentId = startNextAssignmentId;
        Clock = DateTime.Now;
        RiskRange= TimeSpan.FromDays(2);
    }

}
