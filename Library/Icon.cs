using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Library
{
    public class Icon
    {
        ImageAwesome _imageAwesome;

        public Icon(FontAwesomeIcon icon)
        {
            _imageAwesome = new ImageAwesome();
            _imageAwesome.Icon = icon;
        }

        public Icon(FontAwesomeIcon icon, Brush brush)
        {
            _imageAwesome = new ImageAwesome();
            _imageAwesome.Icon = icon;
            _imageAwesome.Foreground = brush;
        }

        public void Spin(double? duration = null)
        {
            if(duration != null)
            {
                _imageAwesome.Spin = true;
                _imageAwesome.SpinDuration = duration.Value;
            }
            else
            {
                _imageAwesome.Spin = false;
                _imageAwesome.SpinDuration = 0;
            }
            
            
        }

        public void Blink(double? duration = null)
        {
            if (duration != null)
            {
                Animation.Blink(_imageAwesome, duration.Value);
            }
            else
            {
                Animation.Blink(_imageAwesome, 0);
            }

                
        }

        public void Color(Brush brush)
        {
            _imageAwesome.Foreground = brush;
        }

        public ImageAwesome Get()
        {
            return _imageAwesome;
        }

        public void Generator(UIElement uIElement)
        {
            
        }
    }
}
