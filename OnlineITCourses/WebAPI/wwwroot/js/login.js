function login() {
    const username = $("#username").val();
    const password = $("#password").val();

    $("#errorMessage").text("");

    if (!username || !password) {
        $("#errorMessage").text("Please enter username and password.");
        return;
    }

    const loginData = {
        username: username,
        password: password
    };

    $.ajax({
        method: "POST",
        url: "/api/auth/login",
        data: JSON.stringify(loginData),
        contentType: "application/json"
    })
        .done(function (token) {
            localStorage.setItem("JWT", token);
            window.location.href = "logs.html";
        })
        .fail(function (err) {
            if (err.responseText) {
                $("#errorMessage").text(err.responseText);
            } else {
                $("#errorMessage").text("Login failed.");
            }
        });
}