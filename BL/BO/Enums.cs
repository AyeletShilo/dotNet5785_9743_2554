
using Helpers;

namespace BO;

public enum Role
{
    Volunteer,
    Manager
}

public enum DisType
{
    Air,
    Walking,
    Car
}

public enum CallType
{
    Shopping,
    Cleaning,
    Repairing,
    TechnologyHelp,
    Talking
}


public enum Status
{
    InTreatment,
    InRiskTreatment
}

public enum CallInTreatment
{
    None,
    Shopping,
    Cleaning,
    Repairing,
    TechnologyHelp,
    Talking
}

public enum EndTreatment

{
    TakenCare,
    SelfCancel,
    CancelAdmin,
    CancelExpired,
}

public enum CallStatus
{
    Opened,
    InTreatment,
    Closed,
    Expired,
    OpenInRisk
}

public enum CallListStatus
{
    Opened,
    InTreatment,
    Closed,
    Expired,
    OpenInRisk,
    InTreatmentInRisk
}

public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}

public enum VolunteerData
{
    Id,
    FullName,
    IsActive,
    HandleCalls,
    CancelCalls,
    ExpiredCalls,
    CallId,
    InTreatment
}

public enum CallData
{
    Id,
    CallId,
    CallType,
    OpenTime,
    LeftTime,
    LastVolunteer,
    CompletionTime,
    Status,
    TotalAssignments
}

public enum CloseCallData
{
    Id,
    CallType,
    CallAddress,
    OpenTime,
    InterTime,
    CloseTime,
    EndTreatment
}

public enum OpenCallData
{
    Id,
    CallType,
    Description,
    CallAddress,
    OpenTime,
    MaxCloseTime,
    VolDistance
}