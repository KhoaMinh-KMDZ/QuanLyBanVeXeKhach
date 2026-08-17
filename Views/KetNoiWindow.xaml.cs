using System.Windows;
using QuanLyBanVeXeKhach.Helpers;

namespace QuanLyBanVeXeKhach.Views
{
    public partial class KetNoiWindow : Window
    {
        public KetNoiWindow()
        {
            InitializeComponent();
            txtServerName.Text = DatabaseHelper.GetServerName();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            string serverName = txtServerName.Text.Trim();
            if (string.IsNullOrEmpty(serverName))
            {
                MessageBox.Show("Vui lòng nhập tên Server!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DatabaseHelper.SetServerName(serverName);

            if (DatabaseHelper.TestConnection())
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Kết nối thất bại!\n\nVui lòng kiểm tra lại tên Server, hoặc đảm bảo SQL Server đang chạy và bạn đã chạy file script tạo Database.", 
                    "Lỗi Kết Nối", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
