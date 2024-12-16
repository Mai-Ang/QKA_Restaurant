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
function enableEditProfile() {
    // Enable all input fields except UserName
    var inputs = document.querySelectorAll('input[type="text"]:not([name="Name"]), input[type="text"]:not([name="UserName"]), input[type="email"], input[type="tel"], input[type="date"]');
    inputs.forEach(function (input) {
        input.disabled = false;
    });

    // Hide edit button and show save button
    document.getElementById('editButton').style.display = 'none';
    document.getElementById('Name').style.display = 'block';
    document.getElementById('saveButton').style.display = 'block';
    document.getElementById('cancelButton').style.display = 'block';
}
function disableEditProfile() {
    // Enable all input fields except UserName
    var inputs = document.querySelectorAll('input[type="text"]:not([name="Name"]), input[type="text"]:not([name="UserName"]), input[type="email"], input[type="tel"], input[type="date"]');
    inputs.forEach(function (input) {
        input.disabled = true;
    });

    // Hide edit button and show save button
    document.getElementById('editButton').style.display = 'block';
    document.getElementById('Name').style.display = 'none';
    document.getElementById('saveButton').style.display = 'none';
    document.getElementById('cancelButton').style.display = 'none';
}
$(document).ready(function () {
    // Thêm
    $(document).on('click', '#newStaffButton', function (e) {
        e.preventDefault();
        // Hiển thị SweetAlert
        Swal.fire({
            title: 'Bạn có chắc chắn?',
            text: "Bạn muốn thêm tài khoản mới?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Có',
            cancelButtonText: 'Không'
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalStaffLabel').text('Loading...');
                $('#modalStaffContent').html('<p>Loading content...</p>');
                $.ajax({
                    url: '/Admin/Staff/Create', // URL đến action Create trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modalStaffLabel').text('Thêm Mới Tài Khoản Nhân Sự');
                        $('#modalStaffContent').html(data); // Load nội dung form vào modal
                        $('#StaffModal').modal('show'); // Hiển thị modal
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
        $('#createStaffForm').off('submit').on('submit', function (event) {
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
                                $('#StaffModal').modal('hide');
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
    $(document).on('click', '#editStaffButton', function (e) {
        e.preventDefault();
        var StaffId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id
        // Hiển thị SweetAlert
        Swal.fire({
            title: 'Bạn có chắc chắn?',
            text: "Bạn có muốn điều chỉnh lương nhân viên này",
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Có',
            cancelButtonText: 'Không'
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalStaffLabel').text('Loading...');
                $('#modalStaffContent').html('<p>Loading content...</p>');
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalStaffLabel').text('Loading...');
                $('#modalStaffContent').html('<p>Loading content...</p>');

                // Gọi Ajax để tải form xóa sản phẩm
                $.ajax({
                    url: '/Admin/Staff/ChangeSalary/' + StaffId, // URL đến action Edit trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modalStaffLabel').text('Điều Chỉnh Lương'); // Cập nhật tiêu đề modal
                        $('#modalStaffContent').html(data); // Load nội dung form vào modal
                        $('#StaffModal').modal('show'); // Hiển thị modal
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
        $('#editStaffForm').off('submit').on('submit', function (event) {
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
                                $('#StaffModal').modal('hide');
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
    $(document).on('click', '#deleteStaffButton', function (e) {
        e.preventDefault();
        var StaffId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id
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
                $('#modalStaffLabel').text('Loading...');
                $('#modalStaffContent').html('<p>Loading content...</p>');
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalStaffLabel').text('Loading...');
                $('#modalStaffContent').html('<p>Loading content...</p>');

                // Gọi Ajax để tải form xóa sản phẩm
                $.ajax({
                    url: '/StaffWareHouse/HomeWareHouse/Delete/' + StaffId, // URL đến action Delete trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modalStaffLabel').text('Xóa Nguyên Liệu'); // Cập nhật tiêu đề modal
                        $('#modalStaffContent').html(data); // Load nội dung form vào modal
                        $('#StaffModal').modal('show'); // Hiển thị modal
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
        $('#deleteStaffForm').off('submit').on('submit', function (event) {
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
                                $('#StaffModal').modal('hide');
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