using System;
using System.Windows;
using QuanLyBanVeXeKhach.Models;

namespace QuanLyBanVeXeKhach.Views
{
    public partial class InVeWindow : Window
    {
        public InVeWindow(VeXe ve, string tenTuyen, string thoiGianXuatBen)
        {
            InitializeComponent();
            
            txtMaVe.Text = $"#{ve.MaVe}";
            txtKhachHang.Text = ve.TenKhachHang;
            txtTuyenXe.Text = tenTuyen;
            txtXuatBen.Text = thoiGianXuatBen;
            txtSoGhe.Text = ve.SoGhe;
            txtGiaVe.Text = ve.GiaVe.ToString("N0") + " VNĐ";
            txtNgayIn.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // Trong thực tế sẽ gọi hàm in ra máy in nhiệt ở đây
            this.Close();
        }
    }
}
