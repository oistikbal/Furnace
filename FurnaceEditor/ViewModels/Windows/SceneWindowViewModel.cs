using FurnaceEditor.Services;

namespace FurnaceEditor.ViewModels.Windows
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
