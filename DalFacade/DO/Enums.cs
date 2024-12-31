
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
    Volunteer,
    Manager
}

/// <summary>
/// Types of calls received in the organization
/// </summary>
public enum TypeOfCall
{
    Shopping,
    Cleaning,
    Repairing,
    TechnologyHelp,
    Talking
}

/// <summary>
/// Type of Assignment
/// </summary>
public enum AssignmentEnum
{
    TakenCare,
    SelfCancel,
    CancelAdmin,
    CancelExpired
}

