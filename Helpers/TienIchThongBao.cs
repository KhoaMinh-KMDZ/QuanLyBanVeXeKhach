using System.Windows;

namespace QuanLyBanVeXeKhach.Helpers
{
    /// <summary>
    /// Lớp tiện ích thông báo: Giúp tách rời MessageBox của WPF ra khỏi ViewModel.
    /// Giúp ViewModel giữ được sự độc lập, dễ dàng viết Unit Test sau này.
    /// </summary>
    public static class TienIchThongBao
    {
        public static void HienThiThanhCong(string thongDiep, string tieuDe = "Thành Công")
        {
            MessageBox.Show(thongDiep, tieuDe, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void HienThiThongTin(string thongDiep, string tieuDe = "Thông Tin")
        {
            MessageBox.Show(thongDiep, tieuDe, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void HienThiLoi(string thongDiep, string tieuDe = "Lỗi")
        {
            MessageBox.Show(thongDiep, tieuDe, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void HienThiCanhBao(string thongDiep, string tieuDe = "Cảnh Báo")
        {
            MessageBox.Show(thongDiep, tieuDe, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static bool HoiDap(string thongDiep, string tieuDe = "Xác Nhận")
        {
            var ketQua = MessageBox.Show(thongDiep, tieuDe, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return ketQua == MessageBoxResult.Yes;
        }
    }
}
