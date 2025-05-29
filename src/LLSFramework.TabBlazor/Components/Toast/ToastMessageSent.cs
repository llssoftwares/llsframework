namespace LLSFramework.TabBlazor.Components.Toast;

/// <summary>
/// Represents a notification event indicating that a toast message has been sent.
/// Implements <see cref="INotification"/> for use with notification or mediator patterns.
/// </summary>
/// <param name="Message">The content of the toast message to be displayed.</param>
public record ToastMessageSent(string Message) : INotification;