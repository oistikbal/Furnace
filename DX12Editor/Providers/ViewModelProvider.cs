using DX12Editor.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DX12Editor.Providers
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
            return _serviceProvider.GetRequiredService(viewModelType);
        }
    }
}
