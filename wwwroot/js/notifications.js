// SignalR connection for notifications
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/bestellingNotificationHub")
    .build();

// Helper to format "time ago"
function timeAgo(date) {
    const seconds = Math.floor((new Date() - date) / 1000);
    if (seconds < 60) return `${seconds}s ago`;
    const minutes = Math.floor(seconds / 60);
    if (minutes < 60) return `${minutes}m ago`;
    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours}h ago`;
    return `${Math.floor(hours / 24)}d ago`;
}

// Show Bootstrap toast (stackable)
function showNotification(notificationType, title = "alert", message) {
    const container = document.getElementById("notification-container");
    const toastId = "toast-" + Date.now();
    const now = new Date();

    // Toast HTML
    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-bg-warning border-0 mb-2" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="false">
            <div class="toast-header">
                <i class="bi bi-bell-fill me-2" style="font-size: 1.25rem; color: #ffc107;"></i>
                <strong class="me-auto">${title}</strong>
                <small class="text-muted" id="${toastId}-time">${timeAgo(now)}</small>
                <button type="button" class="btn-close ms-2" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body d-flex justify-content-between align-items-center">
                <span class="text-start flex-grow-1">${message}</span>
                ${notificationType === "NieuweBestelling" ? `
                    <a href="/Bestelling/Index" class="btn btn-sm btn-warning ms-3">Bestellingen</a>
                ` : ""}
            </div>
        </div>
    `;
    container.insertAdjacentHTML("beforeend", toastHtml);

    // Show the toast using Bootstrap's JS API
    const toastElem = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElem, { autohide: false });
    toast.show();

    // Update "time ago" every second
    const timeElem = document.getElementById(`${toastId}-time`);
    let interval = setInterval(() => {
        if (document.getElementById(toastId)) {
            timeElem.textContent = timeAgo(now);
        } else {
            clearInterval(interval);
        }
    }, 1000);
}

// Ensure notification container exists and is stackable
document.addEventListener("DOMContentLoaded", function () {
    if (!document.getElementById("notification-container")) {
        const div = document.createElement("div");
        div.id = "notification-container";
        div.style.position = "fixed";
        div.style.top = "80px";
        div.style.right = "30px";
        div.style.zIndex = "1055";
        div.style.width = "350px";
        div.style.maxWidth = "100vw";
        document.body.appendChild(div);
    }
});

connection.start().then(function () {
    const userRole = document.body.getAttribute("data-user-role") || "Klant";
    connection.invoke("AddToGroup", userRole)
        .then(() => console.log("Joined group:", userRole))
        .catch(err => console.error("AddToGroup error:", err));
})

// Receive notification from server for new orders
connection.on("NieuweBestelling", function (message) {
    showNotification("NieuweBestelling", "Bestelling", message);
});