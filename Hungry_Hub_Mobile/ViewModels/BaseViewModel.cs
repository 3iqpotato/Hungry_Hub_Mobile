using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Hungry_Hub_Mobile.Services.Interfaces;

namespace Hungry_Hub_Mobile.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    
    private bool _isBusy;
    private string _title;
    private string _errorMessage;

    protected BaseViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        
        LogoutCommand = new Command(async () => await ExecuteLogoutAsync());
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }

    public bool IsNotBusy => !_isBusy;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public ICommand LogoutCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual async Task ExecuteLogoutAsync()
    {
        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("👉 Изпълняване на логаут от базовия клас...");
            
            await _authService.LogoutAsync();
            await _navigationService.GoToAsync("start");
            
            System.Diagnostics.Debug.WriteLine("✅ Успешен логаут");
        }, "Грешка при изход");
    }

    protected async Task ExecuteAsync(Func<Task> operation, string errorMessage = null)
    {
        if (IsBusy) return;
        
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await operation();
        }
        catch (HttpRequestException ex)
        {
            var message = ex.Message;
            var jsonStart = message.IndexOf('{');
            if (jsonStart >= 0)
            {
                var json = message.Substring(jsonStart);
                message = ParseApiError(json);
            }
            ErrorMessage = message;
        }
        catch (Exception ex)
        {
            ErrorMessage = errorMessage ?? ex.Message;
            System.Diagnostics.Debug.WriteLine($"Грешка: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string ParseApiError(string json)
    {
        try
        {
            var doc = System.Text.Json.JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("detail", out var detail))
                return detail.GetString() ?? "Грешка.";

            var messages = new List<string>();
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                    foreach (var item in prop.Value.EnumerateArray())
                        messages.Add(item.GetString() ?? "");
                else if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.String)
                    messages.Add(prop.Value.GetString() ?? "");
            }
            return messages.Count > 0 ? string.Join("\n", messages) : "Грешка.";
        }
        catch
        {
            return json;
        }
    }
}