using System.Windows;
using AakStudio.Shell.UI.Controls;
using DX12Editor.ViewModels;

namespace DX12Editor.Views
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