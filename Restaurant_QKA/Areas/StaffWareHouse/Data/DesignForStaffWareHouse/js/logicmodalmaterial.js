function enableEdit() {
    document.getElementById("editbutton").style.display = 'none';
    document.getElementById("cancelbutton").style.display = 'block';
    document.getElementById("labelaction").style.display = 'block';

    var inputs = document.querySelectorAll('#contentaction');
    inputs.forEach(function (input) {
        input.style.display = 'block';
    });
}
function disableEdit() {
    document.getElementById("editbutton").style.display = 'block';
    document.getElementById("cancelbutton").style.display = 'none';
    document.getElementById("labelaction").style.display = 'none';

    var inputs = document.querySelectorAll('#contentaction');
    inputs.forEach(function (input) {
        input.style.display = 'none';
    });
}
$(document).ready(function () {
    // Thêm
    $(document).on('click', '#newMaterialButton', function (e) {
        e.preventDefault();
        // Hiển thị SweetAlert
        Swal.fire({
            title: 'Bạn có chắc chắn?',
            text: "Bạn muốn thêm nguyên liệu?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Có',
            cancelButtonText: 'Không'
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalMaterialLabel').text('Loading...');
                $('#modalMaterialContent').html('<p>Loading content...</p>');
                $.ajax({
                    url: '/StaffWareHouse/HomeWareHouse/Create', // URL đến action Create trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modalMaterialLabel').text('Thêm Mới Nguyên Liệu');
                        $('#modalMaterialContent').html(data); // Load nội dung form vào modal
                        $('#MaterialModal').modal('show'); // Hiển thị modal
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
    function bindCreateForm() {
        $('#createMaterialForm').off('submit').on('submit', function (event) {
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
                                $('#MaterialModal').modal('hide');
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
    // Sửa
    $(document).on('click', '#editMaterialButton', function (e) {
        e.preventDefault();
        var MaterialId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id
        // Hiển thị SweetAlert
        Swal.fire({
            title: 'Bạn có chắc chắn?',
            text: "Bạn có muốn chỉnh sửa nguyên liệu này",
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Có',
            cancelButtonText: 'Không'
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalMaterialLabel').text('Loading...');
                $('#modalMaterialContent').html('<p>Loading content...</p>');
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalMaterialLabel').text('Loading...');
                $('#modalMaterialContent').html('<p>Loading content...</p>');

                // Gọi Ajax để tải form xóa sản phẩm
                $.ajax({
                    url: '/StaffWareHouse/HomeWareHouse/Edit/' + MaterialId, // URL đến action Edit trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modalMaterialLabel').text('Thay Đổi Thông Tin Nguyên Liệu'); // Cập nhật tiêu đề modal
                        $('#modalMaterialContent').html(data); // Load nội dung form vào modal
                        $('#MaterialModal').modal('show'); // Hiển thị modal
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
                Swal.fire('Hủy!', 'Không có gì thay đổi.', 'warning');
            }
        });
    });
    function bindEditForm() {
        $('#editMaterialForm').off('submit').on('submit', function (event) {
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
                            title: "Thông Tin Đã Được Cập Nhật!",
                            confirmButtonText: 'OK'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                $('#MaterialModal').modal('hide');
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

    // Xóa
    $(document).on('click', '#deleteMaterialButton', function (e) {
        e.preventDefault();
        var MaterialId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id
        // Hiển thị SweetAlert
        Swal.fire({
            title: 'Bạn có chắc chắn?',
            text: "Bạn muốn xóa nguyên liệu này?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Có',
            cancelButtonText: 'Không'
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalMaterialLabel').text('Loading...');
                $('#modalMaterialContent').html('<p>Loading content...</p>');
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalMaterialLabel').text('Loading...');
                $('#modalMaterialContent').html('<p>Loading content...</p>');

                // Gọi Ajax để tải form xóa sản phẩm
                $.ajax({
                    url: '/StaffWareHouse/HomeWareHouse/Delete/' + MaterialId, // URL đến action Delete trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modalMaterialLabel').text('Xóa Nguyên Liệu'); // Cập nhật tiêu đề modal
                        $('#modalMaterialContent').html(data); // Load nội dung form vào modal
                        $('#MaterialModal').modal('show'); // Hiển thị modal
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
    function bindDeleteForm() {
        $('#deleteMaterialForm').off('submit').on('submit', function (event) {
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
                            title: "Nguyên Liệu Đã Được Xóa!",
                            confirmButtonText: 'OK'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                $('#MaterialModal').modal('hide');
                                location.reload();
                            }
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Xóa Không Thành Công!',
                            text: 'Không Tìm Thấy Nguyên Liệu.'
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
});