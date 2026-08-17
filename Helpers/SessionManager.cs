using QuanLyBanVeXeKhach.Models;

namespace QuanLyBanVeXeKhach.Helpers
{
    public static class SessionManager
    {
        public static NhanVien? CurrentUser { get; private set; }
        public static VaiTro?   CurrentRole { get; private set; }

        public static void Login(NhanVien user, VaiTro role)
        {
            CurrentUser = user;
            CurrentRole = role;
        }

        public static void Logout()
        {
            CurrentUser = null;
            CurrentRole = null;
        }

        public static bool IsLoggedIn => CurrentUser != null;

        // Quyền hạn
        public static bool CanBanVe          => CurrentRole?.QuyenBanVe          ?? false;
        public static bool CanQuanLyChuyenXe => CurrentRole?.QuyenQuanLyChuyenXe ?? false;
        public static bool CanQuanLyDanhMuc  => CurrentRole?.QuyenQuanLyDanhMuc  ?? false;
        public static bool CanBaoCao         => CurrentRole?.QuyenBaoCao         ?? false;
        public static bool CanQuanLyNhanVien => CurrentRole?.QuyenQuanLyNhanVien ?? false;

        public static bool IsAdmin => CurrentRole?.TenVaiTro == "Admin";
    }
}
