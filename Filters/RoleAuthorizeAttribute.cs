// Họ và tên: Nguyễn Duy Trung
// Mã sinh viên: 23103100256
// Nội dung thực hiện: Module 1 - Kiểm tra đăng nhập và phân quyền tại Controller bằng Session

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyKhoaHoc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var maTaiKhoan = session.GetInt32("MaTaiKhoan");
            var vaiTro = session.GetString("VaiTro");

            // 1. Chưa đăng nhập -> Chuyển về trang Login
            if (maTaiKhoan == null || string.IsNullOrEmpty(vaiTro))
            {
                context.Result = new RedirectToActionResult("Login", "TaiKhoan", null);
                return;
            }

            // 2. Có yêu cầu vai trò cụ thể nhưng người dùng không thuộc vai trò đó -> Chuyển đến trang Từ chối truy cập
            if (_allowedRoles != null && _allowedRoles.Length > 0)
            {
                if (!_allowedRoles.Contains(vaiTro))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "TaiKhoan", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}