namespace LLSFramework.TabBlazor.Components.Lookups;

public class LookupItemViewModelBase
{
    public string Id { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public static T ToDerived<T>(LookupItemViewModelBase model) where T : LookupItemViewModelBase, new()
    {
        return new T
        {
            Id = model.Id,
            Text = model.Text
        };
    }

    public LookupItemViewModelBase ToBase()
    {
        return new LookupItemViewModelBase
        {
            Id = Id,
            Text = Text
        };
    }
}