using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuanLyBanVeXeKhach.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value?.ToString()) switch
            {
                // Trạng thái chuyến xe
                "Sắp chạy"    => new SolidColorBrush(Color.FromRgb(33, 150, 243)),  // Blue
                "Đang chạy"   => new SolidColorBrush(Color.FromRgb(76, 175, 80)),   // Green
                "Hoàn thành"  => new SolidColorBrush(Color.FromRgb(158, 158, 158)), // Gray
                "Đã hủy"      => new SolidColorBrush(Color.FromRgb(244, 67, 54)),   // Red

                // Trạng thái vé
                "Đặt chỗ"     => new SolidColorBrush(Color.FromRgb(255, 152, 0)),   // Orange
                "Đã thanh toán" => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Green

                // Trạng thái nhân viên / tuyến xe
                "Đang làm"    => new SolidColorBrush(Color.FromRgb(76, 175, 80)),   // Green
                "Nghỉ việc"   => new SolidColorBrush(Color.FromRgb(244, 67, 54)),   // Red
                "Hoạt động"   => new SolidColorBrush(Color.FromRgb(76, 175, 80)),   // Green
                "Tạm dừng"    => new SolidColorBrush(Color.FromRgb(255, 152, 0)),   // Orange

                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
