$(document).ready(function () {
    // Khi nhấn vào nút "Edit Coupon", modal sẽ hiện ra và load form qua Ajax
    $(document).on('click', '.editCoupontButton', function (e) {
        e.preventDefault();
        var cusId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id

        // Hiển thị SweetAlert
        Swal.fire({
            title: 'Are you sure?',
            text: "Do you want to edit this coupont?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modaCoupontLabel').text('Loading...');
                $('#modalCoupontContent').html('<p>Loading content...</p>');
                // Gọi Ajax để tải form resetpass sản phẩm
                // Gọi Ajax để tải form xóa sản phẩm
                $.ajax({
                    url: '/Admin/Coupon/Edit/' + cusId, // URL đến action Edit trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modaCoupontLabel').text('Edit Coupont'); // Cập nhật tiêu đề modal
                        $('#modalCoupontContent').html(data); // Load nội dung form vào modal
                        $('#CoupontModal').modal('show'); // Hiển thị modal

                        // Thêm sự kiện submit vào form trong modal để xóa thông tin người dùng
                        $('#modalCoupontContent Form').off('submit').on('submit', function (event) {
                            event.preventDefault();
                            var form = $(this);

                            // Hiển thị hộp thoại xác nhận trước khi edit
                            Swal.fire({
                                icon: 'warning',
                                title: 'Are you sure?',
                                text: "Do you really want to edit this coupont?",
                                showCancelButton: true,
                                confirmButtonColor: '#3085d6',
                                cancelButtonColor: '#d33',
                                confirmButtonText: 'Yes, continue edit!',
                                cancelButtonText: 'Cancel'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    // Nếu người dùng xác nhận, tiến hành gửi AJAX
                                    $.ajax({
                                        url: form.attr('action'), // URL của action Edit
                                        type: form.attr('method'), // Phương thức POST
                                        data: new FormData(this), // Sử dụng FormData để gửi dữ liệu form
                                        contentType: false,
                                        processData: false,
                                        success: function (response) {
                                            if (response.success) {
                                                Swal.fire({
                                                    icon: 'success',
                                                    title: "Coupont Updated!",
                                                    confirmButtonText: 'OK'
                                                }).then((result) => {
                                                    if (result.isConfirmed) {
                                                        $('#CoupontModal').modal('hide');
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
                                }
                            });
                        });

                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error!',
                            text: 'Failed to load edit form.'
                        });
                    }
                });

            } else {
                // Nếu người dùng chọn No
                Swal.fire('Cancelled!', 'No changes were made.', 'warning');
            }
        });
    });
    // Khi nhấn vào nút "Delete Coupont", modal sẽ hiện ra và load form qua Ajax
    $(document).on('click', '.deleteCoupontButton', function (e) {
        e.preventDefault();
        var cusId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id

        // Hiển thị SweetAlert
        Swal.fire({
            title: 'Are you sure?',
            text: "Do you want to delete this coupont?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modaCoupontLabel').text('Loading...');
                $('#modalCoupontContent').html('<p>Loading content...</p>');
                // Gọi Ajax để tải form resetpass sản phẩm
                // Gọi Ajax để tải form xóa sản phẩm
                $.ajax({
                    url: '/Admin/Coupon/Delete/' + cusId, // URL đến action Delete trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modaCoupontLabel').text('Delete Coupont'); // Cập nhật tiêu đề modal
                        $('#modalCoupontContent').html(data); // Load nội dung form vào modal
                        $('#CoupontModal').modal('show'); // Hiển thị modal

                        // Thêm sự kiện submit vào form trong modal để xóa thông tin người dùng
                        $('#modalCoupontContent form').off('submit').on('submit', function (event) {
                            event.preventDefault();
                            var form = $(this);

                            // Hiển thị hộp thoại xác nhận trước khi xóa
                            Swal.fire({
                                icon: 'warning',
                                title: 'Are you sure?',
                                text: "Do you really want to delete this coupont?",
                                showCancelButton: true,
                                confirmButtonColor: '#3085d6',
                                cancelButtonColor: '#d33',
                                confirmButtonText: 'Yes, delete it!',
                                cancelButtonText: 'Cancel'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    // Nếu người dùng xác nhận, tiến hành gửi AJAX
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
                                                    title: "Coupont Deleted!",
                                                    confirmButtonText: 'OK'
                                                }).then((result) => {
                                                    if (result.isConfirmed) {
                                                        $('#CoupontModal').modal('hide');
                                                        location.reload();
                                                    }
                                                });
                                            } else {
                                                Swal.fire({
                                                    icon: 'error',
                                                    title: 'Delete Failed!',
                                                    text: 'Coupon could not be found.'
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
                                }
                            });
                        });

                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error!',
                            text: 'Failed to load delete form.'
                        });
                    }
                });

            } else {
                // Nếu người dùng chọn No
                Swal.fire('Cancelled!', 'No changes were made.', 'warning');
            }
        });
    });
    // Khi nhấn vào nút "New Coupon", modal sẽ hiện ra và load form qua Ajax
    $(document).on('click', '.newCoupontButton', function (e) {
        e.preventDefault();

        // Hiển thị SweetAlert
        Swal.fire({
            title: 'Are you sure?',
            text: "bạn có muốn thêm mới coupon?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modaCoupontLabel').text('Loading...');
                $('#modalCoupontContent').html('<p>Loading content...</p>');
                // Gọi Ajax để tải form resetpass sản phẩm
                // Gọi Ajax để tải form xóa sản phẩm
                $.ajax({
                    url: '/Admin/Coupon/Create', // URL đến action Create trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modaCoupontLabel').text('Create Coupont');
                        $('#modalCoupontContent').html(data); // Load nội dung form vào modal
                        $('#CoupontModal').modal('show'); // Hiển thị modal
                        bindCreateForm() // Gọi hàm bind lại sự kiện submit form

                        // Thêm sự kiện submit vào form trong modal để xóa thông tin người dùng
                        $('#createCoupontForm').off('submit').on('submit', function (event) {
                            event.preventDefault();
                            var form = $(this);

                            // Hiển thị hộp thoại xác nhận trước khi create
                            Swal.fire({
                                icon: 'warning',
                                title: 'Tiếp Tục?',
                                showCancelButton: true,
                                confirmButtonColor: '#3085d6',
                                cancelButtonColor: '#d33',
                                confirmButtonText: 'Yes, continue create!',
                                cancelButtonText: 'Cancel'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    // Nếu người dùng xác nhận, tiến hành gửi AJAX
                                    $.ajax({
                                        url: form.attr('action'), // URL của action Edit
                                        type: form.attr('method'), // Phương thức POST
                                        data: new FormData(this), // Sử dụng FormData để gửi dữ liệu form
                                        contentType: false,
                                        processData: false,
                                        success: function (response) {
                                            if (response.success) {
                                                Swal.fire({
                                                    icon: 'success',
                                                    title: "Coupont Created!",
                                                    confirmButtonText: 'OK'
                                                }).then((result) => {
                                                    if (result.isConfirmed) {
                                                        $('#CoupontModal').modal('hide');
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
                                }
                            });
                        });

                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error!',
                            text: 'Failed to load create form.'
                        });
                    }
                });

            } else {
                // Nếu người dùng chọn No
                Swal.fire('Cancelled!', 'No changes were made.', 'warning');
            }
        });
    });
    // Hàm bind sự kiện submit cho form Create 
    function bindCreateForm() {
        $('#createCouponForm').off('submit').on('submit', function (event) {
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
                                $('#CouponModal').modal('hide');
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
});