$(function () {
    const token = localStorage.getItem("JWT");

    if (!token) {
        window.location.href = "login.html";
        return;
    }

    loadLogs();
});

function loadLogs() {
    const token = localStorage.getItem("JWT");

    if (!token) {
        window.location.href = "login.html";
        return;
    }

    const count = $("#logCount").val();

    $("#errorMessage").text("");
    $("#logsTableBody").empty();

    $.ajax({
        method: "GET",
        url: `/api/logs/get/${count}`,
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .done(function (logs) {
            if (!logs || logs.length === 0) {
                $("#logsTableBody").append(`
                <tr>
                    <td colspan="5">No logs found.</td>
                </tr>
            `);
                return;
            }

            logs.forEach(function (log) {
                $("#logsTableBody").append(`
                <tr>
                    <td>${log.id}</td>
                    <td>${log.timestamp}</td>
                    <td>${log.level}</td>
                    <td>${log.message}</td>
                    <td>${log.errorText ?? ""}</td>
                </tr>
            `);
            });
        })
        .fail(function (err) {
            if (err.status === 401) {
                localStorage.removeItem("JWT");
                window.location.href = "login.html";
                return;
            }

            $("#errorMessage").text("Error while loading logs.");
            console.error(err);
        });
}

function logout() {
    localStorage.removeItem("JWT");
    window.location.href = "login.html";
}