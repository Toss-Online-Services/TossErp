namespace TossErp.HumanResources.Domain.Enums;

public enum Gender
{
    Male = 0,
    Female = 1,
    Other = 2,
    PreferNotToSay = 3
}

public enum EmploymentStatus
{
    Active = 0,
    Inactive = 1,
    Left = 2,
    Suspended = 3,
    Terminated = 4,
    OnLeave = 5,
    Retired = 6
}

public enum EmploymentType
{
    FullTime = 0,
    PartTime = 1,
    Contract = 2,
    Temporary = 3,
    Intern = 4,
    Consultant = 5
}

public enum MaritalStatus
{
    Single = 0,
    Married = 1,
    Divorced = 2,
    Widowed = 3,
    Separated = 4
}

public enum LeaveType
{
    Annual = 0,
    Sick = 1,
    Maternity = 2,
    Paternity = 3,
    Compassionate = 4,
    Study = 5,
    Unpaid = 6,
    CompensatoryOff = 7,
    Emergency = 8
}

public enum LeaveStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}

public enum AttendanceStatus
{
    Present = 0,
    Absent = 1,
    HalfDay = 2,
    Late = 3,
    OnLeave = 4,
    Holiday = 5,
    WeekOff = 6
}

public enum PayrollFrequency
{
    Monthly = 0,
    BiWeekly = 1,
    Weekly = 2,
    Quarterly = 3,
    Annually = 4
}

public enum SalaryComponentType
{
    Earning = 0,
    Deduction = 1,
    Allowance = 2,
    Bonus = 3,
    Tax = 4,
    Insurance = 5,
    Other = 6
}

public enum SalaryComponent
{
    BasicSalary = 0,
    HouseAllowance = 1,
    TransportAllowance = 2,
    MedicalAllowance = 3,
    Bonus = 4,
    Overtime = 5,
    Commission = 6,
    Tax = 7,
    Pension = 8,
    Insurance = 9,
    Other = 10
}

public enum PayrollStatus
{
    Draft = 0,
    Submitted = 1,
    Processed = 2,
    Paid = 3,
    Cancelled = 4
}

public enum PerformanceRating
{
    Outstanding = 5,
    ExceedsExpectations = 4,
    MeetsExpectations = 3,
    BelowExpectations = 2,
    Unsatisfactory = 1
}

public enum TrainingStatus
{
    Planned = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    Postponed = 4
}
