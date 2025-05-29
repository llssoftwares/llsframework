namespace LLSFramework.Core.Dtos;

/// <summary>
/// Represents a base Data Transfer Object (DTO) for lookup items.
/// Typically used for populating dropdowns, lists, or selection controls.
/// </summary>
public class LookupItemDtoBase
{
    /// <summary>
    /// Gets or sets the unique identifier for the lookup item.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the display text for the lookup item.
    /// </summary>
    public required string Text { get; set; }
}