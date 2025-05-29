namespace LLSFramework.TabBlazor.Components.Lookups;

public static class LookupExtensions
{
    public static LookupItemDtoBase ToDto(this LookupItemViewModelBase model)
    {
        return new LookupItemDtoBase
        {
            Id = model.Id,
            Text = model.Text
        };
    }

    public static LookupItemViewModelBase ToViewModel(this LookupItemDtoBase dto)
    {
        return new LookupItemViewModelBase
        {
            Id = dto.Id,
            Text = dto.Text
        };
    }

    public static List<LookupItemViewModelBase> ToViewModelList(this IEnumerable<LookupItemDtoBase> list)
    {
        return [.. list.Select(x => x.ToViewModel())];
    }

    public static List<LookupItemDtoBase> ToDtoList(this IEnumerable<LookupItemViewModelBase> list)
    {
        return [.. list.Select(x => x.ToDto())];
    }

    public static List<T> ToDerivedList<T>(this IEnumerable<LookupItemViewModelBase> list) where T : LookupItemViewModelBase, new()
    {
        return [.. list.Select(LookupItemViewModelBase.ToDerived<T>)];
    }

    public static List<LookupItemViewModelBase> ToBaseList<T>(this IEnumerable<T> list) where T : LookupItemViewModelBase
    {
        return [.. list.Select(item => item.ToBase())];
    }

    public static List<LookupItemViewModelBase> ToBaseList<T>(this T item) where T : LookupItemViewModelBase
    {
        return [item.ToBase()];
    }
}