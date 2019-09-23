using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// Based on https://github.com/HassaanAkbar/JoystickControl

namespace RaspberryPiCar.UWPApp.Joystick {
    public sealed partial class Joystick : UserControl {

        public double X {
            get => _joystickX / _controlRadius;
            private set => _joystickX = value;
        }

        public double Y {
            get => -_joystickY / _controlRadius;
            private set => _joystickY = value;
        }

        public Joystick() {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            ConfigureEvents();
            SetTheme();
            SetDefaults();
        }

        void ConfigureEvents() {
            Window.Current.Content.PointerMoved    += OnPointerMoved;
            Window.Current.Content.PointerReleased += OnPointerReleased;
            Window.Current.Content.LostFocus       += OnLostFocus;
        }

        void SetDefaults() {
            InnerDiameter = InnerDiameter == 0 ? 60 : InnerDiameter;
            OuterDiameter = OuterDiameter == 0 ? 100 : OuterDiameter;
            Theme         = 0;
        }

        void SetTheme() {
            switch ( Theme ) {
                case JoystickTheme.AccentTheme:
                    SolidColorBrush x = Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush;
                    InnerFill  ??=x;
                    OuterStroke??=x;
                    break;

                case JoystickTheme.Dark:
                    OuterFill ??=(Brush)Resources["DarkA"];
                    InnerFill =  (Brush)Resources["DarkB"];
                    break;

                case JoystickTheme.Light:
                    OuterFill??=(Brush)Resources["LightA"];
                    InnerFill??=(Brush)Resources["LightB"];
                    break;

            }
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs eventArgs) {
            if ( !_controllerPressed ) {
                return;
            }

            double x    = eventArgs.GetCurrentPoint(ControlArea).Position.X - _controlRadius;
            double y    = eventArgs.GetCurrentPoint(ControlArea).Position.Y - _controlRadius;
            double disp = Math.Sqrt(x * x + y * y);

            if ( disp < _controlRadius ) {
                JoystickTransform.X = X = x;
                JoystickTransform.Y = Y = y;
            } else {
                JoystickTransform.X = X = _controlRadius * (x / disp); //A*cos(x)
                JoystickTransform.Y = Y = _controlRadius * (y / disp); //A*sin(x)
            }

            JoystickMoved?.Invoke(this, new JoystickEventArgs(X, Y));
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
            => OnPointerReleased(sender, null);

        private void OnPointerReleased(object sender, PointerRoutedEventArgs pointerRoutedEventArgs) {
            _controllerPressed  = false;
            JoystickTransform.X = 0;
            JoystickTransform.Y = 0;
            X                   = 0;
            Y                   = 0;
            JoystickReleased?.Invoke(this, new EventArgs());
        }

        private void Controller_PointerPressed(object sender, PointerRoutedEventArgs e) {
            _controlRadius     = OuterDiameter / 2;
            _controllerPressed = true;
            JoystickPressed?.Invoke(this, new EventArgs());
        }

        /// <summary>  
        ///  Gets or sets the outer diameter of joystick.  
        /// </summary>  
        public double OuterDiameter {
            get => (double)GetValue(OuterDiameterProperty);
            set => SetValue(OuterDiameterProperty, value);
        }

        public static readonly DependencyProperty OuterDiameterProperty =
            DependencyProperty.Register("OuterDiameter", typeof(double), typeof(Joystick), null);

        /// <summary> 
        ///  Gets or sets inner diameter of the joystick.  
        /// </summary>
        public double InnerDiameter {
            get => (double)GetValue(InnerDiameterProperty);
            set => SetValue(InnerDiameterProperty, value);
        }

        public static readonly DependencyProperty InnerDiameterProperty =
            DependencyProperty.Register("InnerDiameter", typeof(double), typeof(Joystick), null);

        /// <summary>  
        ///  Gets or sets the color of joystick i.e color of inner circle.  
        /// </summary>
        public Brush InnerFill {
            get => (Brush)GetValue(InnerFillProperty);
            set => SetValue(InnerFillProperty, value);
        }

        public static readonly DependencyProperty InnerFillProperty =
            DependencyProperty.Register("InnerFill", typeof(Brush), typeof(Joystick), null);

        /// <summary>  
        ///  Gets or sets the inner border color of joystick i.e border of inner circle.  
        /// </summary>
        public Brush InnerStroke {
            get => (Brush)GetValue(InnerStrokeProperty);
            set => SetValue(InnerStrokeProperty, value);
        }

        public static readonly DependencyProperty InnerStrokeProperty =
            DependencyProperty.Register("InnerStroke", typeof(Brush), typeof(Joystick), null);

        /// <summary>  
        ///  Gets or sets the color of joystick control area.  
        /// </summary>
        public Brush OuterFill {
            get => (Brush)GetValue(OuterFillProperty);
            set => SetValue(OuterFillProperty, value);
        }

        public static readonly DependencyProperty OuterFillProperty =
            DependencyProperty.Register("OuterFill", typeof(Brush), typeof(Joystick), null);

        /// <summary>  
        ///  Gets or sets the border color of joystick control area.  
        /// </summary>
        public Brush OuterStroke {
            get => (Brush)GetValue(OuterStrokeProperty);
            set => SetValue(OuterStrokeProperty, value);
        }

        public static readonly DependencyProperty OuterStrokeProperty =
            DependencyProperty.Register("OuterStroke", typeof(Brush), typeof(Joystick), null);

        public enum JoystickTheme {
            AccentTheme,
            Dark,
            Light
        }

        public JoystickTheme Theme {
            get => (JoystickTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Theme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(JoystickTheme), typeof(Joystick), new PropertyMetadata(0));

        public event EventHandler<JoystickEventArgs> JoystickMoved;
        public event EventHandler                    JoystickPressed;
        public event EventHandler                    JoystickReleased;

        private double _joystickY;
        private double _joystickX;
        private bool   _controllerPressed;
        private double _controlRadius;
    }
}