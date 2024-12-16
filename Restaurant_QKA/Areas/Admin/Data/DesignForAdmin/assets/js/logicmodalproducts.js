$(document).ready(function () {
    // Khi nhấn vào nút "New Product", modal sẽ hiện ra và load form qua Ajax
    $(document).on('click', '#newProductButton', function (e) {
        e.preventDefault();
        // Hiển thị SweetAlert
        Swal.fire({
            title: 'Bạn có chắc chắn?',
            text: "Bạn muốn thêm mới món?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalProductLabel').text('Loading...');
                $('#modalProductContent').html('<p>Loading content...</p>');
                $.ajax({
                    url: '/Admin/MenuItem/Create', // URL đến action Create trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modalProductLabel').text('Create Product');
                        $('#modalProductContent').html(data); // Load nội dung form vào modal
                        $('#ProductModal').modal('show'); // Hiển thị modal
                        bindCreateForm() // Gọi hàm bind lại sự kiện submit form
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Lỗi!',
                            text: 'Lỗi khi tải trang.'
                        });
                    }
                });
            }
            else {
                // Nếu người dùng chọn No
                Swal.fire('Hủy!', 'Không có gì thay đổi.', 'warning');
            }
        });
    });

        // Khi nhấn vào nút "Delete Product", modal sẽ hiện ra và load form qua Ajax
    $(document).on('click', '#deleteProductButton', function (e)
    {
            e.preventDefault();
            var productId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id
            // Hiển thị SweetAlert
            Swal.fire({
                title: 'Bạn có chắc chắn?',
                text: "Bạn muốn xóa món này?",
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Có',
                cancelButtonText: 'Không'
            }).then((result) => {
                if (result.isConfirmed) {
                        // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                        $('#modalProductLabel').text('Loading...');
                        $('#modalProductContent').html('<p>Loading content...</p>');
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalProductLabel').text('Loading...');
                $('#modalProductContent').html('<p>Loading content...</p>');

                // Gọi Ajax để tải form xóa sản phẩm
                $.ajax({
                    url: '/Admin/MenuItem/Delete/' + productId, // URL đến action Delete trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modalProductLabel').text('Delete Product'); // Cập nhật tiêu đề modal
                        $('#modalProductContent').html(data); // Load nội dung form vào modal
                        $('#ProductModal').modal('show'); // Hiển thị modal
                        bindDeleteForm(); // Bind lại sự kiện submit form cho form xóa
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Lỗi!',
                            text: 'Lỗi khi tải trang.'
                        });
                    }
                });
                }
                else {
                    // Nếu người dùng chọn No
                    Swal.fire('Hủy!', 'Không có gì thay đổi.', 'warning');
                }
            });
        });

        // Khi nhấn vào nút "Edit Product", modal sẽ hiện ra và load form qua Ajax
    $(document).on('click', '#editProductButton', function (e)
    {
            e.preventDefault();
            var productId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id
            // Hiển thị SweetAlert
            Swal.fire({
                title: 'Bạn có chắc chắn?',
                text: "Bạn có muốn chỉnh sửa món này",
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Có',
                cancelButtonText: 'Không'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                    $('#modalProductLabel').text('Loading...');
                    $('#modalProductContent').html('<p>Loading content...</p>');
            // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
            $('#modalProductLabel').text('Loading...');
            $('#modalProductContent').html('<p>Loading content...</p>');

            // Gọi Ajax để tải form xóa sản phẩm
            $.ajax({
                url: '/Admin/MenuItem/Edit/' + productId, // URL đến action Edit trong controller
                type: 'GET',
                success: function (data) {
                    $('#modalProductLabel').text('Edit Product'); // Cập nhật tiêu đề modal
                    $('#modalProductContent').html(data); // Load nội dung form vào modal
                    $('#ProductModal').modal('show'); // Hiển thị modal
                    bindEditForm(); // Bind lại sự kiện submit form cho form 
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi!',
                        text: 'Lỗi khi tải trang.'
                    });
                }
            });
                }
                else {
                    // Nếu người dùng chọn No
                    Swal.fire('Cancelled!', 'No changes were made.', 'warning');
                }
            });
    });

        // Hàm bind sự kiện submit cho form Create 
        function bindCreateForm() {
            $('#createProductForm').off('submit').on('submit', function (event) {
                event.preventDefault(); // Ngăn form submit mặc định
                var form = $(this);

                $.ajax({
                    url: form.attr('action'), // URL của action Create
                    type: form.attr('method'), // Phương thức POST
                    data: new FormData(this), // Sử dụng FormData để gửi dữ liệu form
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: 'success',
                                title: "Thành Công!",
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    $('#ProductModal').modal('hide');
                                    location.reload();
                                }
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Thất Bại!',
                                text: 'Hãy kiểm tra lại thông tin.'
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Lỗi!',
                            text: 'Có lỗi trong quá trình xử lý. Vui lòng thử lại.'
                        });
                    }
                });
            });
        }

        // Hàm bind sự kiện submit cho form Delete 
        function bindDeleteForm() {
            $('#deleteProductForm').off('submit').on('submit', function (event) {
                event.preventDefault();
                var form = $(this);

                $.ajax({
                    url: form.attr('action'),
                    type: form.attr('method'),
                    data: new FormData(this),
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: 'success',
                                title: "Product Deleted!",
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    $('#ProductModal').modal('hide');
                                    location.reload();
                                }
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Xóa Không Thành Công!',
                                text: 'Không Tìm Thấy Sản Phẩm.'
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Lỗi!',
                            text: 'Có Lỗi Xảy Ra Trong Quá Trình Xóa.'
                        });
                    }
                });
            });
        }

        // Hàm bind sự kiện submit cho form Edit 
        function bindEditForm() {
            $('#editProductForm').off('submit').on('submit', function (event) {
                event.preventDefault(); // Ngăn form submit mặc định
                var form = $(this);

                $.ajax({
                    url: form.attr('action'), // URL của action EDIT
                    type: form.attr('method'), // Phương thức POST
                    data: new FormData(this), // Sử dụng FormData để gửi dữ liệu form
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: 'success',
                                title: "Product Updated!",
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    $('#ProductModal').modal('hide');
                                    location.reload();
                                }
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Cập Nhật Không Thành Công!',
                                text: 'Hãy kiểm tra lại thông tin.'
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Lỗi!',
                            text: 'Có lỗi trong quá trình xử lý. Vui lòng thử lại.'
                        });
                    }
                });
            });
        }
    });