using MediaOverlayWPF.ViewModels;
using System.Windows;

namespace MediaOverlayWPF.Classes
{
    public class WpfDispatcherService : IDispatcherService
    {
        public async Task InvokeAsync(Func<Task> Action)
        {
            await Application.Current.Dispatcher.InvokeAsync(Action);
        }

        public async Task<Task<T>> InvokeAsync<T>(Func<Task<T>> Action)
        {
            return await Application.Current.Dispatcher.InvokeAsync(Action);
        }
    }
}
