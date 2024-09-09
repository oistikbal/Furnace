using System.Diagnostics;
using FurnaceEditor.Views;

namespace FurnaceEditor
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
