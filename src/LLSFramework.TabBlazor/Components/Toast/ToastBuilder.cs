namespace LLSFramework.TabBlazor.Components.Toast;

/// <summary>
/// Provides a fluent builder API for configuring and displaying toast notifications.
/// Allows customization of toast appearance, behavior, and content before displaying.
/// </summary>
public class ToastBuilder(ToastService toastService)
{
    // The delay (in seconds) before the toast automatically disappears.
    private int _delay = 5;
    // The position on the screen where the toast will appear.
    private ToastPosition _position = ToastPosition.BottomEnd;
    // Whether to show the toast header (title/subtitle area).
    private bool _showHeader = true;
    // Whether to show a progress bar indicating the remaining time.
    private bool _showProgress = true;
    // Whether the user can manually remove the toast.
    private bool _allowUserRemove;
    // The main title text of the toast.
    private string _title = string.Empty;
    // The subtitle text of the toast.
    private string _subTitle = string.Empty;

    /// <summary>
    /// Sets the delay (in seconds) before the toast disappears.
    /// </summary>
    /// <param name="delay">The delay in seconds (default is 5).</param>
    /// <returns>The builder instance for chaining.</returns>
    public ToastBuilder Delay(int delay = 5)
    {
        _delay = delay;

        return this;
    }

    /// <summary>
    /// Sets the position on the screen where the toast will appear.
    /// </summary>
    /// <param name="position">The desired toast position.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ToastBuilder Position(ToastPosition position)
    {
        _position = position;

        return this;
    }

    /// <summary>
    /// Sets whether to show the toast header (title/subtitle area).
    /// </summary>
    /// <param name="showHeader">True to show the header; otherwise, false.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ToastBuilder ShowHeader(bool showHeader)
    {
        _showHeader = showHeader;

        return this;
    }

    /// <summary>
    /// Sets whether to show a progress bar on the toast.
    /// </summary>
    /// <param name="showProgress">True to show the progress bar; otherwise, false.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ToastBuilder ShowProgress(bool showProgress)
    {
        _showProgress = showProgress;

        return this;
    }

    /// <summary>
    /// Sets whether the user can manually remove the toast.
    /// </summary>
    /// <param name="allowUserRemove">True to allow user removal; otherwise, false.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ToastBuilder AllowUserRemove(bool allowUserRemove)
    {
        _allowUserRemove = allowUserRemove;

        return this;
    }

    /// <summary>
    /// Sets the main title text of the toast.
    /// </summary>
    /// <param name="title">The title text.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ToastBuilder Title(string title)
    {
        _title = title;

        return this;
    }

    /// <summary>
    /// Sets the subtitle text of the toast.
    /// </summary>
    /// <param name="subTitle">The subtitle text.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ToastBuilder SubTitle(string subTitle)
    {
        _subTitle = subTitle;

        return this;
    }

    /// <summary>
    /// Builds and displays the toast notification asynchronously with the configured options and message.
    /// </summary>
    /// <param name="message">The main message content of the toast.</param>
    public async Task AddAsync(string message)
    {
        await toastService.AddToastAsync(new ToastModel
        {
            Title = _title,
            SubTitle = _subTitle,
            Message = message,
            Options = new ToastOptions
            {
                Delay = _delay,
                Position = _position,
                ShowHeader = _showHeader,
                ShowProgress = _showProgress,
                AllowUserRemove = _allowUserRemove
            }
        });
    }
}