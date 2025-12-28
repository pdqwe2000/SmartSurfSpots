const API_URL = "https://smartsurfspots-production.up.railway.app" 

let token = localStorage.getItem("token")
let user = JSON.parse(localStorage.getItem("user") || "null")
let map = null
let markers = []
const L = window.L // Declare the L variable

// Inicialização
document.addEventListener("DOMContentLoaded", () => {
    const isLoginPage = window.location.pathname.includes("login.html")
    const isRegisterPage = window.location.pathname.includes("register.html")

    if (isLoginPage || isRegisterPage) {
        // On auth pages, setup form listeners
        setupAuthListeners()
    } else {
        // On main app page, check authentication
        if (token && user) {
            showApp()
        } else {
            // Redirect to login if not authenticated
            window.location.href = "login.html"
        }

        setupAppListeners()
    }
})

function setupAuthListeners() {
    const loginForm = document.getElementById("login-form")
    const registerForm = document.getElementById("register-form")

    if (loginForm) {
        loginForm.addEventListener("submit", handleLogin)
    }

    if (registerForm) {
        registerForm.addEventListener("submit", handleRegister)
    }
}

function setupAppListeners() {
    const addSpotForm = document.getElementById("add-spot-form")
    if (addSpotForm) {
        addSpotForm.addEventListener("submit", handleAddSpot)
    }

    const useLocationButton = document.querySelector(".btn-location")
    if (useLocationButton) {
        useLocationButton.addEventListener("click", useMyLocation)
    }
}

// Auth Functions
async function handleLogin(e) {
    e.preventDefault()
    const email = document.getElementById("login-email").value
    const password = document.getElementById("login-password").value
    const submitBtn = e.target.querySelector('button[type="submit"]')

    setButtonLoading(submitBtn, true)

    try {
        const response = await fetch(`${API_URL}/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password }),
        })

        if (response.ok) {
            const data = await response.json()
            token = data.token
            user = data.user
            localStorage.setItem("token", token)
            localStorage.setItem("user", JSON.stringify(user))
            window.location.href = "index.html"
        } else {
            const errorData = await response.json()
            showError(errorData.message || "Email ou password incorretos")
            setButtonLoading(submitBtn, false)
        }
    } catch (error) {
        console.error("Erro na requisição:", error)
        showError("Erro ao fazer login. Verifique sua conexão.")
        setButtonLoading(submitBtn, false)
    }
}

async function handleRegister(e) {
    e.preventDefault()
    const name = document.getElementById("register-name").value
    const email = document.getElementById("register-email").value
    const password = document.getElementById("register-password").value
    const submitBtn = e.target.querySelector('button[type="submit"]')

    setButtonLoading(submitBtn, true)

    try {
        const response = await fetch(`${API_URL}/auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name, email, password }),
        })

        if (response.ok) {
            const data = await response.json()
            token = data.token
            user = data.user
            localStorage.setItem("token", token)
            localStorage.setItem("user", JSON.stringify(user))
            window.location.href = "index.html"
        } else {
            const error = await response.json()
            showError(error.message || "Erro ao criar conta")
            setButtonLoading(submitBtn, false)
        }
    } catch (error) {
        showError("Erro ao criar conta. Verifique sua conexão.")
        setButtonLoading(submitBtn, false)
    }
}

function setButtonLoading(button, isLoading) {
    const btnText = button.querySelector(".btn-text")
    const btnLoading = button.querySelector(".btn-loading")

    if (isLoading) {
        button.disabled = true
        if (btnText) btnText.classList.add("hidden")
        if (btnLoading) btnLoading.classList.remove("hidden")
    } else {
        button.disabled = false
        if (btnText) btnText.classList.remove("hidden")
        if (btnLoading) btnLoading.classList.add("hidden")
    }
}

function showError(message) {
    const errorDiv = document.getElementById("auth-error")
    if (errorDiv) {
        errorDiv.textContent = message
        errorDiv.classList.remove("hidden")
        setTimeout(() => errorDiv.classList.add("hidden"), 5000)
    }
}

function logout() {
    token = null
    user = null
    localStorage.removeItem("token")
    localStorage.removeItem("user")
    window.location.href = "login.html"
}

// Screen Management
function showApp() {
    const appScreen = document.getElementById("app-screen")
    if (appScreen) {
        appScreen.classList.add("active")
        const userName = document.getElementById("user-name")
        const userInitials = document.getElementById("user-initials")

        if (userName && user) {
            userName.textContent = user.name
        }

        if (userInitials && user) {
            const initials = user.name
                .split(" ")
                .map((n) => n[0])
                .join("")
                .toUpperCase()
                .slice(0, 2)
            userInitials.textContent = initials
        }

        initMap()
        loadSpots()
    }
}

// Map Functions
function initMap() {
    if (!map) {
        map = L.map("map").setView([41.1579, -8.6291], 10)
        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
            attribution: "© OpenStreetMap",
        }).addTo(map)
    }
}

// Spots Functions
async function loadSpots() {
    try {
        const response = await fetch(`${API_URL}/spots`, {
            headers: { Authorization: `Bearer ${token}` },
        })

        if (response.ok) {
            const spots = await response.json()
            displaySpots(spots)
        } else if (response.status === 401) {
            logout()
        }
    } catch (error) {
        console.error("Erro ao carregar spots:", error)
    }
}

function displaySpots(spots) {
    markers.forEach((marker) => map.removeLayer(marker))
    markers = []

    const spotsList = document.getElementById("spots-list")
    spotsList.innerHTML = ""

    if (spots.length === 0) {
        spotsList.innerHTML = `
            <div class="loading-state">
                <p>Nenhum spot adicionado ainda.</p>
            </div>
        `
        return
    }

    spots.forEach((spot) => {
        const spotItem = document.createElement("div")
        spotItem.className = "spot-item"
        const levelEmoji = spot.level === 1 ? "🟢" : spot.level === 2 ? "🟡" : "🔴"
        const levelText = spot.level === 1 ? "Iniciante" : spot.level === 2 ? "Intermédio" : "Avançado"

        spotItem.innerHTML = `
            <h4>${spot.name}</h4>
            <p>${levelEmoji} ${levelText} • ${spot.createdBy || "Unknown"}</p>
        `
        spotItem.onclick = () => showSpotDetails(spot)
        spotsList.appendChild(spotItem)

        const marker = L.marker([spot.latitude, spot.longitude])
            .addTo(map)
            .bindPopup(`<b>${spot.name}</b><br>${spot.description || ""}`)

        marker.on("click", () => showSpotDetails(spot))
        markers.push(marker)
    })

    if (spots.length > 0) {
        const bounds = L.latLngBounds(spots.map((s) => [s.latitude, s.longitude]))
        map.fitBounds(bounds, { padding: [50, 50] })
    }
}

async function showSpotDetails(spot) {
    const detailsDiv = document.getElementById("spot-details")
    const levelEmoji = spot.level === 1 ? "🟢" : spot.level === 2 ? "🟡" : "🔴"
    const levelText = spot.level === 1 ? "Iniciante" : spot.level === 2 ? "Intermédio" : "Avançado"

    detailsDiv.innerHTML = `
        <button class="close-details" onclick="closeSpotDetails()">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <line x1="18" y1="6" x2="6" y2="18"></line>
                <line x1="6" y1="6" x2="18" y2="18"></line>
            </svg>
        </button>
        <h2>${spot.name}</h2>
        <p><strong>Nível:</strong> ${levelEmoji} ${levelText}</p>
        <p><strong>Descrição:</strong> ${spot.description || "Sem descrição"}</p>
        <p><strong>Criado por:</strong> ${spot.createdBy || "Unknown"}</p>
        <p><strong>Coordenadas:</strong> ${spot.latitude.toFixed(4)}, ${spot.longitude.toFixed(4)}</p>
        
        <button class="btn-checkin" onclick="doCheckIn(${spot.id}, '${spot.name}')">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="width: 18px; height: 18px;">
                <polyline points="20 6 9 17 4 12"></polyline>
            </svg>
            Fazer Check-in
        </button>
        
        <button class="delete-spot-btn" onclick="deleteSpot(${spot.id})">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <polyline points="3 6 5 6 21 6"></polyline>
                <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                <line x1="10" y1="11" x2="10" y2="17"></line>
                <line x1="14" y1="11" x2="14" y2="17"></line>
            </svg>
            Remover Spot
        </button>
        
        <div class="weather-info">
            <h3>🌤️ A carregar meteorologia...</h3>
        </div>
        
        <div class="checkins-section">
            <h3>👥 Check-ins Recentes</h3>
            <div id="spot-checkins">A carregar...</div>
        </div>
    `
    detailsDiv.classList.remove("hidden")

    try {
        const response = await fetch(`${API_URL}/spots/${spot.id}/weather`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })

        if (response.ok) {
            const weather = await response.json()
            displayWeather(weather)
        }
    } catch (error) {
        console.error("Erro ao carregar meteorologia:", error)
    }

    loadSpotCheckIns(spot.id)
}

function closeSpotDetails() {
    const detailsDiv = document.getElementById("spot-details")
    detailsDiv.classList.add("hidden")
}

function displayWeather(weather) {
    const weatherDiv = document.querySelector(".weather-info")
    weatherDiv.innerHTML = `
        <h3>🌤️ Condições Atuais</h3>
        <div class="weather-current">
            <div class="weather-item">
                <strong>🌡️ Temperatura</strong>
                ${weather.current.temperature}°C
            </div>
            <div class="weather-item">
                <strong>💨 Vento</strong>
                ${weather.current.windSpeed} km/h
            </div>
            <div class="weather-item">
                <strong>🧭 Direção</strong>
                ${weather.current.windDirection}°
            </div>
            <div class="weather-item">
                <strong>☁️ Tempo</strong>
                ${weather.current.weatherDescription}
            </div>
        </div>
        ${weather.forecast && weather.forecast.length > 0
            ? `
            <h4>Próximas Horas</h4>
            ${weather.forecast
                .slice(0, 6)
                .map(
                    (f) => `
                <div style="background: white; padding: 10px; margin: 8px 0; border-radius: 10px;">
                    <strong>${new Date(f.time).toLocaleTimeString("pt-PT", { hour: "2-digit", minute: "2-digit" })}</strong>
                    - ${f.temperature}°C, Vento: ${f.windSpeed} km/h
                    ${f.waveHeight > 0 ? `, Ondas: ${f.waveHeight}m` : ""}
                </div>
            `,
                )
                .join("")}
        `
            : ""
        }
    `
}

// Modal Functions
function showAddSpotModal() {
    document.getElementById("add-spot-modal").classList.remove("hidden")
}

function closeAddSpotModal() {
    document.getElementById("add-spot-modal").classList.add("hidden")
    document.getElementById("add-spot-form").reset()
}

async function handleAddSpot(e) {
    e.preventDefault()

    const spotData = {
        name: document.getElementById("spot-name").value,
        latitude: Number.parseFloat(document.getElementById("spot-lat").value),
        longitude: Number.parseFloat(document.getElementById("spot-lon").value),
        description: document.getElementById("spot-description").value,
        level: Number.parseInt(document.getElementById("spot-level").value),
    }

    try {
        const response = await fetch(`${API_URL}/spots`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(spotData),
        })

        if (response.ok) {
            closeAddSpotModal()
            loadSpots()
        } else {
            alert("Erro ao adicionar spot")
        }
    } catch (error) {
        alert("Erro ao adicionar spot: " + error.message)
    }
}

async function deleteSpot(spotId) {
    if (!confirm("Tem a certeza que deseja remover este spot?")) {
        return
    }

    try {
        const response = await fetch(`${API_URL}/spots/${spotId}`, {
            method: "DELETE",
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })

        if (response.ok) {
            closeSpotDetails()
            loadSpots()
        } else if (response.status === 401) {
            logout()
        } else {
            alert("Erro ao remover spot")
        }
    } catch (error) {
        console.error("Erro ao remover spot:", error)
        alert("Erro ao remover spot: " + error.message)
    }
}

// Geolocation Function
function useMyLocation() {
    const button = document.querySelector(".btn-location")
    const btnText = button.querySelector(".btn-text")
    const btnLoading = button.querySelector(".btn-loading")

    if (!navigator.geolocation) {
        alert("A geolocalização não é suportada pelo seu navegador")
        return
    }

    // Show loading state
    button.disabled = true
    btnText.classList.add("hidden")
    btnLoading.classList.remove("hidden")

    navigator.geolocation.getCurrentPosition(
        (position) => {
            // Success - populate the form fields
            document.getElementById("spot-lat").value = position.coords.latitude.toFixed(4)
            document.getElementById("spot-lon").value = position.coords.longitude.toFixed(4)

            // Reset button state
            button.disabled = false
            btnText.classList.remove("hidden")
            btnLoading.classList.add("hidden")

            // Show success feedback
            const originalText = btnText.textContent
            btnText.innerHTML = `
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="width: 16px; height: 16px; display: inline;">
          <polyline points="20 6 9 17 4 12"></polyline>
        </svg>
        Localização obtida!
      `
            setTimeout(() => {
                btnText.textContent = originalText
            }, 2000)
        },
        (error) => {
            // Error handling
            let errorMessage = "Erro ao obter localização"
            switch (error.code) {
                case error.PERMISSION_DENIED:
                    errorMessage = "Permissão de localização negada. Por favor, ative nas configurações do navegador."
                    break
                case error.POSITION_UNAVAILABLE:
                    errorMessage = "Informação de localização não disponível"
                    break
                case error.TIMEOUT:
                    errorMessage = "O pedido de localização expirou"
                    break
            }

            alert(errorMessage)

            // Reset button state
            button.disabled = false
            btnText.classList.remove("hidden")
            btnLoading.classList.add("hidden")
        },
        {
            enableHighAccuracy: true,
            timeout: 10000,
            maximumAge: 0,
        },
    )
}

// Check-in Functions
async function doCheckIn(spotId, spotName) {
    const notes = prompt(`Check-in em ${spotName}.\nNotas (opcional):`)

    if (notes === null) return // User cancelled

    try {
        const response = await fetch(`${API_URL}/checkins`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify({ spotId, notes: notes || "" }),
        })

        if (response.ok) {
            alert("✓ Check-in realizado com sucesso!")
            loadSpotCheckIns(spotId)
        } else {
            alert("Erro ao fazer check-in")
        }
    } catch (error) {
        alert("Erro: " + error.message)
    }
}

async function loadSpotCheckIns(spotId) {
    try {
        const response = await fetch(`${API_URL}/checkins/spot/${spotId}`, {
            headers: { Authorization: `Bearer ${token}` },
        })

        if (response.ok) {
            const checkIns = await response.json()
            const checkInsDiv = document.getElementById("spot-checkins")

            if (checkIns.length === 0) {
                checkInsDiv.innerHTML = '<p style="color: #999; font-style: italic;">Ainda não há check-ins neste spot.</p>'
            } else {
                checkInsDiv.innerHTML = checkIns
                    .slice(0, 5)
                    .map(
                        (c) => `
          <div class="checkin-item">
            <div class="checkin-header">
              <strong>${c.userName}</strong>
              <span class="checkin-date">${new Date(c.dateTime).toLocaleString("pt-PT", {
                            day: "2-digit",
                            month: "2-digit",
                            hour: "2-digit",
                            minute: "2-digit",
                        })}</span>
            </div>
            ${c.notes ? `<p class="checkin-notes">${c.notes}</p>` : ""}
          </div>
        `,
                    )
                    .join("")
            }
        }
    } catch (error) {
        console.error("Erro ao carregar check-ins:", error)
    }
}
