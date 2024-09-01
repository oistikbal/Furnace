using DX12Editor.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DX12Editor.Utilities.Providers
{
    public class ViewModelProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TViewModel GetViewModel<TViewModel>() where TViewModel : ViewModelBase
        {
            return _serviceProvider.GetRequiredService<TViewModel>();
        }

        public object GetViewModel(Type viewModelType)
        {
            // Resolve the service from the service provider
            var viewModel = _serviceProvider.GetRequiredService(viewModelType);

            // Check if the resolved type is a subclass of ViewModelBase
            if (!typeof(ViewModelBase).IsAssignableFrom(viewModelType))
            {
                throw new InvalidOperationException($"The type {viewModelType.FullName} is not a valid ViewModelBase.");
            }

            return viewModel;
        }
    }
}
