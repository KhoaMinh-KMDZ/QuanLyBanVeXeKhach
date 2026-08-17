using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;
using QuanLyBanVeXeKhach.Repositories;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class BanVeViewModel : BaseViewModel
    {
        // ── Danh sách chuyến ─────────────────────────────────────
        private List<ChuyenXe> _danhSachChuyen = new();
        public List<ChuyenXe> DanhSachChuyen
        {
            get => _danhSachChuyen;
            set => SetProperty(ref _danhSachChuyen, value);
        }

        private ChuyenXe? _chuyenDuocChon;
        public ChuyenXe? ChuyenDuocChon
        {
            get => _chuyenDuocChon;
            set { SetProperty(ref _chuyenDuocChon, value); LoadDanhSachGhe(); DangKyTheoDoiVeXe(); }
        }

        // ── Sơ đồ ghế ────────────────────────────────────────────
        private ObservableCollection<GheXe> _danhSachGheTang1 = new();
        public ObservableCollection<GheXe> DanhSachGheTang1
        {
            get => _danhSachGheTang1;
            set => SetProperty(ref _danhSachGheTang1, value);
        }

        private ObservableCollection<GheXe> _danhSachGheTang2 = new();
        public ObservableCollection<GheXe> DanhSachGheTang2
        {
            get => _danhSachGheTang2;
            set => SetProperty(ref _danhSachGheTang2, value);
        }

        private GheXe? _gheDuocChon;
        public GheXe? GheDuocChon
        {
            get => _gheDuocChon;
            set => SetProperty(ref _gheDuocChon, value);
        }

        private bool _coHaiTang;
        public bool CoHaiTang
        {
            get => _coHaiTang;
            set => SetProperty(ref _coHaiTang, value);
        }

        private int _soCotGheTang1 = 3;
        public int SoCotGheTang1
        {
            get => _soCotGheTang1;
            set => SetProperty(ref _soCotGheTang1, value);
        }

        private int _soCotGheTang2 = 3;
        public int SoCotGheTang2
        {
            get => _soCotGheTang2;
            set => SetProperty(ref _soCotGheTang2, value);
        }

        // ── Thông tin khách hàng ──────────────────────────────────
        private string _sdtKhachHang = "";
        public string SDTKhachHang
        {
            get => _sdtKhachHang;
            set
            {
                SetProperty(ref _sdtKhachHang, value);
                if (_sdtKhachHang.Length >= 10) TimKhachHang();
            }
        }

        private string _hoTenKhachHang = "";
        public string HoTenKhachHang
        {
            get => _hoTenKhachHang;
            set => SetProperty(ref _hoTenKhachHang, value);
        }

        // ── Trạng thái UI ─────────────────────────────────────────
        private string _thongBao = "";
        public string ThongBao
        {
            get => _thongBao;
            set => SetProperty(ref _thongBao, value);
        }

        public string TomTatVe => GheDuocChon == null ? "Chưa chọn ghế"
            : $"Ghế {GheDuocChon.TenGhe}  —  {ChuyenDuocChon?.GiaVeText}";

        // ── Thanh Toán ─────────────────────────────────────────────
        private bool _isThanhToanOpen;
        public bool IsThanhToanOpen
        {
            get => _isThanhToanOpen;
            set => SetProperty(ref _isThanhToanOpen, value);
        }

        private bool _laTienMat = true;
        public bool LaTienMat
        {
            get => _laTienMat;
            set 
            { 
                SetProperty(ref _laTienMat, value); 
                if (!_laTienMat) { TienKhachDua = 0; TienThua = 0; }
            }
        }

        private decimal _tienKhachDua;
        public decimal TienKhachDua
        {
            get => _tienKhachDua;
            set
            {
                SetProperty(ref _tienKhachDua, value);
                if (ChuyenDuocChon != null)
                {
                    TienThua = _tienKhachDua - ChuyenDuocChon.GiaVe;
                }
            }
        }

        private decimal _tienThua;
        public decimal TienThua
        {
            get => _tienThua;
            set => SetProperty(ref _tienThua, value);
        }

        // ── Commands ──────────────────────────────────────────────
        public ICommand ChonGheCommand      { get; }
        public ICommand BanVeCommand        { get; }
        public ICommand TimKhachHangCommand { get; }
        public ICommand LamMoiCommand       { get; }
        public ICommand XacNhanThanhToanCommand { get; }
        public ICommand HuyThanhToanCommand { get; }

        private readonly ChuyenXeRepository  _chuyenRepo = new();
        private readonly VeXeRepository      _veRepo     = new();
        private readonly KhachHangRepository _khRepo     = new();

        public BanVeViewModel()
        {
            ChonGheCommand      = new RelayCommand(p => ChonGhe(p as GheXe));
            BanVeCommand        = new RelayCommand(_ => MoBangThanhToan());
            TimKhachHangCommand = new RelayCommand(_ => TimKhachHang());
            LamMoiCommand       = new RelayCommand(_ => Load());
            XacNhanThanhToanCommand = new RelayCommand(_ => XuLyXacNhanThanhToan());
            HuyThanhToanCommand = new RelayCommand(_ => IsThanhToanOpen = false);
            Load();
        }

        public void Load()
        {
            DanhSachChuyen = _chuyenRepo.GetAll(trangThai: null)
                .Where(c => c.TrangThai == 0 || c.TrangThai == 1).ToList();

            if (DanhSachChuyen.Any())
                ChuyenDuocChon = DanhSachChuyen[0];
        }

        /// <summary>
        /// Hàm này có nhiệm vụ tạo ra sơ đồ ghế tuỳ theo loại xe.
        /// Logic tạo sơ đồ đã được tách ra lớp TienIchSoDoGhe để gọn gàng hơn.
        /// </summary>
        private void LoadDanhSachGhe()
        {
            DanhSachGheTang1.Clear();
            DanhSachGheTang2.Clear();
            GheDuocChon = null;
            OnPropertyChanged(nameof(TomTatVe));

            if (ChuyenDuocChon == null) return;

            var danhSachGheDaBan = _veRepo.GetGheDaBan(ChuyenDuocChon.MaChuyen)
                               .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var kq = TienIchSoDoGhe.TaoSoDoGhe(ChuyenDuocChon.TenLoaiXe, danhSachGheDaBan);
            
            CoHaiTang = kq.CoHaiTang;
            SoCotGheTang1 = kq.SoCotTang1;
            SoCotGheTang2 = kq.SoCotTang2;
            DanhSachGheTang1 = new ObservableCollection<GheXe>(kq.Tang1);
            DanhSachGheTang2 = new ObservableCollection<GheXe>(kq.Tang2);
        }

        private SqlDependency? _dependency;

        /// <summary>
        /// Đăng ký theo dõi sự thay đổi trong Database để cập nhật sơ đồ ghế realtime.
        /// Đã thêm cơ chế gỡ (Unsubscribe) sự kiện cũ để chống Rò rỉ bộ nhớ (Memory Leak).
        /// </summary>
        private void DangKyTheoDoiVeXe()
        {
            if (ChuyenDuocChon == null) return;

            try
            {
                // Hủy đăng ký sự kiện cũ để tránh Memory Leak do tích tụ event handler
                if (_dependency != null)
                {
                    _dependency.OnChange -= Dependency_OnChange;
                    _dependency = null;
                }

                using var conn = new SqlConnection(DatabaseHelper.ConnectionString);
                conn.Open();

                using var cmd = new SqlCommand(
                    "SELECT MaVe, MaChuyen, SoGhe, TrangThai FROM dbo.VeXe WHERE MaChuyen = @MaChuyen;", conn);
                cmd.Parameters.AddWithValue("@MaChuyen", ChuyenDuocChon.MaChuyen);

                _dependency = new SqlDependency(cmd);
                _dependency.OnChange += Dependency_OnChange;

                using var reader = cmd.ExecuteReader(); 
            }
            catch { }
        }

        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadDanhSachGhe();
                    DangKyTheoDoiVeXe(); 
                });
            }
        }

        private void ChonGhe(GheXe? ghe)
        {
            if (ghe == null || ghe.DaDat) return;
            if (GheDuocChon != null) GheDuocChon.DuocChon = false;
            ghe.DuocChon = true;
            GheDuocChon = ghe;
            OnPropertyChanged(nameof(TomTatVe));
        }

        private void TimKhachHang()
        {
            if (string.IsNullOrWhiteSpace(SDTKhachHang)) return;
            var kh = _khRepo.GetBySDT(SDTKhachHang.Trim());
            if (kh != null) HoTenKhachHang = kh.HoTen;
        }

        private void MoBangThanhToan()
        {
            if (ChuyenDuocChon == null) { TienIchThongBao.HienThiCanhBao("Vui lòng chọn chuyến xe!"); return; }
            if (GheDuocChon == null || GheDuocChon.DaDat) { TienIchThongBao.HienThiCanhBao("Vui lòng chọn ghế trống!"); return; }
            if (string.IsNullOrWhiteSpace(SDTKhachHang)) { TienIchThongBao.HienThiCanhBao("Vui lòng nhập số điện thoại khách hàng!"); return; }
            if (string.IsNullOrWhiteSpace(HoTenKhachHang)) { TienIchThongBao.HienThiCanhBao("Vui lòng nhập họ tên khách hàng!"); return; }

            ThongBao = "";
            TienKhachDua = 0;
            TienThua = 0;
            LaTienMat = true;
            IsThanhToanOpen = true; // Mở bảng thanh toán
        }

        /// <summary>
        /// Xử lý xác nhận thanh toán. Gọi duy nhất 1 hàm xuống Repository để 
        /// thực thi Transaction (gộp lưu Khách + Vé).
        /// </summary>
        private void XuLyXacNhanThanhToan()
        {
            if (LaTienMat && TienKhachDua < ChuyenDuocChon!.GiaVe)
            {
                TienIchThongBao.HienThiCanhBao("Tiền khách đưa không đủ để thanh toán vé!");
                return;
            }

            try
            {
                int maNV = SessionManager.CurrentUser?.MaNV ?? 1;
                string ghiChu = LaTienMat ? $"Thanh toán Tiền mặt. Tiền khách đưa: {TienKhachDua:N0}đ, Tiền thừa: {TienThua:N0}đ" : "Thanh toán Chuyển khoản";

                var veMoi = new VeXe
                {
                    MaChuyen = ChuyenDuocChon!.MaChuyen,
                    MaNV = maNV,
                    SoGhe = GheDuocChon!.TenGhe,
                    GiaVe = ChuyenDuocChon!.GiaVe,
                    GhiChu = ghiChu,
                    TenKhachHang = HoTenKhachHang.Trim()
                };

                // Gọi Repository thực hiện thanh toán qua Transaction
                int maVe = _veRepo.ThucHienThanhToan(veMoi, HoTenKhachHang.Trim(), SDTKhachHang.Trim());

                if (maVe > 0)
                {
                    string thongDiep = $"✅ Thanh toán thành công!\n\n" +
                                       $"Mã vé:    #{maVe}\n" +
                                       $"Ghế:       {GheDuocChon.TenGhe}\n" +
                                       $"Khách:     {HoTenKhachHang}\n" +
                                       $"Giá vé:    {ChuyenDuocChon.GiaVeText}\n" +
                                       $"Hình thức: {(LaTienMat ? "Tiền mặt" : "Chuyển khoản")}\n" +
                                       $"Chuyến:   {ChuyenDuocChon.ThongTin}";
                                       
                    TienIchThongBao.HienThiThanhCong(thongDiep);

                    // Hiển thị giao diện In Vé giả lập
                    veMoi.MaVe = maVe;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var inVeWin = new Views.InVeWindow(veMoi, ChuyenDuocChon.TenTuyen, ChuyenDuocChon.ThoiGianXuatBen.ToString("dd/MM/yyyy HH:mm"));
                        inVeWin.ShowDialog();
                    });

                    SDTKhachHang = "";
                    HoTenKhachHang = "";
                    ThongBao = "";
                    IsThanhToanOpen = false;
                    LoadDanhSachGhe();
                }
                else
                {
                    TienIchThongBao.HienThiLoi("Thanh toán thất bại! Lỗi không xác định.");
                    IsThanhToanOpen = false;
                }
            }
            catch (Exception ex)
            {
                // ex.Message chính là đoạn báo lỗi lấy từ SQL RAISERROR (ví dụ: Trùng ghế)
                TienIchThongBao.HienThiLoi($"Lỗi hệ thống: {ex.Message}");
                IsThanhToanOpen = false;
            }
        }
    }
}
