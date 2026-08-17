using System;
using System.Windows.Input;

namespace QuanLyBanVeXeKhach.Commands
{
    // Lớp LenhCoBan (RelayCommand) dùng để gắn các sự kiện nút bấm từ Giao diện (View) xuống Code xử lý (ViewModel)
    public class LenhCoBan : ICommand
    {
        // Hành động sẽ được thực thi khi nhấn nút
        private readonly Action<object> _thucThiThaoTac;
        
        // Điều kiện để quyết định nút có được sáng lên cho nhấn hay không
        private readonly Predicate<object> _dieuKienThucThi;

        // Sự kiện báo hiệu khi điều kiện thực thi thay đổi (ví dụ: nút Đăng nhập chỉ sáng khi đã nhập User/Pass)
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Hàm khởi tạo nhận vào một hàm Hành động và một hàm Điều kiện (có thể để trống)
        public LenhCoBan(Action<object> thucThiThaoTac, Predicate<object> dieuKienThucThi = null)
        {
            _thucThiThaoTac = thucThiThaoTac;
            _dieuKienThucThi = dieuKienThucThi;
        }

        // Kiểm tra xem nút bấm có được phép nhấn hay không (True = được nhấn)
        public bool CanExecute(object parameter)
        {
            return _dieuKienThucThi == null ? true : _dieuKienThucThi(parameter);
        }

        // Thực hiện thao tác khi nút được nhấn
        public void Execute(object parameter)
        {
            _thucThiThaoTac(parameter);
        }
    }
}
