using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;

namespace Library
{
    public class Animation
    {
        public static void Blink(UIElement element, double? duration = null)
        {
            if (duration != null) {
                DoubleAnimation da = new DoubleAnimation();
                da.From = 1;
                da.To = 0;
                da.Duration = new Duration(TimeSpan.FromSeconds(duration.Value));
                da.AutoReverse = true;
                da.RepeatBehavior = RepeatBehavior.Forever;
                element.BeginAnimation(UIElement.OpacityProperty, da);
            }
            else
            {
                element.BeginAnimation(UIElement.OpacityProperty, null);
            }
            
        }
    }
}
