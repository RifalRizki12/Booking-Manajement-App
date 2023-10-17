$(document).ready(function () {
    $("#loginButton").click(function () {
        const email = $("#email").val();
        const password = $("#password").val();
        const data = {
            email: email,
            password: password
        };

        $.ajax({
            type: "POST",
            url: "https://localhost:7179/api/Auth/login",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (response) {
                // Login berhasil
                swal.fire({
                    title: "Login Berhasil!",
                    text: "Anda berhasil login.",
                    icon: "success"
                }).then(() => {
                    // Redirect ke halaman lain setelah login berhasil
                    window.location.href = "Employee.html";
                });
            },
            error: function (xhr, status, error) {
                // Login gagal
                const errorResponse = JSON.parse(xhr.responseText);
                handleLoginError(errorResponse);
                console.log(errorResponse);
            }
        });
    });

    function handleLoginError(errorResponse) {
        // Handle pesan kesalahan yang dikirim dari API
        swal.fire({
            title: "Login Gagal",
            text: errorResponse.message,
            icon: "error"
        });
    }
});
