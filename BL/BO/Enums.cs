
namespace BO;

public enum Job
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
   