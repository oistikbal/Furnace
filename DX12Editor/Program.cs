using System.Diagnostics;
using DX12Editor.Views;

namespace DX12Editor
{
    class Program
    {
        [System.STAThreadAttribute()]
        public static void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
