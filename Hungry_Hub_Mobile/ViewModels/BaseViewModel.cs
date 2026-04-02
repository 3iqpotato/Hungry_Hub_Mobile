using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Hungry_Hub_Mobile.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    private bool _isBusy;
    private string _title;
    private string _errorMessage;

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

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected async Task ExecuteAsync(Func<Task> operation, string errorMessage = null)
    {
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await operation();
        }
        catch (HttpRequestException ex)
        {
            // Извади само JSON частта след "HTTP 401: "
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

            // {"detail": "Грешен email или парола"}
            if (doc.RootElement.TryGetProperty("detail", out var detail))
                return detail.GetString() ?? "Грешка.";

            // {"email": ["..."]} или {"img": ["..."]}
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