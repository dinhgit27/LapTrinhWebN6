using Microsoft.EntityFrameworkCore;
using FashionEcommerce.Models;

namespace FashionEcommerce.Data
{
    // DbContext chính của ứng dụng
    // Quản lý kết nối database và ánh xạ các entity models
    // Cấu hình relationships, constraints và indexes
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets cho các entity - đại diện cho các bảng trong database
        public DbSet<User> Users { get; set; }  // Bảng người dùng
        public DbSet<UserAddress> UserAddresses { get; set; }  // Bảng địa chỉ người dùng
        public DbSet<Promotion> Promotions { get; set; }  // Bảng chương trình khuyến mãi
        public DbSet<PromotionCondition> PromotionConditions { get; set; }  // Bảng điều kiện khuyến mãi
        public DbSet<Product> Products { get; set; }  // Bảng sản phẩm
        public DbSet<ProductVariant> ProductVariants { get; set; }  // Bảng biến thể sản phẩm
        public DbSet<ProductReview> ProductReviews { get; set; }  // Bảng đánh giá sản phẩm
        public DbSet<ProductPromotion> ProductPromotions { get; set; }  // Bảng liên kết sản phẩm-khuyến mãi
        public DbSet<ProductImage> ProductImages { get; set; }  // Bảng hình ảnh sản phẩm
        public DbSet<Order> Orders { get; set; }  // Bảng đơn hàng
        public DbSet<OrderDetail> OrderDetails { get; set; }  // Bảng chi tiết đơn hàng
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }  // Bảng lịch sử trạng thái đơn hàng
        public DbSet<Notification> Notifications { get; set; }  // Bảng thông báo
        public DbSet<Coupon> Coupons { get; set; }  // Bảng mã giảm giá
        public DbSet<Category> Categories { get; set; }  // Bảng danh mục sản phẩm
        public DbSet<CartItem> CartItems { get; set; }  // Bảng sản phẩm trong giỏ hàng
        public DbSet<Article> Articles { get; set; }  // Bảng bài viết
        public DbSet<ArticleCategory> ArticleCategories { get; set; }  // Bảng danh mục bài viết
        public DbSet<MasterSize> MasterSizes { get; set; }  // Bảng master kích thước
        public DbSet<MasterColor> MasterColors { get; set; }  // Bảng master màu sắc

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình unique indexes - đảm bảo tính duy nhất
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();  // Email người dùng duy nhất
            modelBuilder.Entity<Product>().HasIndex(p => p.Slug).IsUnique();  // Slug sản phẩm duy nhất
            modelBuilder.Entity<ProductVariant>().HasIndex(pv => pv.Sku).IsUnique();  // SKU duy nhất
            modelBuilder.Entity<Order>().HasIndex(o => o.OrderCode).IsUnique();  // Mã đơn hàng duy nhất
            modelBuilder.Entity<Coupon>().HasIndex(c => c.Code).IsUnique();  // Mã coupon duy nhất
            modelBuilder.Entity<Category>().HasIndex(c => c.Slug).IsUnique();  // Slug danh mục duy nhất
            modelBuilder.Entity<Article>().HasIndex(a => a.Slug).IsUnique();  // Slug bài viết duy nhất
            modelBuilder.Entity<ArticleCategory>().HasIndex(ac => ac.Slug).IsUnique();  // Slug danh mục bài viết duy nhất

            // Cấu hình check constraints - ràng buộc kiểm tra
            modelBuilder.Entity<ProductReview>().HasCheckConstraint("CK_ProductReview_Rating", "[Rating] >= 1 AND [Rating] <= 5");  // Rating từ 1-5
            modelBuilder.Entity<Promotion>().HasCheckConstraint("CK_Promotion_DiscountType", "[DiscountType] IN ('FIXED_AMOUNT', 'PERCENTAGE')");  // Loại giảm giá hợp lệ

            // Cấu hình self-referencing relationship cho Categories - quan hệ đệ quy
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);  // Không cascade delete để tránh xóa nhầm

            // Cấu hình cascade deletes - xóa theo cascade khi cần thiết
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa user thì xóa cart items

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa user thì xóa notifications

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa order thì xóa order details

            modelBuilder.Entity<OrderStatusHistory>()
                .HasOne(osh => osh.Order)
                .WithMany(o => o.OrderStatusHistories)
                .HasForeignKey(osh => osh.OrderId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa order thì xóa status history

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa product thì xóa images

            modelBuilder.Entity<ProductPromotion>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.ProductPromotions)
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa product thì xóa promotions liên quan

            modelBuilder.Entity<ProductPromotion>()
                .HasOne(pp => pp.Promotion)
                .WithMany(p => p.ProductPromotions)
                .HasForeignKey(pp => pp.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa promotion thì xóa product promotions

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa product thì xóa variants

            modelBuilder.Entity<PromotionCondition>()
                .HasOne(pc => pc.Promotion)
                .WithMany(p => p.PromotionConditions)
                .HasForeignKey(pc => pc.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa promotion thì xóa conditions

            modelBuilder.Entity<UserAddress>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAddresses)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Xóa user thì xóa addresses
        }
    }
}