namespace LLSFramework.TabBlazor.Components.Toast;

public class ToastBuilder(ToastService toastService)
{
    private int _delay = 5;
    private ToastPosition _position = ToastPosition.BottomEnd;
    private bool _showHeader = true;
    private bool _showProgress = true;
    private bool _allowUserRemove;
    private string _title = string.Empty;
    private string _subTitle = string.Empty;

    public ToastBuilder Delay(int delay = 5)
    {
        _delay = delay;

        return this;
    }

    public ToastBuilder Position(ToastPosition position)
    {
        _position = position;

        return this;
    }

    public ToastBuilder ShowHeader(bool showHeader)
    {
        _showHeader = showHeader;

        return this;
    }

    public ToastBuilder ShowProgress(bool showProgress)
    {
        _showProgress = showProgress;

        return this;
    }

    public ToastBuilder AllowUserRemove(bool allowUserRemove)
    {
        _allowUserRemove = allowUserRemove;

        return this;
    }

    public ToastBuilder Title(string title)
    {
        _title = title;

        return this;
    }

    public ToastBuilder SubTitle(string subTitle)
    {
        _subTitle = subTitle;

        return this;
    }

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