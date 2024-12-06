function ClearAlerts() {
    const AlertContainer = document.querySelector("#AlertsContainer")
    if (AlertContainer.hasChildNodes) {
        AlertContainer.textContent = "";
    }
}

function AddAlert(message) {
    const AlertContainer = document.querySelector("#AlertsContainer")
    AlertContainer.appendChild(CreateAlert(message))
}

function CreateAlert(message) {
    const AlertElement = document.createElement("div");
    AlertElement.classList.add("alert");
    AlertElement.classList.add("alert-danger");
    AlertElement.classList.add("row")
    AlertElement.role = "alert";
    AlertElement.textContent = message;

    return AlertElement;

}