// File Type Detector
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("input[type='file']").forEach(fileInput => {
        fileInput.addEventListener("change", function () {
            const file = this.files[0];
            const errorMessage = this.nextElementSibling;

            if (file) {
                console.log("نوع فایل انتخاب شده:", file.type);

                if (!file.type.startsWith("image/")) {
                    if (errorMessage) {
                        errorMessage.textContent = "فقط فایل‌های تصویری مجاز هستند.";
                    }
                    this.value = "";
                } else {
                    if (errorMessage) {
                        errorMessage.textContent = "";
                    }
                }
            }
        });
    });
});

// Base Price input Format
function formatNumber(value) {
    if (!value) return "";
    return Number(value).toLocaleString("en-US");
}

document.addEventListener("DOMContentLoaded", function () {
    const input = document.getElementById("currencyInput");

    input.value = formatNumber(input.value);

    input.addEventListener("input", function () {
        const cursorPosition = this.selectionStart;
        const rawValue = this.value.replace(/\D/g, "");
        const formattedValue = formatNumber(rawValue);

        this.value = formattedValue;

        const diff = formattedValue.length - rawValue.length;
        this.setSelectionRange(cursorPosition + diff, cursorPosition + diff);
    });
});

// Image Gallery Preview
function updateFileList(inputId, listId) {
    document.getElementById(inputId).addEventListener("change", function (event) {
        const fileList = document.getElementById(listId);

        Array.from(event.target.files).forEach(file => {
            if (!file.type.startsWith("image/")) return;

            const reader = new FileReader();
            reader.onload = function (e) {
                const div = document.createElement("div");
                div.classList.add("thumbnail");

                const img = document.createElement("img");
                img.src = e.target.result;

                const removeBtn = document.createElement("button");
                removeBtn.textContent = "×";
                removeBtn.classList.add("remove-btn");
                removeBtn.onclick = function () {
                    div.remove();
                };

                div.appendChild(img);
                div.appendChild(removeBtn);
                fileList.appendChild(div);
            };
            reader.readAsDataURL(file);
        });
    });
}

updateFileList("MainImage", "fileList1");