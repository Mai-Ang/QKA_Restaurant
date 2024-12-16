$(document).ready(function () {
    // Thêm
    $(document).on('click', '#newTransactionButton', function (e) {
        e.preventDefault();
        // Hiển thị SweetAlert
        Swal.fire({
            title: 'Bạn có chắc chắn?',
            text: "Bạn muốn tạo phiếu nhập nguyên liệu mới?",
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Có',
            cancelButtonText: 'Không'
        }).then((result) => {
            if (result.isConfirmed) {
                // Reset lại nội dung modal để tránh dữ liệu cũ hiển thị
                $('#modalTransactionLabel').text('Loading...');
                $('#modalTransactionContent').html('<p>Loading content...</p>');
                $.ajax({
                    url: '/StaffWareHouse/Transaction/Create', // URL đến action Create trong controller
                    type: 'GET',
                    success: function (data) {
                        $('#modalTransactionLabel').text('Thêm Mới Phiếu Nhập Nguyên Liệu');
                        $('#modalTransactionContent').html(data); // Load nội dung form vào modal
                        $('#TransactionModal').modal('show'); // Hiển thị modal
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
        $('#createTransactionForm').off('submit').on('submit', function (event) {
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
                                $('#TransactionModal').modal('hide');
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
})