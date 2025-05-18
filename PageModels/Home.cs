namespace Leo.PageModels
{
    public partial class Home
    {
        private static Home? _instance;
        private static double _opacityBuffer = 0.5;

        public Home()
        {
            InitializeComponent();

            _instance = this;
            LogoEffect.Opacity = _opacityBuffer;
        }

        public static async void activateAnimation()
        {
            for (var i = 0; i < 50; i++)
            {
                _instance!.LogoEffect.Opacity = _instance.LogoEffect.Opacity + 0.1;
                await Task.Delay(10);
            }

            _opacityBuffer = _instance!.LogoEffect.Opacity;
        }

        public static async void deactivateAnimation()
        {
            for (var i = 0; i < 50; i++)
            {
                _instance!.LogoEffect.Opacity = _instance.LogoEffect.Opacity - 0.1;
                await Task.Delay(10);
            }

            _opacityBuffer = _instance!.LogoEffect.Opacity;
        }
    }
}
