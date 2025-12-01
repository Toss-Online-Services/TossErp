namespace Toss.Domain.Enums;

/// <summary>
/// Represents the condition/state of an asset.
/// </summary>
public enum AssetCondition
{
    /// <summary>
    /// Asset is in excellent condition, like new.
    /// </summary>
    Excellent = 0,

    /// <summary>
    /// Asset is in good condition with minor wear.
    /// </summary>
    Good = 1,

    /// <summary>
    /// Asset is in fair condition, functional but showing signs of wear.
    /// </summary>
    Fair = 2,

    /// <summary>
    /// Asset is in poor condition, needs repair or maintenance.
    /// </summary>
    Poor = 3,

    /// <summary>
    /// Asset is not functional and requires significant repair.
    /// </summary>
    NeedsRepair = 4,

    /// <summary>
    /// Asset is no longer usable and should be disposed of.
    /// </summary>
    Obsolete = 5
}

