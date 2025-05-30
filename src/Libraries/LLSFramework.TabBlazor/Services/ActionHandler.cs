namespace LLSFramework.TabBlazor.Services;

/// <summary>
/// Provides utility methods for handling asynchronous actions with consistent UI feedback,
/// including loading state management, toast notifications, and modal dialog control.
/// </summary>
public class ActionHandler(BlazorMediator blazorMediator, IModalService modalService, LoadingState loadingState)
{
    /// <summary>
    /// Handles an asynchronous action, showing loading state and handling exceptions.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    public async Task HandleAsync(Func<Task> action)
        => await PrivateHandleAsync(action);

    /// <summary>
    /// Handles an asynchronous action, optionally showing a success message and closing a modal.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="successMessage">Message to display on success (optional).</param>
    /// <param name="modalResult">Modal result to close with (optional).</param>
    public async Task HandleAsync(Func<Task> action, string? successMessage = null, ModalResult? modalResult = null)
        => await PrivateHandleAsync(action, successMessage, modalResult: modalResult ?? ModalResult.Ok());

    /// <summary>
    /// Handles an asynchronous action that returns a result, showing loading state,
    /// publishing toast notifications for success or error, and closing the modal with the result.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the action.</typeparam>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="successMessage">Message to display on success (optional).</param>
    /// <param name="errorMessage">Message to display on error (optional).</param>
    public async Task HandleAsync<T>(Func<Task<T>> action, string? successMessage = null, string? errorMessage = null)
    {
        loadingState.Start();

        T result = default!;

        try
        {
            result = await action.Invoke();

            // Show a toast notification if a success message is provided.
            if (!string.IsNullOrWhiteSpace(successMessage))
                await blazorMediator.Publish(new ToastMessageSent(successMessage));
        }
        catch (Exception ex)
        {
            // Show a toast notification with the error message or exception message.
            if (!string.IsNullOrWhiteSpace(errorMessage))
                await blazorMediator.Publish(new ToastMessageSent(errorMessage));
            else
                await blazorMediator.Publish(new ToastMessageSent(ex.Message));
        }

        loadingState.Stop();

        // Close the modal and return the result.
        modalService.Close(ModalResult.Ok(result));
    }

    /// <summary>
    /// Internal method to handle an asynchronous action with optional success/error messages and modal result.
    /// Manages loading state, toast notifications, and modal closing.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <param name="successMessage">Message to display on success (optional).</param>
    /// <param name="errorMessage">Message to display on error (optional).</param>
    /// <param name="modalResult">Modal result to close with (optional).</param>
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

            // Show a toast notification if a success message is provided.
            if (!string.IsNullOrWhiteSpace(successMessage))
                await blazorMediator.Publish(new ToastMessageSent(successMessage));
        }
        catch (Exception ex)
        {
            // Show a toast notification with the error message or exception message.
            if (!string.IsNullOrWhiteSpace(errorMessage))
                await blazorMediator.Publish(new ToastMessageSent(errorMessage));
            else
                await blazorMediator.Publish(new ToastMessageSent(ex.Message));
        }

        loadingState.Stop();

        // Close the modal if a result is provided.
        if (modalResult != null)
            modalService.Close(modalResult);
    }
}