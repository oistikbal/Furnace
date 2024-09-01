using DX12Editor.Services;

namespace DX12Editor.ViewModels.Windows
{
    public class SceneWindowViewModel : ViewModelBase
    {
        private SceneService _sceneService;

        public SceneWindowViewModel(SceneService sceneService)
        {
            _sceneService = sceneService;
        }
    }
}
