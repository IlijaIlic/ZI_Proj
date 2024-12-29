using System.Configuration;
using System.Data;
using System.Windows;
using ZI_CRYPTER.Utils;

namespace ZI_CRYPTER
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static ViewModelBase ViewModelBaseInstance { get; private set; } = new ViewModelBase();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ViewModelBaseInstance.SharedDecodeOutputPutanja = "C:\\Program Files\\";
        }
    }

}
