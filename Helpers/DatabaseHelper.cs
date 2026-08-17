using Microsoft.Data.SqlClient;

namespace QuanLyBanVeXeKhach.Helpers
{
    public static class DatabaseHelper
    {
        private static string _serverName = ".";
        
        public static string GetServerName()
        {
            try
            {
                if (System.IO.File.Exists("dbconfig.txt"))
                {
                    _serverName = System.IO.File.ReadAllText("dbconfig.txt").Trim();
                }
                else
                {
                    System.IO.File.WriteAllText("dbconfig.txt", _serverName);
                }
            }
            catch { }
            if (string.IsNullOrEmpty(_serverName)) _serverName = ".";
            return _serverName;
        }

        public static void SetServerName(string serverName)
        {
            _serverName = serverName;
            try { System.IO.File.WriteAllText("dbconfig.txt", _serverName); } catch { }
        }

        public static string ConnectionString =>
            $"Data Source={GetServerName()};Initial Catalog=QuanLyBanVeXeKhach;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static void RunMigration()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    var checkCmd = new Microsoft.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM sys.tables WHERE name='BenXe'", conn);
                    if ((int)checkCmd.ExecuteScalar() == 0)
                    {
                        string sql = @"
                            CREATE TABLE BenXe (
                                MaBenXe INT IDENTITY(1,1) PRIMARY KEY,
                                TenBenXe NVARCHAR(200) NOT NULL,
                                DiaChi NVARCHAR(500),
                                TinhThanh NVARCHAR(100),
                                TrangThai BIT DEFAULT 1
                            );

                            INSERT INTO BenXe (TenBenXe, TinhThanh) VALUES
                            (N'Bến xe Miền Đông', N'TP.HCM'),
                            (N'Bến xe Miền Tây', N'TP.HCM'),
                            (N'Bến xe An Sương', N'TP.HCM'),
                            (N'Bến xe Liên tỉnh Đà Lạt', N'Lâm Đồng'),
                            (N'Bến xe Vũng Tàu', N'Bà Rịa - Vũng Tàu'),
                            (N'Bến xe Tây Ninh', N'Tây Ninh');

                            ALTER TABLE TuyenXe ADD MaBenDi INT;
                            ALTER TABLE TuyenXe ADD MaBenDen INT;

                            EXEC('UPDATE TuyenXe SET MaBenDi = 1 WHERE DiemDi LIKE N''%HCM%''');
                            EXEC('UPDATE TuyenXe SET MaBenDen = 4 WHERE DiemDen LIKE N''%Đà Lạt%''');
                            EXEC('UPDATE TuyenXe SET MaBenDen = 5 WHERE DiemDen LIKE N''%Vũng Tàu%''');
                            EXEC('UPDATE TuyenXe SET MaBenDi = 1 WHERE MaBenDi IS NULL');
                            EXEC('UPDATE TuyenXe SET MaBenDen = 1 WHERE MaBenDen IS NULL');

                            ALTER TABLE TuyenXe DROP COLUMN DiemDi;
                            ALTER TABLE TuyenXe DROP COLUMN DiemDen;

                            ALTER TABLE TuyenXe ADD CONSTRAINT FK_TuyenXe_BenDi FOREIGN KEY (MaBenDi) REFERENCES BenXe(MaBenXe);
                            ALTER TABLE TuyenXe ADD CONSTRAINT FK_TuyenXe_BenDen FOREIGN KEY (MaBenDen) REFERENCES BenXe(MaBenXe);

                            DROP PROC sp_ThemTuyenXe;
                            EXEC('
                            CREATE PROC sp_ThemTuyenXe
                                @TenTuyen NVARCHAR(200),
                                @MaBenDi   INT,
                                @MaBenDen  INT,
                                @KhoangCach DECIMAL(10,1),
                                @ThoiGianDiChuyen NVARCHAR(50)
                            AS
                            BEGIN
                                INSERT INTO TuyenXe (TenTuyen, MaBenDi, MaBenDen, KhoangCach, ThoiGianDiChuyen)
                                VALUES (@TenTuyen, @MaBenDi, @MaBenDen, @KhoangCach, @ThoiGianDiChuyen);
                                SELECT SCOPE_IDENTITY() AS MaTuyen;
                            END
                            ');
                        ";
                        new Microsoft.Data.SqlClient.SqlCommand(sql, conn).ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        public static bool TestConnection()
        {
            try { using var c = GetConnection(); c.Open(); return true; }
            catch { return false; }
        }

        public static int ExecuteNonQuery(string sql, SqlParameter[]? parameters = null)
        {
            try
            {
                using var conn = GetConnection(); conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
            catch { return 0; }
        }

        public static object? ExecuteScalar(string sql, SqlParameter[]? parameters = null)
        {
            try
            {
                using var conn = GetConnection(); conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
            catch { return null; }
        }

        public static List<T> ExecuteQuery<T>(string sql, Func<SqlDataReader, T> map, SqlParameter[]? parameters = null)
        {
            var list = new List<T>();
            try
            {
                using var conn = GetConnection(); conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try { list.Add(map(reader)); } catch { }
                }
            }
            catch { }
            return list;
        }

        public static T? ExecuteQuerySingle<T>(string sql, Func<SqlDataReader, T> map, SqlParameter[]? parameters = null) where T : class
        {
            try
            {
                using var conn = GetConnection(); conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    try { return map(reader); } catch(Exception ex) { System.Windows.MessageBox.Show("Lỗi Map: " + ex.Message); return null; }
                }
                return null;
            }
            catch (Exception ex) 
            { 
                System.Windows.MessageBox.Show("Lỗi CSDL: " + ex.Message, "Lỗi kết nối", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error); 
                return null; 
            }
        }

        // ── Safe reader helpers ──────────────────────────────────────
        public static string SafeString(SqlDataReader r, string col)
        {
            try { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? "" : r.GetString(i); }
            catch { return ""; }
        }
        public static int SafeInt(SqlDataReader r, string col)
        {
            try { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? 0 : r.GetInt32(i); }
            catch { return 0; }
        }
        public static decimal SafeDecimal(SqlDataReader r, string col)
        {
            try { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? 0m : r.GetDecimal(i); }
            catch { return 0m; }
        }
        public static bool SafeBool(SqlDataReader r, string col)
        {
            try { int i = r.GetOrdinal(col); return !r.IsDBNull(i) && r.GetBoolean(i); }
            catch { return false; }
        }
        public static DateTime SafeDateTime(SqlDataReader r, string col)
        {
            try { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? DateTime.MinValue : r.GetDateTime(i); }
            catch { return DateTime.MinValue; }
        }
        public static DateTime? SafeDateTimeNull(SqlDataReader r, string col)
        {
            try { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? null : r.GetDateTime(i); }
            catch { return null; }
        }
        public static int? SafeIntNull(SqlDataReader r, string col)
        {
            try { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? null : r.GetInt32(i); }
            catch { return null; }
        }
        public static decimal? SafeDecimalNull(SqlDataReader r, string col)
        {
            try { int i = r.GetOrdinal(col); return r.IsDBNull(i) ? null : r.GetDecimal(i); }
            catch { return null; }
        }
    }
}
