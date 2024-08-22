using DX12Editor.ViewModels;

namespace DX12Editor.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class WindowAttribute : Attribute
    {
        public string Name { get; private set; }
        public Type ViewModelType { get; private set; }

        public WindowAttribute(string name, Type viewModelType)
        {
            if (!typeof(ViewModelBase).IsAssignableFrom(viewModelType))
            {
                throw new ArgumentException($"The ViewModel type must inherit from {nameof(ViewModelBase)}.", nameof(viewModelType));
            }

            Name = name;
            ViewModelType = viewModelType;
        }
    }
}
