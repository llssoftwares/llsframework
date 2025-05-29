namespace LLSFramework.TabBlazor.Services;

public class ActionHandler(BlazorMediator blazorMediator, IModalService modalService, LoadingState loadingState)
{
    public async Task HandleAsync(Func<Task> action)
        => await PrivateHandleAsync(action);

    public async Task HandleAsync(Func<Task> action, string? successMessage = null, ModalResult? modalResult = null)
        => await PrivateHandleAsync(action, successMessage, modalResult: modalResult ?? ModalResult.Ok());

    public async Task HandleAsync<T>(Func<Task<T>> action, string? successMessage = null, string? errorMessage = null)
    {
        loadingState.Start();

        T result = default!;

        try
        {
            result = await action.Invoke();

            if (!string.IsNullOrWhiteSpace(successMessage))
                await blazorMediator.Publish(new ToastMessageSent(successMessage));
        }
        catch (Exception ex)
        {

            if (!string.IsNullOrWhiteSpace(errorMessage))
                await blazorMediator.Publish(new ToastMessageSent(errorMessage));
            else
                await blazorMediator.Publish(new ToastMessageSent(ex.Message));
        }

        loadingState.Stop();

        modalService.Close(ModalResult.Ok(result));
    }

    private async Task PrivateHandleAsync(
        Func<Task> action,
        string? successMessage = null,
        string? errorMessage = null,
        ModalResult? modalResult = null)
    {
        loadingState.Start();

        try
        {
            await action.Invoke();

            if (!string.IsNullOrWhiteSpace(successMessage))
                await blazorMediator.Publish(new ToastMessageSent(successMessage));
        }
        catch (Exception ex)
        {

            if (!string.IsNullOrWhiteSpace(errorMessage))
                await blazorMediator.Publish(new ToastMessageSent(errorMessage));
            else
                await blazorMediator.Publish(new ToastMessageSent(ex.Message));
        }

        loadingState.Stop();

        if (modalResult != null)
            modalService.Close(modalResult);
    }
}