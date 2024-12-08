
using Helpers;

namespace BO;

public enum Role
{
    Manager,
    Donater
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
    CancelExpired
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
    hour,
    day,
    month,
    year
}

public enum VolunteerData
{
    Id,
    PullName,
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
    FullAddress,
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
    FullAddress,
    OpenTime,
    MaxCloseTime,
    VolDistance
}