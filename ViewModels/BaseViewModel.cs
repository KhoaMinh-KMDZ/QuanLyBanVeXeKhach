using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace QuanLyBanVeXeKhach.ViewModels
{
    // Lớp này là lớp nền tảng cho mọi ViewModel trong mô hình MVVM (Model-View-ViewModel).
    // INotifyPropertyChanged là một interface giúp WPF biết được khi nào dữ liệu (property) thay đổi để nó tự động vẽ lại giao diện.
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Sự kiện này sẽ được gọi (kích hoạt) mỗi khi có một biến (property) bị thay đổi giá trị.
        public event PropertyChangedEventHandler? PropertyChanged;

        // Hàm này có nhiệm vụ "hô to" thông báo cho giao diện (View) biết rằng: "Ê giao diện, thuộc tính XYZ vừa thay đổi giá trị đó, cập nhật lại hiển thị đi!"
        // [CallerMemberName] là một mẹo nhỏ của C#, nó sẽ tự động lấy tên của thuộc tính đang gọi hàm này để truyền vào biến name.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Hàm này rất tiện dụng: Nó sẽ kiểm tra xem giá trị mới (value) có khác với giá trị cũ (field) hay không.
        // Nếu khác, nó sẽ gán giá trị mới và tự động gọi OnPropertyChanged để cập nhật giao diện.
        // Dùng hàm này giúp code ở các ViewModel con ngắn gọn hơn rất nhiều.
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false; // Nếu bằng nhau thì thôi, không làm gì cả
            field = value; // Gán giá trị mới
            OnPropertyChanged(name); // Báo cho giao diện biết
            return true;
        }
    }

    // RelayCommand là một lớp giúp chúng ta gắn các sự kiện click chuột (ví dụ: nhấn nút Button) vào các hàm xử lý bên trong ViewModel.
    // Lớp này triển khai (implement) ICommand của WPF.
    public class RelayCommand : ICommand
    {
        // _execute là hàm sẽ chạy khi người dùng bấm nút
        private readonly Action<object?> _execute;
        
        // _canExecute là hàm sẽ kiểm tra xem nút đó có ĐƯỢC PHÉP bấm hay không (nếu trả về false, nút sẽ bị mờ đi)
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute    = execute;
            _canExecute = canExecute;
        }

        // Khai báo sự kiện báo cho giao diện biết khi nào trạng thái "được phép bấm" (CanExecute) thay đổi.
        // CommandManager.RequerySuggested giúp WPF tự động kiểm tra lại trạng thái của nút liên tục.
        public event EventHandler? CanExecuteChanged
        {
            add    => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // WPF sẽ gọi hàm này liên tục để hỏi xem "Nút này có cho bấm không?"
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        
        // WPF sẽ gọi hàm này khi người dùng thực sự bấm vào nút
        public void Execute(object? parameter) => _execute(parameter);
    }
}
