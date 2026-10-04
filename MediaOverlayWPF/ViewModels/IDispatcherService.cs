namespace MediaOverlayWPF.ViewModels
{
    public interface IDispatcherService
    {
        Task InvokeAsync(Func<Task> Action);
        Task<Task<T>> InvokeAsync<T>(Func<Task<T>> Action);
    }
}
