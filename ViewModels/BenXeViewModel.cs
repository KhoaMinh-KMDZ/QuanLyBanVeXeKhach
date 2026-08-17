using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;
using QuanLyBanVeXeKhach.Repositories;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class BenXeViewModel : BaseViewModel
    {
        private readonly BenXeRepository _repo;

        public ObservableCollection<BenXe> DanhSachBenXe { get; set; }

        private BenXe? _benXeDuocChon;
        public BenXe? BenXeDuocChon
        {
            get => _benXeDuocChon;
            set
            {
                SetProperty(ref _benXeDuocChon, value);
                if (value != null)
                {
                    BenXeDangSua = new BenXe
                    {
                        MaBenXe = value.MaBenXe,
                        TenBenXe = value.TenBenXe,
                        DiaChi = value.DiaChi,
                        TinhThanh = value.TinhThanh,
                        TrangThai = value.TrangThai
                    };
                }
            }
        }

        private BenXe _benXeDangSua;
        public BenXe BenXeDangSua
        {
            get => _benXeDangSua;
            set => SetProperty(ref _benXeDangSua, value);
        }

        private string _tuKhoa = "";
        public string TuKhoa
        {
            get => _tuKhoa;
            set { SetProperty(ref _tuKhoa, value); LoadData(); }
        }

        public ICommand ThemCommand { get; }
        public ICommand SuaCommand { get; }
        public ICommand XoaCommand { get; }
        public ICommand ResetCommand { get; }

        public BenXeViewModel()
        {
            _repo = new BenXeRepository();
            DanhSachBenXe = new ObservableCollection<BenXe>();
            _benXeDangSua = new BenXe();

            ThemCommand = new RelayCommand(_ => Them());
            SuaCommand = new RelayCommand(_ => Sua(), _ => BenXeDuocChon != null);
            XoaCommand = new RelayCommand(_ => Xoa(), _ => BenXeDuocChon != null);
            ResetCommand = new RelayCommand(_ => ResetForm());

            LoadData();
        }

        private void LoadData()
        {
            var ds = string.IsNullOrWhiteSpace(TuKhoa) ? _repo.GetAll() : _repo.GetAll(TuKhoa.Trim());
            DanhSachBenXe.Clear();
            foreach (var item in ds) DanhSachBenXe.Add(item);
        }

        private void ResetForm()
        {
            BenXeDangSua = new BenXe();
            BenXeDuocChon = null;
        }

        private void Them()
        {
            if (string.IsNullOrWhiteSpace(BenXeDangSua.TenBenXe))
            {
                TienIchThongBao.HienThiCanhBao("Vui lòng nhập tên bến xe!");
                return;
            }

            int id = _repo.Add(BenXeDangSua);
            if (id > 0)
            {
                MessageBox.Show("Thêm bến xe thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ResetForm();
            }
            else
                MessageBox.Show("Thêm bến xe thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Sua()
        {
            if (BenXeDuocChon == null) return;
            if (string.IsNullOrWhiteSpace(BenXeDangSua.TenBenXe))
            {
                TienIchThongBao.HienThiCanhBao("Vui lòng nhập tên bến xe!");
                return;
            }

            BenXeDangSua.MaBenXe = BenXeDuocChon.MaBenXe;
            if (_repo.Update(BenXeDangSua))
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            else
                MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Xoa()
        {
            if (BenXeDuocChon == null) return;
            if (MessageBox.Show($"Bạn có chắc muốn vô hiệu hóa bến xe {BenXeDuocChon.TenBenXe}?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (_repo.Delete(BenXeDuocChon.MaBenXe))
                {
                    MessageBox.Show("Đã vô hiệu hóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ResetForm();
                }
                else
                    MessageBox.Show("Lỗi khi vô hiệu hóa bến xe!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
