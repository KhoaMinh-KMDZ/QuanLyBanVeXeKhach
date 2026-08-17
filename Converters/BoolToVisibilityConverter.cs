using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QuanLyBanVeXeKhach.Converters
{
    // Lớp này dùng để chuyển đổi (Convert) giá trị kiểu đúng/sai (bool) 
    // sang trạng thái hiển thị/ẩn (Visibility) trên giao diện phần mềm.
    // Lưu ý: Các hàm Convert và ConvertBack là bắt buộc phải giữ nguyên tên tiếng Anh 
    // vì chúng ta đang kế thừa (thực thi) giao diện IValueConverter của WPF.
    public class BoolToVisibilityConverter : IValueConverter
    {
        // Hàm chuyển đổi từ Bool (dữ liệu code) sang Visibility (giao diện)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Bước 1: Kiểm tra xem giá trị truyền vào có phải là kiểu bool hay không
            bool giaTriDungSai = false;
            
            if (value is bool)
            {
                // Nếu đúng là kiểu bool, ta ép kiểu và lấy giá trị đó
                giaTriDungSai = (bool)value;
            }

            // Bước 2: Kiểm tra xem có yêu cầu đảo ngược kết quả (Invert) hay không
            // Tham số parameter có thể là chữ "Invert" được truyền từ file XAML (giao diện)
            if (parameter != null && parameter.ToString() == "Invert")
            {
                // Đảo ngược giá trị (ví dụ từ true thành false, false thành true)
                giaTriDungSai = !giaTriDungSai;
            }

            // Bước 3: Trả về trạng thái hiển thị trên giao diện
            if (giaTriDungSai == true)
            {
                return Visibility.Visible; // Hiện lên màn hình
            }
            else
            {
                return Visibility.Collapsed; // Ẩn đi hoàn toàn khỏi màn hình
            }
        }

        // Hàm chuyển đổi ngược lại từ Visibility (giao diện) sang Bool (dữ liệu code)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Bước 1: Mặc định ban đầu cho biến là false
            bool ketQua = false;

            // Bước 2: Kiểm tra xem giá trị từ giao diện đưa vào có phải là Visibility không
            if (value is Visibility)
            {
                Visibility trangThaiHienThi = (Visibility)value;
                
                // Nếu đang hiển thị (Visible) thì biến ketQua sẽ là true, ngược lại là false
                if (trangThaiHienThi == Visibility.Visible)
                {
                    ketQua = true;
                }
            }

            // Bước 3: Trả kết quả về
            return ketQua;
        }
    }
}
