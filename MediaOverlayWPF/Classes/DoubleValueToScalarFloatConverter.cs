using System.Windows.Data;
using System.Globalization;

namespace MediaOverlayWPF.Classes
{
    public class DoubleValueToScalarFloatConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return (float)Value / 100;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return (double)Value * 100;
        }
    }
}
