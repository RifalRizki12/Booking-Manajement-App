$(document).ready(function () {
    var table = $('#tableEmployee').DataTable({
        // dom: 'Bfrtip',
        buttons: [
            {
                extend: 'copy',
                text: 'Copy',
                className: 'btn btn-dark',
                exportOptions: {
                    columns: ':visible:not(.no-export)'
                }
            },
            {
                extend: 'excel',
                text: 'Export to Excel',
                className: 'btn btn-success',
                exportOptions: {
                    columns: ':visible:not(.no-export)'
                },
            },
            {
                extend: 'pdf',
                text: 'Export to PDF',
                className: 'btn btn-primary',
                exportOptions: {
                    columns: ':visible:not(.no-export)'
                },
                customize: function (doc) {
                    doc.pageOrientation = 'landscape';
                    doc.pageSize = 'A3';
                }
            },
            {
                extend: 'print',
                text: 'Print',
                className: 'btn btn-info',
                exportOptions: {
                    columns: ':visible:not(.no-export)'
                },
                customize: function (win) {
                    $(win.document.body).css('font-size', '12px');
                    $(win.document.body).find('table').addClass('compact').css('font-size', 'inherit');
                }
            },
            {
                extend: 'colvis',
                postfixButtons: ['colvisRestore']
            }
        ],
        scrollX: true,
        columnDefs: [
            {
                visible: false
            }
        ],
        ajax: {
            url: "https://localhost:7179/api/Employee",
            dataSrc: "data",
            dataType: "JSON"
        },
        columns: [
            {
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + 1;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return row.firstName + ' ' + row.lastName;
                }
            },
            {
                data: "gender",
                render: function (data) {
                    return data === 0 ? "Male" : "Female";
                }
            },
            { data: "birthDate" },
            { data: "email" },
            { data: "phoneNumber" },
            { data: "hiringDate" },
            {
                data: null,
                render: function (data, type, row) {
                    return `<button type="button" class="btn btn-warning edit-button" data-guid="${row.guid}">Edit</button>
                            <button type="button" class="btn btn-danger delete-button" data-guid="${row.guid}">-</button>`;
                }
            },
        ]
    });
    // Mengaktifkan tombol Copy sesuai dengan tombol ekstensi DataTable
    $('#custom-copy-button').on('click', function () {
        table.button(0).trigger();
    });
    // Mengaktifkan tombol Excel sesuai dengan tombol ekstensi DataTable
    $('#custom-excel-button').on('click', function () {
        table.button(1).trigger();
    });
    // Mengaktifkan tombol PDF sesuai dengan tombol ekstensi DataTable
    $('#custom-pdf-button').on('click', function () {
        table.button(2).trigger();
    });
    // Mengaktifkan tombol Print sesuai dengan tombol ekstensi DataTable
    $('#custom-print-button').on('click', function () {
        table.button(3).trigger();
    });
    $('#custom-visibility').on('click', function () {
        table.button(4).trigger();
    });


    $('.dt-buttons').removeClass('dt-buttons');

    $(document).on("click", ".delete-button", function () {
        var employeeId = $(this).data("guid");
        $('#modalCenter').modal('hide');
        // Tampilkan SweetAlert konfirmasi
        Swal.fire({
            title: 'Konfirmasi',
            text: 'Anda yakin ingin menghapus data ini?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then((result) => {
            if (result.isConfirmed) {
                // Jika pengguna mengonfirmasi penghapusan
                $.ajax({
                    url: "https://localhost:7179/api/Employee/" + employeeId,
                    type: "DELETE",
                    success: function (data) {
                        $('#modalCenter').modal('hide');
                        // Refresh tabel data employee setelah penghapusan
                        $('#tableEmployee').DataTable().ajax.reload();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Data berhasil dihapus!'
                        });
                    },
                    error: function (xhr) {
                        $('#modalCenter').modal('hide');
                        var errorMessage = xhr.responseJSON ? xhr.responseJSON.message : 'Gagal menghapus data.';
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: errorMessage
                        });
                    }
                });
            }
        });
    });

    $('#saveEmployeeButton').click(function () {
        var firstName = $('#firstNameInput').val();
        var lastName = $('#lastNameInput').val();
        var gender = $('#genderInput').val();
        var birthDate = $('#birthDateInput').val();
        var email = $('#emailInput').val();
        var phoneNumber = $('#phoneNumberInput').val();
        var hiringDate = $('#hiringDateInput').val(); // Ambil nilai dari input Hiring Date
    
        // Periksa apakah elemen input yang diperlukan sudah diisi
        /*if (!firstName || !birthDate || !email || !phoneNumber || !hiringDate) {
            $('#modalCenter').modal('hide');
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Harap isi semua field yang diperlukan.'
            });
            return; // Berhenti jika ada field yang kosong
        }*/

        var newData = {
            firstName: firstName,
            lastName: lastName,
            gender: parseInt(gender),
            birthDate: birthDate,
            email: email,
            phoneNumber: phoneNumber,
            hiringDate: hiringDate // Tambahkan Hiring Date ke dalam data
        };
    
        $.ajax({
            url: "https://localhost:7179/api/Employee",
            type: "POST",
            data: JSON.stringify(newData),
            contentType: "application/json",
            success: function (data) {
                // Data berhasil ditambahkan
                $('#modalCenter').modal('hide');
                $('#tableEmployee').DataTable().ajax.reload();
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Data berhasil ditambahkan!'
                });
            },
            error: function (xhr) {
                $('#modalCenter').modal('hide');
                if (xhr.status === 400) {
                    // Terjadi kesalahan validasi
                    var response = JSON.parse(xhr.responseText);
                    if (response.error && response.error.length > 0) {
                        // Tampilkan pesan-pesan error kepada pengguna
                        var errorMessages = response.error.join('\n');
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: errorMessages
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Gagal menambahkan data.'
                        });
                    }
                } else if (xhr.status === 500) {
                    var response = JSON.parse(xhr.responseText);
                    if (response.error) {
                        Swal.fire({
                            icon: 'error',
                            title: response.status || 'Error',
                            text: response.error
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Gagal menambahkan data.'
                        });
                    }
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Terjadi kesalahan: ' + xhr.status
                    });
                }
            }
        });
        
    });

    // Menangkap klik pada tombol "Edit" pada tabel
    $(document).on("click", ".edit-button", function () {
        var employeeId = $(this).data("guid");
        // Make an AJAX request to get the employee data
        $.ajax({
            url: "https://localhost:7179/api/Employee/" + employeeId,
            type: "GET",
            success: function (data) {
                if (data && data.status === 'OK') {
                    var employeeData = data.data;

                    $("#editEmployeeId").val(employeeData.guid);
                    $("#editNik").val(employeeData.nik);
                    $("#editFirstName").val(employeeData.firstName);
                    $("#editLastName").val(employeeData.lastName);
                    $("#editGender").val(employeeData.gender.toString());
                    $("#editBirthDate").val(employeeData.birthDate);
                    $("#editEmail").val(employeeData.email);
                    $("#editPhoneNumber").val(employeeData.phoneNumber);
                    $("#editHiringDate").val(employeeData.hiringDate);

                    // Show the edit modal
                    $("#modalEditEmployee").modal("show");
                } else {
                    // Handle any errors or invalid responses here
                    console.error("Error: Unable to retrieve employee data.");
                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: "Failed to retrieve employee data."
                    });
                }
            },
            error: function (xhr) {
                // Handle AJAX request errors here
                console.error("Error: AJAX request failed.", xhr);
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Failed to retrieve employee data."
                });
            }
        });
    });

    // Menangani klik pada tombol "Save Changes" di dalam modal edit
    $("#editEmployeeButton").click(function () {
        // Mendapatkan data yang diubah dari inputan modal
        var updatedEmployee = {
            guid: $("#editEmployeeId").val(),
            nik: $("#editNik").val(),
            firstName: $("#editFirstName").val(),
            lastName: $("#editLastName").val(),
            gender: parseInt($("#editGender").val()), // Konversi ke tipe data integer
            birthDate: $("#editBirthDate").val(),
            email: $("#editEmail").val(),
            phoneNumber: $("#editPhoneNumber").val(),
            hiringDate: $("#editHiringDate").val()
        };

        // Lakukan permintaan PUT ke API untuk memperbarui data employee
        $.ajax({
            url: "https://localhost:7179/api/Employee", // Pastikan URL sudah benar
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify(updatedEmployee),
            success: function (data) {
                // Sembunyikan modal edit
                $("#modalEditEmployee").modal("hide");
                // Refresh tabel data employee setelah pembaruan
                $('#tableEmployee').DataTable().ajax.reload();
                Swal.fire({
                    icon: "success",
                    title: "Success",
                    text: "Data berhasil diperbarui!"
                });
            },
            error: function (xhr) {
                $("#modalEditEmployee").modal("hide");
                if (xhr.status === 400) {
                    // Terjadi kesalahan validasi
                    var response = JSON.parse(xhr.responseText);
                    if (response.error && response.error.length > 0) {
                        // Tampilkan pesan-pesan error kepada pengguna
                        var errorMessages = response.error.join('\n');
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: errorMessages
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Gagal menambahkan data.'
                        });
                    }
                } else if (xhr.status === 500) {
                    var response = JSON.parse(xhr.responseText);
                    if (response.error) {
                        Swal.fire({
                            icon: 'error',
                            title: response.status || 'Error',
                            text: response.error
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Gagal menambahkan data.'
                        });
                    }
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Terjadi kesalahan: ' + xhr.status
                    });
                }
            }
        });
    });
    
});
