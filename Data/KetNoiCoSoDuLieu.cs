using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLyBanVeXeKhach.Data
{
    // Lớp này chịu trách nhiệm duy nhất là Tương tác với CSDL. (Không chứa giao diện MessageBox)
    // Cấu trúc viết rất cơ bản, có thêm SqlParameter để sinh viên tập làm quen với code an toàn.
    public class KetNoiCoSoDuLieu
    {
        // TẠM THỜI để chuỗi kết nối ở đây cho sinh viên dễ hiểu luồng chạy.
        // (Lưu ý: Khi làm dự án thực tế lớn hơn, hãy chuyển chuỗi này ra file cấu hình)
        private readonly string chuoiKetNoi = @"Data Source=.;Initial Catalog=QuanLyBanVeXeKhach;Integrated Security=True;TrustServerCertificate=True;";

        // 1. Hàm đọc dữ liệu (SELECT) - Có tham số (SqlParameter) để chống Hack (SQL Injection)
        // Mảng parameters cho phép truyền các giá trị an toàn thay vì cộng chuỗi.
        // Gán "= null" nghĩa là nếu không truyền tham số nào thì mặc định là null (không bắt buộc phải có).
        public DataTable DocDuLieu(string cauLenhSQL, SqlParameter[] parameters = null)
        {
            DataTable bangDuLieu = new DataTable();
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    using (SqlCommand lenh = new SqlCommand(cauLenhSQL, ketNoi))
                    {
                        // Nếu có truyền tham số vào, thì gắn nó vào câu lệnh SQL
                        if (parameters != null)
                        {
                            lenh.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter boChuyenDoi = new SqlDataAdapter(lenh))
                        {
                            boChuyenDoi.Fill(bangDuLieu);
                        }
                    }
                }
            }
            catch (Exception loi)
            {
                // NÉM LỖI lên trên. Tầng giao diện (Ví dụ: ViewModel, Form) gọi hàm này
                // sẽ tự dùng Try-Catch của riêng nó để bắt lỗi và quyết định cách hiển thị.
                throw new Exception("Lỗi khi lấy dữ liệu: " + loi.Message);
            }
            return bangDuLieu;
        }

        // 2. Hàm thực thi lệnh Thêm, Sửa, Xóa (INSERT, UPDATE, DELETE) - Có tham số
        public int ThucThiCauLenh(string cauLenhSQL, SqlParameter[] parameters = null)
        {
            int soDongAnhHuong = 0;
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    using (SqlCommand lenh = new SqlCommand(cauLenhSQL, ketNoi))
                    {
                        if (parameters != null)
                        {
                            lenh.Parameters.AddRange(parameters);
                        }
                        
                        soDongAnhHuong = lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception loi)
            {
                throw new Exception("Lỗi khi cập nhật dữ liệu: " + loi.Message);
            }
            return soDongAnhHuong;
        }

        // 3. Hàm wrapper ngắn gọn, trả về True/False cho tiện sử dụng
        public bool ThucThiLenh(string cauLenhSQL, SqlParameter[] parameters = null)
        {
            return ThucThiCauLenh(cauLenhSQL, parameters) > 0;
        }
    }
}
