using System.Windows;
using System.Windows.Input;
using QuanLyBanVeXeKhach.ViewModels;

namespace QuanLyBanVeXeKhach.Views
{
    public partial class DangNhapView : Window
    {
        private readonly DangNhapViewModel _vm;

        public DangNhapView()
        {
            InitializeComponent();
            _vm = new DangNhapViewModel();
            DataContext = _vm;

            _vm.DangNhapThanhCong = () =>
            {
                try
                {
                    var mainWin = new MainView();
                    Application.Current.MainWindow = mainWin;
                    mainWin.Closed += (s, e) => Application.Current.Shutdown();
                    mainWin.Show();
                    mainWin.Activate();
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể mở giao diện chính: " + ex.Message,
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }

        private void btnDangNhap_Click(object sender, RoutedEventArgs e)
            => _vm.DangNhapCommand.Execute(pbPass.Password);

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) _vm.DangNhapCommand.Execute(pbPass.Password);
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        // Kéo cửa sổ
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}
