namespace LLSFramework.TabBlazor.Components.Modals;

/// <summary>
/// Represents the result of a lookup modal dialog interaction.
/// Contains the selected items and a flag indicating whether the modal was cancelled.
/// </summary>
public class LookupModalResult(List<LookupItemViewModelBase> selected, bool cancelled)
{
    /// <summary>
    /// Gets the list of items selected by the user in the modal.
    /// </summary>
    public List<LookupItemViewModelBase> Selected { get; } = selected;

    /// <summary>
    /// Gets a value indicating whether the modal was cancelled (true if the user closed or dismissed the modal without making a selection).
    /// </summary>
    public bool Cancelled { get; } = cancelled;
}