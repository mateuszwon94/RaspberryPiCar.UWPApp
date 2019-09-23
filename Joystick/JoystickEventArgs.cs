using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryPiCar.UWPApp.Joystick {
    public class JoystickEventArgs {
        public JoystickEventArgs() {
            X = 0;
            Y = 0;
        }

        public JoystickEventArgs(double x, double y) {
            X = x;
            Y = y;
        }

        public JoystickEventArgs((double X, double Y) position) {
            X = position.X;
            Y = position.Y;
        }

        public double X { get; private set; }

        public double Y { get; private set; }

        public (double X, double Y) Position => (X, Y);
    }
}
