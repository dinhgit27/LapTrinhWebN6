namespace FashionEcommerce.DTOs
{
    /// <summary>
    /// DTO để đọc/trả về thông tin danh mục sản phẩm
    /// </summary>
    public class CategoryReadDto
    {
        /// <summary>
        /// ID danh mục
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tên danh mục
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Slug URL-friendly
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// ID của danh mục cha
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Tên danh mục cha (nếu có)
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// Danh mục có đang hoạt động không
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Số lượng sản phẩm trong danh mục này
        /// </summary>
        public int ProductCount { get; set; }

        /// <summary>
        /// Số lượng danh mục con
        /// </summary>
        public int ChildrenCount { get; set; }
    }
}
