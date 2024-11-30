$(document).ready(function () {
    // Khi nhấn vào nút "New Product", modal sẽ hiện ra và load form qua Ajax
    $('#newProductButton').on('click', function () {
        $.ajax({
            url: '/MenuItem/Create', // URL đến action Create trong controller
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
                    title: 'Error!',
                    text: 'Failed to load create form.'
                });
            }
        });
    });

    // Khi nhấn vào nút "Delete Product", modal sẽ hiện ra và load form qua Ajax
    $(document).on('click', '#deleteProductButton', function () {
        var productId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id

        // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
        $('#modalProductLabel').text('Loading...');
        $('#modalProductContent').html('<p>Loading content...</p>');

        // Gọi Ajax để tải form xóa sản phẩm
        $.ajax({
            url: '/MenuItem/Delete/' + productId, // URL đến action Delete trong controller
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
                    title: 'Error!',
                    text: 'Failed to load delete form.'
                });
            }
        });
    });

    // Khi nhấn vào nút "Edit Product", modal sẽ hiện ra và load form qua Ajax
    $(document).on('click', '#editProductButton', function () {
        var productId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id

        // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
        $('#modalProductLabel').text('Loading...');
        $('#modalProductContent').html('<p>Loading content...</p>');

        // Gọi Ajax để tải form xóa sản phẩm
        $.ajax({
            url: '/MenuItem/Edit/' + productId, // URL đến action Edit trong controller
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
                    title: 'Error!',
                    text: 'Failed to load edit form.'
                });
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
                            title: "Product Created!",
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
                            title: 'Create Failed!',
                            text: 'Please check your input and try again.'
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: 'An error occurred while submitting the form.'
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
                            title: 'Delete Failed!',
                            text: 'Product could not be found.'
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: 'An error occurred during deletion.'
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
                            title: 'Update Failed!',
                            text: 'Please check your input and try again.'
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: 'An error occurred while submitting the form.'
                    });
                }
            });
        });
    }

});