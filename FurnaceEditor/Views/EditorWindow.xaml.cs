using System.Windows;
using AakStudio.Shell.UI.Controls;

namespace FurnaceEditor.Views
{
    public partial class EditorWindow : CustomChromeWindow
    {
        public EditorWindow()
        {
            InitializeComponent();
            Closed += (object? sender, EventArgs e) => { Application.Current.Shutdown(); };
        }
    }
}