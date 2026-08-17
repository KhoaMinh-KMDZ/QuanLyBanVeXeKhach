using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuanLyBanVeXeKhach.Converters
{
    // Chuyển bool (DaDat) → SolidColorBrush cho nút ghế
    public class GheColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDaDat && isDaDat)
                return new SolidColorBrush(Color.FromRgb(239, 83, 80));   // đỏ = đã bán
            return new SolidColorBrush(Color.FromRgb(102, 187, 106));     // xanh = còn trống
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
