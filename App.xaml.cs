using System.Windows;
using Microsoft.Data.SqlClient;
using QuanLyBanVeXeKhach.Helpers;

namespace QuanLyBanVeXeKhach
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            QuanLyBanVeXeKhach.Helpers.DatabaseHelper.RunMigration();

            // Kiểm tra kết nối trước
            if (!DatabaseHelper.TestConnection())
            {
                var ketNoiWin = new Views.KetNoiWindow();
                if (ketNoiWin.ShowDialog() != true)
                {
                    // Người dùng bấm X (Tắt) khi chưa kết nối thành công -> Đóng luôn app
                    this.Shutdown();
                    return;
                }
            }

            try 
            {
                // Bắt đầu theo dõi thay đổi từ SQL Server (SqlDependency)
                SqlDependency.Start(DatabaseHelper.ConnectionString);
            } 
            catch { } // Bỏ qua nếu CSDL chưa bật Service Broker

            // Khởi động ứng dụng từ màn hình đăng nhập
            var login = new Views.DangNhapView();
            login.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try 
            {
                // Dừng theo dõi khi tắt ứng dụng
                SqlDependency.Stop(DatabaseHelper.ConnectionString);
            } 
            catch { }
            
            base.OnExit(e);
        }
    }
}
