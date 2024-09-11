namespace DeliveryApp.Core.Interfaces;

/// <summary>
/// Передавання повідомлень
/// </summary>
public interface INavigationService
{
    Task SendMessageAsync(string message);

    Task<IEnumerable<string>> ReceiveMassagesAsync();
}
