$(document).ready(function () {

    // Khi nhấn vào nút "Delete User", modal sẽ hiện ra và load form qua Ajax
    $(document).on('click', '#deleteUserButton', function () {
        var cusId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id

        // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
        $('#modalUserLabel').text('Loading...');
        $('#modalUserContent').html('<p>Loading content...</p>');

        // Gọi Ajax để tải form xóa sản phẩm
        $.ajax({
            url: '/Customer/Delete/' + cusId, // URL đến action Delete trong controller
            type: 'GET',
            success: function (data) {
                $('#modalUserLabel').text('Delete User'); // Cập nhật tiêu đề modal
                $('#modalUserContent').html(data); // Load nội dung form vào modal
                $('#UserModal').modal('show'); // Hiển thị modal
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

    // Khi nhấn vào nút "Edit User", modal sẽ hiện ra và load form qua Ajax
    $(document).on('click', '#editUserButton', function () {
        var productId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id

        // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
        $('#modalUserLabel').text('Loading...');
        $('#modalUserContent').html('<p>Loading content...</p>');

        // Gọi Ajax để tải form xóa sản phẩm
        $.ajax({
            url: '/Customer/Edit/' + productId, // URL đến action Edit trong controller
            type: 'GET',
            success: function (data) {
                $('#modalUserLabel').text('Edit Product'); // Cập nhật tiêu đề modal
                $('#modalUserContent').html(data); // Load nội dung form vào modal
                $('#UserModal').modal('show'); // Hiển thị modal
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

    // Hàm bind sự kiện submit cho form Delete 
    function bindDeleteForm() {
        $('#deleteUserForm').off('submit').on('submit', function (event) {
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
                            title: "User Deleted!",
                            confirmButtonText: 'OK'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                $('#UserModal').modal('hide');
                                location.reload();
                            }
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Delete Failed!',
                            text: 'User could not be found.'
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
        $('#editUserForm').off('submit').on('submit', function (event) {
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
                            title: "User Updated!",
                            confirmButtonText: 'OK'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                $('#UserModal').modal('hide');
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