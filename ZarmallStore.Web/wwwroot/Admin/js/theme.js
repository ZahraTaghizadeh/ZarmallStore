function setLayoutMode() {
    const layoutMode = localStorage.getItem("data-layout-mode");
    if (layoutMode) {
        document.body.setAttribute("data-layout-mode", layoutMode);
    } else {
        document.body.setAttribute("data-layout-mode", "light");
    }
}

function toggleDarkMode() {
    const currentMode = document.body.getAttribute("data-layout-mode");
    if (currentMode === "dark") {
        document.body.setAttribute("data-layout-mode", "light");
        localStorage.setItem("data-layout-mode", "light");
    } else {
        document.body.setAttribute("data-layout-mode", "dark");
        localStorage.setItem("data-layout-mode", "dark");
    }
}

document.addEventListener("DOMContentLoaded", setLayoutMode);