using AakStudio.Shell.UI.Controls;
using DX12Editor.ViewModels;

namespace DX12Editor.Views
{
    public partial class MainWindow : CustomChromeWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Closed += (object? sender, EventArgs e) => { System.Environment.Exit(0); };
        }
    }
}