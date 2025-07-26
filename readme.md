# ECommerce - Dự án .NET MVC

Project TMĐT đơn giản được xây dựng bằng ASP.NET MVC.

---

## 🚀 Yêu cầu hệ thống

Trước khi bắt đầu, cần chuẩn bị:

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (cài workload **ASP.NET and web development**)
- SQL Server (hoặc SQL Server Express)
- Git
- Cơ sở dữ liệu dùng MySQL (nên dùng Laragon cho tiện)

---

## 📥 Clone dự án

Mở terminal hoặc Git Bash và chạy:

```bash
git clone https://github.com/TrHgTung/ECommerce.git
cd ECommerce
```

## Cấu hình để chạy dev test

- Tạo cơ sở dữ liệu trước: ví dụ tên ecommerce_db
- Kêt nối CSDL thông qua chuỗi kết nối trong appsettings.json
- Chạy lệnh migration: Tự tìm hiểu ở `https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli`
