using QuanLyBanVeXeKhach.Models;
using QuanLyBanVeXeKhach.Repositories;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class LichSuGiaoDichViewModel : BaseViewModel
    {
        private readonly LichSuRepository _repo;

        private ObservableCollection<LichSuGiaoDich> _danhSachLichSu = new ObservableCollection<LichSuGiaoDich>();
        public ObservableCollection<LichSuGiaoDich> DanhSachLichSu
        {
            get => _danhSachLichSu;
            set { _danhSachLichSu = value; OnPropertyChanged(); }
        }

        public ICommand TaiLaiCommand { get; }

        public LichSuGiaoDichViewModel()
        {
            _repo = new LichSuRepository();
            DanhSachLichSu = new ObservableCollection<LichSuGiaoDich>();
            TaiLaiCommand = new RelayCommand(_ => LoadData());
            LoadData();
        }

        private void LoadData()
        {
            var data = _repo.GetAll();
            DanhSachLichSu.Clear();
            foreach (var item in data)
            {
                DanhSachLichSu.Add(item);
            }
        }
    }
}
