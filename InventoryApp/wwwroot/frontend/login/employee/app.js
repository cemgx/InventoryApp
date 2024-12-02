const API_URL = "https://localhost:44335/api/Employee";
const userName = "Test User"; // Örnek kullanıcı adı
document.getElementById("userName").textContent = userName;

// Navigation Logic
const sections = document.querySelectorAll(".content-section");
const navButtons = document.querySelectorAll(".nav-btn");

navButtons.forEach((button) => {
    button.addEventListener("click", () => {
        navButtons.forEach((btn) => btn.classList.remove("active"));
        button.classList.add("active");

        sections.forEach((section) => section.classList.remove("active"));
        document.getElementById(`${button.id.replace("Page", "Section")}`).classList.add("active");
    });
});

// Get Employees
document.getElementById("getEmployeesBtn").addEventListener("click", async () => {
    try {
        const response = await fetch(`${API_URL}`);
        if (!response.ok) throw new Error("Failed to fetch employees.");
        const employees = await response.json();
        displayTable(employees, "employeeTable");
    } catch (error) {
        showResponse(error.message, "getResponse", true);
    }
});

// GetEmployee By ID
document.getElementById("getEmployeeByIdBtn").addEventListener("click", async () => {
    const id = document.getElementById("getByIdInput").value;
    if (!id) {
        showResponse("Please enter an ID.", "getResponse", true);
        return;
    }

    try {
        const response = await fetch(`${API_URL}/search?id=${encodeURIComponent(id)}`);
        if (!response.ok) throw new Error("Failed to fetch employee by ID.");

        // Burada gelen veriyi kontrol ediyoruz
        const employee = await response.json();

        // Eğer gelen employee verisi null veya boşsa
        if (!employee || !employee.id) {
            showResponse("Employee not found.", "getResponse", true);
            return;
        }

        // Gelen veriyi tabloya ekle
        displayTable([employee], "employeeTable");
    } catch (error) {
        showResponse(error.message, "getResponse", true);
    }
});

// Get Employee by Name
document.getElementById("getEmployeeByNameBtn").addEventListener("click", async () => {
    const name = document.getElementById("getByNameInput").value;
    if (!name) return showResponse("Please enter a name.", "getResponse", true);

    try {
        const response = await fetch(`${API_URL}/search?name=${encodeURIComponent(name)}`);
        if (!response.ok) throw new Error("Failed to fetch employees by name.");
        const employees = await response.json();
        displayTable(employees, "employeeTable");
    } catch (error) {
        showResponse(error.message, "getResponse", true);
    }
});

// GetAllEmployees Method
document.getElementById("getAllEmployeesBtn").addEventListener("click", async () => {
    try {
        const response = await fetch(`${API_URL}/all`);
        if (!response.ok) throw new Error("Failed to fetch all employees.");
        const employees = await response.json();
        displayTable(employees, "employeeTable");
    } catch (error) {
        showResponse(error.message, "getResponse", true);
    }
});

// Post - Create Employee
document.getElementById("createEmployeeBtn").addEventListener("click", async () => {
    const name = document.getElementById("postName").value;
    const email = document.getElementById("postEmail").value;
    const password = document.getElementById("postPassword").value;

    if (!name || !email || !password) {
        return showResponse("All fields are required.", "postResponse", true);
    }

    try {
        const response = await fetch(`${API_URL}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name, email, password }),
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Failed to create employee.");
        }
        const result = await response.text();
        showResponse(result, "postResponse");
    } catch (error) {
        showResponse(error.message, "postResponse", true);
    }
});

// Put - Update Employee
document.getElementById("updateEmployeeBtn").addEventListener("click", async () => {
    const id = document.getElementById("putId").value;
    const name = document.getElementById("putName").value;
    const email = document.getElementById("putEmail").value;
    const password = document.getElementById("putPassword").value;

    if (!id || !name || !email || !password) {
        return showResponse("All fields are required.", "putResponse", true);
    }

    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name, email, password }),
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Failed to update employee.");
        }
        showResponse("Employee updated successfully.", "putResponse");
    } catch (error) {
        showResponse(error.message, "putResponse", true);
    }
});

// Delete Employee
document.getElementById("deleteEmployeeBtn").addEventListener("click", async () => {
    const id = document.getElementById("deleteId").value;
    if (!id) return showResponse("Please enter an ID.", "deleteResponse", true);

    try {
        const response = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
        if (!response.ok) throw new Error("Failed to delete employee.");
        showResponse("Employee deleted successfully.", "deleteResponse");
    } catch (error) {
        showResponse(error.message, "deleteResponse", true);
    }
});

// Helper Functions
function displayTable(data, tableId) {
    const table = document.getElementById(tableId);
    table.innerHTML = `
        <thead>
            <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Email</th>
            </tr>
        </thead>
        <tbody>
            ${data
            .map(
                (item) =>
                    `<tr>
                            <td>${item.id || "N/A"}</td>
                            <td>${item.name || "N/A"}</td>
                            <td>${item.email || "N/A"}</td>
                        </tr>`
            )
            .join("")}
        </tbody>
    `;
}

function showResponse(message, elementId, isError = false) {
    const element = document.getElementById(elementId);
    element.textContent = message;
    element.style.color = isError ? "red" : "green";
}
