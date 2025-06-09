using Leo.Classes;
using Leo.WindowModels;

namespace Leo.PageModels
{
    public partial class Home
    {
        private static Home? _instance;
        private static double _opacityBuffer = 0.5;

        private static readonly MessageBox MessageBox = new();
        private static readonly Logger Logger = new();

        public Home()
        {
            InitializeComponent();

            _instance = this;
            LogoEffect.Opacity = _opacityBuffer;
        }

        public static async void activateAnimation()
        {
            try
            {
                for (var i = 0; i < 50; i++)
                {
                    _instance!.LogoEffect.Opacity = _instance.LogoEffect.Opacity + 0.1;
                    await Task.Delay(10);
                }

                _opacityBuffer = _instance!.LogoEffect.Opacity;
            }
            catch (Exception ex)
            {
                Logger.error("Async error in activation animation\n" + ex);
                MessageBox.showMessage( Properties.Resources.messageBox_errorSign, Properties.Resources.system_message5,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }

        public static async void deactivateAnimation()
        {
            try
            {
                for (var i = 0; i < 50; i++)
                {
                    _instance!.LogoEffect.Opacity = _instance.LogoEffect.Opacity - 0.1;
                    await Task.Delay(10);
                }

                _opacityBuffer = _instance!.LogoEffect.Opacity;
            }
            catch (Exception ex)
            {
                Logger.error("Async error in deactivation animation\n" + ex);
                MessageBox.showMessage( Properties.Resources.messageBox_errorSign, Properties.Resources.system_message5,
                    MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
            }
        }
    }
}
