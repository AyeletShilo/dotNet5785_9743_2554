
using System.Data;

namespace DO;

/// <summary>
/// Type of distance: aerial distance, walking distance, driving distance
/// </summary>
public enum RangeType
{
    Air,
    Walking,
    Car
}

/// <summary>
/// Role of the volunteer
/// </summary>
public enum Role
{
    Manager,
    Donater
}

/// <summary>
/// Types of calls received in the organization
/// </summary>
public enum TypeOfCall
{
    shopping,
    cleaning,
    repairing,
    technologyHelp,
    talking
}

/// <summary>
/// <param name="EndTreatment">The manner in which the current call was handled by the current volunteer.
/// </summary>
public enum AssignmentEnum
{
    TakenCare,
    SelfCancel,
    CancelAdmin,
    CancelExpired
}

