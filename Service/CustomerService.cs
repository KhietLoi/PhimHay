namespace MovieWebsite.Service
{
    public class CustomerService
    {
        //Phát sinh mã khách hàng/người dùng xem phim
        public string GenerateCustomerCode()
        {
            var guid = Guid.NewGuid().ToString().Substring(0, 8);  // Lấy 8 ký tự đầu từ GUID
            return "KH" + guid.ToUpper();  // Thêm "KH" vào đầu mã và chuyển thành chữ hoa
        }
    }
}
