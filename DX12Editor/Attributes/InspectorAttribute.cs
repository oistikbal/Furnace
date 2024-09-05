using System.Windows.Controls;

namespace DX12Editor.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InspectorAttribute<T> : Attribute where T : UserControl
    {
        public Type ContentType { get; private set; }

        public InspectorAttribute()
        {
            ContentType = typeof(T);
        }
    }
}
