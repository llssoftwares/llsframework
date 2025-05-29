namespace LLSFramework.TabBlazor.Components.Lookups;

/// <summary>
/// Extension methods for converting between lookup view models and DTOs,
/// as well as for transforming and projecting lookup collections.
/// </summary>
public static class LookupExtensions
{
    /// <summary>
    /// Converts a <see cref="LookupItemViewModelBase"/> instance to a <see cref="LookupItemDtoBase"/>.
    /// </summary>
    /// <param name="model">The view model to convert.</param>
    /// <returns>A new <see cref="LookupItemDtoBase"/> with copied Id and Text.</returns>
    public static LookupItemDtoBase ToDto(this LookupItemViewModelBase model)
    {
        return new LookupItemDtoBase
        {
            Id = model.Id,
            Text = model.Text
        };
    }

    /// <summary>
    /// Converts a <see cref="LookupItemDtoBase"/> instance to a <see cref="LookupItemViewModelBase"/>.
    /// </summary>
    /// <param name="dto">The DTO to convert.</param>
    /// <returns>A new <see cref="LookupItemViewModelBase"/> with copied Id and Text.</returns>
    public static LookupItemViewModelBase ToViewModel(this LookupItemDtoBase dto)
    {
        return new LookupItemViewModelBase
        {
            Id = dto.Id,
            Text = dto.Text
        };
    }

    /// <summary>
    /// Converts a collection of <see cref="LookupItemDtoBase"/> to a list of <see cref="LookupItemViewModelBase"/>.
    /// </summary>
    /// <param name="list">The DTO collection to convert.</param>
    /// <returns>A list of view models.</returns>
    public static List<LookupItemViewModelBase> ToViewModelList(this IEnumerable<LookupItemDtoBase> list)
    {
        return [.. list.Select(x => x.ToViewModel())];
    }

    /// <summary>
    /// Converts a collection of <see cref="LookupItemViewModelBase"/> to a list of <see cref="LookupItemDtoBase"/>.
    /// </summary>
    /// <param name="list">The view model collection to convert.</param>
    /// <returns>A list of DTOs.</returns>
    public static List<LookupItemDtoBase> ToDtoList(this IEnumerable<LookupItemViewModelBase> list)
    {
        return [.. list.Select(x => x.ToDto())];
    }

    /// <summary>
    /// Converts a collection of <see cref="LookupItemViewModelBase"/> to a list of a derived type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The derived type of <see cref="LookupItemViewModelBase"/>.</typeparam>
    /// <param name="list">The base view model collection to convert.</param>
    /// <returns>A list of derived view models.</returns>
    public static List<T> ToDerivedList<T>(this IEnumerable<LookupItemViewModelBase> list) where T : LookupItemViewModelBase, new()
    {
        return [.. list.Select(LookupItemViewModelBase.ToDerived<T>)];
    }

    /// <summary>
    /// Converts a collection of derived <see cref="LookupItemViewModelBase"/> types to a list of base view models.
    /// </summary>
    /// <typeparam name="T">The derived type of <see cref="LookupItemViewModelBase"/>.</typeparam>
    /// <param name="list">The derived view model collection to convert.</param>
    /// <returns>A list of base view models.</returns>
    public static List<LookupItemViewModelBase> ToBaseList<T>(this IEnumerable<T> list) where T : LookupItemViewModelBase
    {
        return [.. list.Select(item => item.ToBase())];
    }

    /// <summary>
    /// Converts a single derived <see cref="LookupItemViewModelBase"/> instance to a list containing its base view model.
    /// </summary>
    /// <typeparam name="T">The derived type of <see cref="LookupItemViewModelBase"/>.</typeparam>
    /// <param name="item">The derived view model to convert.</param>
    /// <returns>A list containing the base view model.</returns>
    public static List<LookupItemViewModelBase> ToBaseList<T>(this T item) where T : LookupItemViewModelBase
    {
        return [item.ToBase()];
    }
}