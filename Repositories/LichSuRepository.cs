using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace QuanLyBanVeXeKhach.Repositories
{
    public class LichSuRepository
    {
        public List<LichSuGiaoDich> GetAll()
        {
            var list = new List<LichSuGiaoDich>();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT nk.*, nv.HoTen AS TenNhanVien 
                    FROM NhatKyHeThong nk
                    LEFT JOIN NhanVien nv ON nk.MaNV = nv.MaNV
                    ORDER BY nk.ThoiGian DESC";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LichSuGiaoDich
                        {
                            MaNhatKy = (int)reader["MaNhatKy"],
                            MaVe = reader["MaVe"] as int?,
                            MaNV = reader["MaNV"] as int?,
                            HanhDong = reader["HanhDong"]?.ToString() ?? "",
                            ThoiGian = (DateTime)reader["ThoiGian"],
                            GhiChu = reader["GhiChu"]?.ToString() ?? "",
                            TenNhanVien = reader["TenNhanVien"]?.ToString() ?? "Hệ thống"
                        });
                    }
                }
            }
            return list;
        }
    }
}
