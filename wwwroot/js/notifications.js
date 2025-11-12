// SignalR connection for notifications
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/bestellingNotificationHub")
    .build();

// Show Bootstrap alert with optional button
function showNotification(notificationType, message, visualStyle = "info") {
    const container = document.getElementById("notification-container");
    const alertId = "alert-" + Date.now();
    let buttonHtml = "";

    if (notificationType === "NieuweBestelling") {
        buttonHtml = `<a href="/Bestelling/Index" class="btn btn-sm btn-primary ms-2">Bestellingen</a>`;
    }

    const alertHtml = `
        <div id="${alertId}" class="alert alert-${visualStyle} alert-dismissible fade show mb-2" role="alert">
            <i class="bi bi-bell-fill"></i> ${message}
            ${buttonHtml}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>`;
    container.insertAdjacentHTML("beforeend", alertHtml);
    setTimeout(() => {
        const alertElem = document.getElementById(alertId);
        if (alertElem) {
            alertElem.classList.remove("show");
            // Wait for Bootstrap's fade transition (500ms), then remove
            setTimeout(() => {
                alertElem.remove();
            }, 500);
        }
    }, 5000); // 5 seconds
}

connection.start().then(function () {
    const userRole = document.body.getAttribute("data-user-role") || "Klant";
    console.log("SignalR connected, joining group:", userRole);
    connection.invoke("AddToGroup", userRole)
        .then(() => console.log("Joined group:", userRole))
        .catch(err => console.error("AddToGroup error:", err));
}).catch(function (err) {
    console.error("SignalR connection error:", err);
});

// Receive notification from server for new orders
connection.on("NieuweBestelling", function (message) {
    console.log("Received NieuweBestelling:", message);
    showNotification("NieuweBestelling", message, "warning");
});