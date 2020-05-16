using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Solitare.Converters
{
    public sealed class RotationConverter : IValueConverter
    {
        /// <summary>
        /// Convert bool or Nullable bool to Visibility
        /// </summary>
        /// <param name="value">bool or Nullable bool</param>
        /// <param name="targetType">Visibility</param>
        /// <param name="parameter">null</param>
        /// <param name="culture">null</param>
        /// <returns>Visible or Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rotation = 0;
            try
            {
                if (int.TryParse(parameter as string, out int paramInt))
                {
                    rotation = paramInt;

                    if (Enum.TryParse(value.ToString(), out PlayerRotationState state))
                    {
                        switch (state)
                        {
                            case PlayerRotationState.All:
                                return rotation;
                            case PlayerRotationState.None:
                                return 0;
                            case PlayerRotationState.Sides:
                                // For the sides, don't rotation the 180 cases
                                if (rotation % 180 == 0)
                                {
                                    return 0;
                                }
                                return rotation;
                        }
                    }
                }
            }
            catch { }

            return rotation;
        }

        /// <summary>
        /// Convert Visibility to boolean
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !((bool)value);
            }
            else
            {
                return false;
            }
        }
    }
}
