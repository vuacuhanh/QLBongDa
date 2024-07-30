using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLSB
{
    public class ConnectionStringManager
    {
        public static string GetConnectionString()
        {
            // Thay thế dòng sau bằng chuỗi kết nối thực tế của bạn
            string connectionString = "Data Source=tranan\\sqlexpress;Initial Catalog=QL_SANBONG_TEST1;Integrated Security=True";
            return connectionString;
        }
    }
}
