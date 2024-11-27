﻿document.getElementById("loginForm").addEventListener("submit", async (event) => {
    event.preventDefault();

    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    try {
        const response = await fetch("https://localhost:44335/api/Auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            credentials: "include", // Cookie'yi göndermek için gerekli
            body: JSON.stringify({ email: username, password }),
        });

        const data = await response.json();

        if (response.ok) {
            // Başarılı giriş
            window.location.href = "success/success.html";
        } else {
            // Hata mesajını kullanıcıya göster
            document.getElementById("message").textContent = data.message || "mail veya şifre hatalı";
        }
    } catch (error) {
        document.getElementById("message").textContent = "Bir hata oluştu.";
        console.error("Error Response:", error);
    }
});