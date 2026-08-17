using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;
using QuanLyBanVeXeKhach.Repositories;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class NhanVienViewModel : BaseViewModel
    {
        private ObservableCollection<NhanVien> _danhSach = new();
        public ObservableCollection<NhanVien> DanhSach
        {
            get => _danhSach;
            set => SetProperty(ref _danhSach, value);
        }

        private NhanVien? _duocChon;
        public NhanVien? DuocChon
        {
            get => _duocChon;
            set { SetProperty(ref _duocChon, value); if (value != null) DienForm(value); }
        }

        public List<VaiTro> DanhSachVaiTro { get; set; } = new();

        private string _hoTen = "", _sdt = "", _email = "", _taiKhoan = "", _matKhauMoi = "";
        public string HoTen    { get => _hoTen;    set => SetProperty(ref _hoTen,    value); }
        public string SDT      { get => _sdt;      set => SetProperty(ref _sdt,      value); }
        public string Email    { get => _email;    set => SetProperty(ref _email,    value); }
        public string TaiKhoan { get => _taiKhoan; set => SetProperty(ref _taiKhoan, value); }
        public string MatKhauMoi { get => _matKhauMoi; set => SetProperty(ref _matKhauMoi, value); }

        private VaiTro? _vaiTroChon;
        public VaiTro? VaiTroChon { get => _vaiTroChon; set => SetProperty(ref _vaiTroChon, value); }

        private bool _trangThai = true;
        public bool TrangThai { get => _trangThai; set => SetProperty(ref _trangThai, value); }

        private bool _dangSua;
        public bool DangSua { get => _dangSua; set => SetProperty(ref _dangSua, value); }

        private string _keyword = "";
        public string Keyword { get => _keyword; set { SetProperty(ref _keyword, value); TimKiem(); } }

        public ICommand ThemMoiCommand      { get; }
        public ICommand LuuCommand          { get; }
        public ICommand HuyFormCommand      { get; }
        public ICommand DoiMatKhauCommand   { get; }
        public ICommand KhoaTKCommand       { get; }
        public ICommand MoKhoaTKCommand     { get; }

        private readonly NhanVienRepository _repo = new();

        public NhanVienViewModel()
        {
            ThemMoiCommand    = new RelayCommand(_ => BatDauThem());
            LuuCommand        = new RelayCommand(_ => Luu());
            HuyFormCommand    = new RelayCommand(_ => { DangSua = false; DuocChon = null; });
            DoiMatKhauCommand = new RelayCommand(_ => DoiMatKhau(), _ => DuocChon != null && !string.IsNullOrEmpty(MatKhauMoi));
            KhoaTKCommand     = new RelayCommand(_ => KhoaTK(false), _ => DuocChon?.TrangThai == true);
            MoKhoaTKCommand   = new RelayCommand(_ => KhoaTK(true),  _ => DuocChon?.TrangThai == false);
            Load();
        }

        private void Load()
        {
            DanhSachVaiTro = _repo.GetAllVaiTro();
            OnPropertyChanged(nameof(DanhSachVaiTro));
            TimKiem();
        }

        private void TimKiem()
        {
            DanhSach = new ObservableCollection<NhanVien>(
                _repo.GetAll(string.IsNullOrWhiteSpace(Keyword) ? null : Keyword));
        }

        private void DienForm(NhanVien nv)
        {
            HoTen      = nv.HoTen; SDT = nv.SDT; Email = nv.Email ?? "";
            TaiKhoan   = nv.TaiKhoan ?? ""; MatKhauMoi = "";
            VaiTroChon = DanhSachVaiTro.FirstOrDefault(v => v.MaVaiTro == nv.MaVaiTro);
            TrangThai  = nv.TrangThai;
            DangSua    = true;
        }

        private void BatDauThem()
        {
            DuocChon = null; HoTen = ""; SDT = ""; Email = ""; TaiKhoan = ""; MatKhauMoi = "123456";
            VaiTroChon = DanhSachVaiTro.FirstOrDefault(); TrangThai = true;
            DangSua = true;
        }

        private void Luu()
        {
            if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(SDT) || VaiTroChon == null)
            { MessageBox.Show("Điền đầy đủ họ tên, SĐT và vai trò!", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            var nv = new NhanVien
            {
                MaNV     = DuocChon?.MaNV ?? 0,
                HoTen    = HoTen.Trim(), SDT = SDT.Trim(),
                Email    = Email, TaiKhoan = TaiKhoan,
                MaVaiTro = VaiTroChon.MaVaiTro,
                MatKhauHash = string.IsNullOrEmpty(MatKhauMoi)
                    ? (DuocChon?.MatKhauHash ?? HashHelper.SHA256Hash("123456"))
                    : HashHelper.SHA256Hash(MatKhauMoi),
            };

            bool ok = DuocChon == null ? _repo.Add(nv) > 0 : _repo.Update(nv);
            if (ok) { DangSua = false; DuocChon = null; Load(); }
            else MessageBox.Show("Tài khoản đã tồn tại hoặc lỗi lưu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void DoiMatKhau()
        {
            if (DuocChon == null || string.IsNullOrEmpty(MatKhauMoi)) return;
            bool ok = _repo.DoiMatKhau(DuocChon.MaNV, HashHelper.SHA256Hash(MatKhauMoi));
            MessageBox.Show(ok ? "Đổi mật khẩu thành công!" : "Đổi mật khẩu thất bại!", ok ? "OK" : "Lỗi",
                MessageBoxButton.OK, ok ? MessageBoxImage.Information : MessageBoxImage.Error);
            MatKhauMoi = "";
        }

        private void KhoaTK(bool moKhoa)
        {
            if (DuocChon == null) return;
            bool ok = _repo.KhoaTaiKhoan(DuocChon.MaNV, moKhoa);
            if (ok) TimKiem();
        }
    }
}
