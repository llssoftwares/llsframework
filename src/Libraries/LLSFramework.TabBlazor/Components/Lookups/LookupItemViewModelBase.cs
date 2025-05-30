namespace LLSFramework.TabBlazor.Components.Lookups;

/// <summary>
/// Represents a base view model for lookup items, typically used in dropdowns, lists, or selection controls.
/// Provides basic properties and conversion methods for derived lookup item types.
/// </summary>
public class LookupItemViewModelBase
{
    /// <summary>
    /// Gets or sets the unique identifier for the lookup item.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display text for the lookup item.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Converts this base view model to a derived type <typeparamref name="T"/>.
    /// Copies the Id and Text properties to the new instance.
    /// </summary>
    /// <typeparam name="T">The derived type of <see cref="LookupItemViewModelBase"/>.</typeparam>
    /// <param name="model">The base model to convert.</param>
    /// <returns>A new instance of type <typeparamref name="T"/> with copied values.</returns>
    public static T ToDerived<T>(LookupItemViewModelBase model) where T : LookupItemViewModelBase, new()
    {
        return new T
        {
            Id = model.Id,
            Text = model.Text
        };
    }

    /// <summary>
    /// Converts this instance to a new <see cref="LookupItemViewModelBase"/> (base type).
    /// Useful for casting derived types back to the base type.
    /// </summary>
    /// <returns>A new <see cref="LookupItemViewModelBase"/> with copied values.</returns>
    public LookupItemViewModelBase ToBase()
    {
        return new LookupItemViewModelBase
        {
            Id = Id,
            Text = Text
        };
    }
}