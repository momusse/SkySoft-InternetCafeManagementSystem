/**
 * SkySoft Internet Cafe — Customer Portal *
 * Connects to the C# backend via REST API (CafeApiServer.cs) on localhost:5000.
 * All data is read from and written to the SQL database through the API.
 * Each function maps to a console app option as documented below.
 */
// CONFIGURATION
/** Base URL for the backend API (CafeApiServer.cs running on localhost:5000) */
const API_BASE = "http://localhost:5000/api";

// INPUT VALIDATION
// Mirrors the C# ContainsBannedWord() method in Program.cs

const BANNED_WORDS = [
  "porn", "pornography", "fuck", "xxx", "shit", "bitch", "ass",
  "fucking", "slut", "whore", "asshole", "bastard", "dick", "cock",
  "pussy", "nude", "naked", "bullshit", "motherfucker", "admin", "fake"
];

/**
 * Normalises a string for banned word comparison.
 * Strips all non-alphanumeric characters and lowercases.
 * @param {string} value
 * @returns {string}
 */
function normalizeText(value) {
  return String(value).toLowerCase().replace(/[^a-z0-9]/g, "");
}

/**
 * Checks whether a string contains any banned word.
 * Mirrors ContainsBannedWord() in Program.cs.
 * @param {string} value
 * @returns {boolean}
 */
function containsBannedWord(value) {
  const clean = normalizeText(value);
  return BANNED_WORDS.some(word => clean.includes(normalizeText(word)));
}

// UI HELPERS
/**
 * Adds shake animation and red border to one or more input elements.
 * @param {...HTMLElement} elements
 */
function triggerInputError(...elements) {
  elements.forEach(el => {
    if (!el) return;
    el.classList.add("input-error");
    setTimeout(() => el.classList.remove("input-error"), 600);
  });
}

/**
 * Shows a success or error message in a message container.
 * @param {string} id  - Element ID of the message container
 * @param {string} text
 * @param {"success"|"error"} type
 */
function showMsg(id, text, type) {
  const el = document.getElementById(id);
  if (!el) return;
  el.className = `message show ${type}`;
  el.textContent = text;
}

/**
 * Formats a number as a pound string.
 * @param {number} value
 * @returns {string}
 */
const money = value => `£${Number(value).toFixed(2)}`;

// API HELPER
/**
 * Wrapper around fetch() for all API calls.
 * Automatically sets Content-Type and throws on non-OK responses.
 * @param {string} path   - API path e.g. "/customers/C001"
 * @param {object} options - fetch options (method, body etc.)
 * @returns {Promise<any>} Parsed JSON response
 */
async function apiFetch(path, options = {}) {
  const response = await fetch(API_BASE + path, {
    headers: { "Content-Type": "application/json" },
    ...options
  });

  const data = await response.json();

  if (!response.ok) {
    throw new Error(data.error || "Request failed");
  }

  return data;
}
// CLOCK

function updateClock() {
  const el = document.getElementById("liveClock");
  if (el) el.textContent = new Date().toLocaleString();
}

updateClock();
setInterval(updateClock, 1000);

// STATE

/** Currently signed-in customer object (from API) */
let currentCustomer = null;

// LANDING — SIGN IN / REGISTER

function showLogin() {
  document.getElementById("loginCard").classList.remove("hidden");
  document.getElementById("registerCard").classList.add("hidden");
}

function showRegister() {
  document.getElementById("loginCard").classList.add("hidden");
  document.getElementById("registerCard").classList.remove("hidden");
}

/**
 * Handles sign in.
 * Calls GET /api/customers/{id} — maps to Console Option 3 (Search by ID).
 */
async function handleLogin() {
  const idInput = document.getElementById("loginId");
  const id = idInput.value.trim().toUpperCase();

  if (!id) {
    showMsg("loginMessage", "Please enter your Customer ID.", "error");
    return;
  }

  try {
    const customer = await apiFetch(`/customers/${id}`);
    currentCustomer = customer;
    openDashboard();
  } catch {
    triggerInputError(idInput);
    showMsg("loginMessage", "Customer ID not found. Please check and try again.", "error");
  }
}

/**
 * Handles new customer registration.
 * Calls POST /api/customers — maps to Console Option 10 (Add Customer).
 * Applies the same banned-word and duplicate-ID validation as the C# backend.
 */
async function handleRegister() {
  const idInput      = document.getElementById("regId");
  const nameInput    = document.getElementById("regName");
  const emailInput   = document.getElementById("regEmail");
  const balanceInput = document.getElementById("regBalance");

  const id      = idInput.value.trim();
  const name    = nameInput.value.trim();
  const email   = emailInput.value.trim();
  const balance = Number(balanceInput.value);

  // Basic required-field check
  if (!id || !name || !email || isNaN(balance) || balance < 0) {
    showMsg("registerMessage", "Please complete all fields with valid values.", "error");
    return;
  }

  // Banned word check — mirrors C# ContainsBannedWord in Program.cs
  const fieldsToCheck = [
    { el: idInput,    val: id,    label: "Customer ID" },
    { el: nameInput,  val: name,  label: "Name"        },
    { el: emailInput, val: email, label: "Email"       }
  ];

  const invalidFields = fieldsToCheck.filter(f => containsBannedWord(f.val));

  if (invalidFields.length > 0) {
    triggerInputError(...invalidFields.map(f => f.el));
    showMsg(
      "registerMessage",
      `Input rejected. Offensive or restricted word detected in: ${invalidFields.map(f => f.label).join(", ")}.`,
      "error"
    );
    return;
  }

  try {
    // POST to API — backend also validates uniqueness and saves to SQL database
    const customer = await apiFetch("/customers", {
      method: "POST",
      body: JSON.stringify({ customerId: id, name, email, balance })
    });
    currentCustomer = customer;
    openDashboard();
  } catch (err) {
    // Handle duplicate ID returned from backend
    showMsg("registerMessage", err.message, "error");
  }
}

// Live banned-word highlight on name field as user types
document.getElementById("regName").addEventListener("input", e => {
  if (containsBannedWord(e.target.value)) {
    e.target.classList.add("input-error");
  } else {
    e.target.classList.remove("input-error");
  }
});

// DASHBOARD — OPEN / CLOSE

async function openDashboard() {
  document.getElementById("landingSection").classList.add("hidden");
  document.getElementById("dashboard").classList.remove("hidden");
  await checkApiConnection();
  await renderAll();
}

function handleLogout() {
  currentCustomer = null;
  document.getElementById("dashboard").classList.add("hidden");
  document.getElementById("landingSection").classList.remove("hidden");
  document.getElementById("loginId").value = "";
  showLogin();
}

// API CONNECTION CHECK

async function checkApiConnection() {
  const banner = document.getElementById("apiBanner");
  try {
    await apiFetch("/stats");
    banner.className = "api-banner api-connected";
    banner.textContent = "✓ Connected to backend API (http://localhost:5000) — all data is live from the SQL database.";
  } catch {
    banner.className = "api-banner api-disconnected";
    banner.textContent = "✗ Backend API not reachable. Run the C# console app (dotnet run) first, then refresh.";
  }
}

/** Renders all dashboard sections */
async function renderAll() {
  await Promise.all([
    renderStats(),
    renderTopbar(),
    renderSelects(),
    renderAllSessions(),
    renderActiveSessions(),
    renderHistory(),
    renderPcGrid(),
    refreshBalance()
  ]);
}

/**
 * Fetches and renders the 4 stat tiles.
 * GET /api/stats
 */
async function renderStats() {
  try {
    const stats = await apiFetch("/stats");
    document.getElementById("statTotalCustomers").textContent = stats.totalCustomers;
    document.getElementById("statActiveSessions").textContent = stats.activeSessions;
    document.getElementById("statAvailablePcs").textContent   = stats.availablePcs;
    document.getElementById("statTotalSessions").textContent  = stats.totalSessions;
  } catch {
    // Leave values as — if API is unreachable
  }
}

/**
 * Updates the welcome bar name, ID and active session status.
 */
async function renderTopbar() {
  document.getElementById("nameDisplay").textContent = currentCustomer.name;
  document.getElementById("idDisplay").textContent   = currentCustomer.customerId;

  try {
    // GET /api/sessions/history/{id} — Console Option 8
    const history = await apiFetch(`/sessions/history/${currentCustomer.customerId}`);
    const active  = history.find(s => !s.endTime);
    document.getElementById("sessionStatus").textContent = active
      ? `🟢 Active session on ${active.pcId}`
      : "No active session";
  } catch {
    document.getElementById("sessionStatus").textContent = "—";
  }
}

/**
 * Populates customer and active session dropdowns.
 * GET /api/customers and GET /api/sessions/active
 */
async function renderSelects() {
  try {
    const customers = await apiFetch("/customers");
    const options   = customers.map(c =>
      `<option value="${c.customerId}">${c.customerId} — ${c.name}</option>`
    ).join("");
    document.getElementById("sessionCustomer").innerHTML = options;
  } catch {
    document.getElementById("sessionCustomer").innerHTML = '<option>Could not load customers</option>';
  }

  try {
    const active = await apiFetch("/sessions/active");
    document.getElementById("activeSessionSelect").innerHTML = active.length
      ? active.map(s =>
          `<option value="${s.sessionId}">${s.sessionId} — ${s.customerId} — ${s.pcId}</option>`
        ).join("")
      : '<option value="">No active sessions</option>';
  } catch {
    document.getElementById("activeSessionSelect").innerHTML = '<option>Could not load sessions</option>';
  }
}

/**
 * Renders the all-sessions table.
 * GET /api/sessions — Console Option 5
 */
async function renderAllSessions() {
  const tbody = document.getElementById("allSessionsTable");
  try {
    const sessions = await apiFetch("/sessions");
    tbody.innerHTML = sessions.length
      ? sessions.map(s => `
          <tr>
            <td>${s.sessionId}</td>
            <td>${s.customerId}</td>
            <td>${s.pcId}</td>
            <td>${s.startTime}</td>
            <td>${s.endTime || '<span class="badge warning">In progress</span>'}</td>
            <td>${money(s.cost)}</td>
          </tr>`).join("")
      : '<tr><td colspan="6">No sessions found.</td></tr>';
  } catch {
    tbody.innerHTML = '<tr><td colspan="6">Could not load sessions from backend.</td></tr>';
  }
}

/**
 * Renders the active sessions table in the Overview tab.
 * GET /api/sessions/active — Console Option 6
 */
async function renderActiveSessions() {
  const tbody = document.getElementById("activeSessionsTable");
  try {
    const active = await apiFetch("/sessions/active");
    tbody.innerHTML = active.length
      ? active.map(s => `
          <tr>
            <td>${s.sessionId}</td>
            <td>${s.pcId}</td>
            <td>${s.startTime}</td>
            <td><span class="badge success">Active</span></td>
          </tr>`).join("")
      : '<tr><td colspan="4">No active sessions right now.</td></tr>';
  } catch {
    tbody.innerHTML = '<tr><td colspan="4">Could not load active sessions.</td></tr>';
  }
}

/**
 * Renders the current customer's personal order history.
 * GET /api/sessions/history/{customerId} — Console Option 8
 */
async function renderHistory() {
  const tbody = document.getElementById("historyTable");
  try {
    const history = await apiFetch(`/sessions/history/${currentCustomer.customerId}`);
    tbody.innerHTML = history.length
      ? history.map(s => `
          <tr>
            <td>${s.sessionId}</td>
            <td>${s.pcId}</td>
            <td>${s.startTime}</td>
            <td>${s.endTime || "—"}</td>
            <td>${money(s.cost)}</td>
            <td>${s.endTime
              ? '<span class="badge warning">Completed</span>'
              : '<span class="badge success">Active</span>'}</td>
          </tr>`).join("")
      : '<tr><td colspan="6">No orders on your account yet.</td></tr>';
  } catch {
    tbody.innerHTML = '<tr><td colspan="6">Could not load order history.</td></tr>';
  }
}

/**
 * Renders the PC availability grid.
 * GET /api/pcs — Console Option 7
 */
async function renderPcGrid() {
  const grid = document.getElementById("pcGrid");
  try {
    const pcs = await apiFetch("/pcs");
    grid.innerHTML = pcs.map(pc => `
      <div class="pc-card">
        <h4>${pc.pcId}</h4>
        <p>Status: ${pc.status === "Available"
          ? '<span class="badge success">Available</span>'
          : '<span class="badge danger">Occupied</span>'}</p>
        <p>${pc.status === "Available"
          ? "Ready — you can order on this machine."
          : "In use — serving an active session."}</p>
        <p class="pc-rate">${money(pc.hourlyRate)}/hr</p>
      </div>`).join("");
  } catch {
    grid.innerHTML = '<p>Could not load PC availability.</p>';
  }
}

/**
 * Fetches the latest balance for the current customer and updates the display.
 * GET /api/customers/{id}
 */
async function refreshBalance() {
  try {
    const customer = await apiFetch(`/customers/${currentCustomer.customerId}`);
    currentCustomer = customer;
    document.getElementById("balanceDisplay").textContent      = money(customer.balance);
    document.getElementById("topupCurrentBalance").value       = money(customer.balance);
  } catch {
    // Keep existing value if refresh fails
  }
}

async function handleStartSession() {
  const customerId = document.getElementById("sessionCustomer").value;

  if (!customerId) {
    showMsg("sessionStartMessage", "Please select a customer.", "error");
    return;
  }

  try {
    const session = await apiFetch("/sessions/start", {
      method: "POST",
      body: JSON.stringify({ customerId })
    });
    showMsg(
      "sessionStartMessage",
      `Session ${session.sessionId} started on ${session.pcId}. Order is now pending.`,
      "success"
    );
    await renderAll();
  } catch (err) {
    showMsg("sessionStartMessage", err.message, "error");
  }
}

/**
 * Ends the selected active session and calculates cost.
 * POST /api/sessions/end — Console Option 2
 * Cost is calculated by CalculateCost(hourlyRate) in Session.cs.
 */
async function handleEndSession() {
  const sessionId = document.getElementById("activeSessionSelect").value;

  if (!sessionId) {
    showMsg("sessionEndMessage", "No active session selected.", "error");
    return;
  }

  try {
    const result = await apiFetch("/sessions/end", {
      method: "POST",
      body: JSON.stringify({ sessionId })
    });
    showMsg(
      "sessionEndMessage",
      `Session ${result.sessionId} ended. Order complete. Cost: ${money(result.cost)}.`,
      "success"
    );
    await renderAll();
  } catch (err) {
    showMsg("sessionEndMessage", err.message, "error");
  }
}

/**
 * Tops up the current customer's balance.
 * POST /api/customers/{id}/topup — Console Option 9
 */
async function handleTopup() {
  const amountInput = document.getElementById("topupAmount");
  const amount      = Number(amountInput.value);

  if (!amount || amount <= 0) {
    triggerInputError(amountInput);
    showMsg("topupMessage", "Please enter a valid positive amount.", "error");
    return;
  }

  try {
    const customer = await apiFetch(`/customers/${currentCustomer.customerId}/topup`, {
      method: "POST",
      body: JSON.stringify({ amount })
    });
    currentCustomer = customer;
    amountInput.value = "";

    document.getElementById("balanceDisplay").textContent = money(customer.balance);
    document.getElementById("topupCurrentBalance").value  = money(customer.balance);

    showMsg("topupMessage", `${money(amount)} added! New balance: ${money(customer.balance)}`, "success");
    await renderStats();
  } catch (err) {
    showMsg("topupMessage", err.message, "error");
  }
}

/**
 * Searches for customers by ID or name.
 * GET /api/customers/{id} — Console Option 3 (by ID)
 * GET /api/customers/search?name= — Console Option 4 (by name)
 * Applies the same banned-word check as ContainsBannedWord in Program.cs.
 */
async function handleSearch() {
  const type     = document.getElementById("searchType").value;
  const rawValue = document.getElementById("searchInput").value.trim();
  const tbody    = document.getElementById("searchResults");

  if (!rawValue) {
    tbody.innerHTML = '<tr><td colspan="5">Please enter a search value.</td></tr>';
    return;
  }

  // Reject banned words — mirrors backend validation
  if (containsBannedWord(rawValue)) {
    triggerInputError(document.getElementById("searchInput"));
    tbody.innerHTML = '<tr><td colspan="5">⚠ Search rejected. Offensive or restricted word detected.</td></tr>';
    return;
  }

  try {
    let results;

    if (type === "id") {
      // Console Option 3 — O(1) average via hash table lookup
      try {
        results = [await apiFetch(`/customers/${rawValue.toUpperCase()}`)];
      } catch {
        results = [];
      }
    } else {
      // Console Option 4 — O(n) name search via GetAll()
      results = await apiFetch(`/customers/search?name=${encodeURIComponent(rawValue)}`);
    }

    if (!results.length) {
      tbody.innerHTML = '<tr><td colspan="5">No customers found.</td></tr>';
      return;
    }

    // Check active sessions to show status column
    const activeSessions = await apiFetch("/sessions/active");
    const activeCustomerIds = new Set(activeSessions.map(s => s.customerId));

    tbody.innerHTML = results.map(c => `
      <tr>
        <td>${c.customerId}</td>
        <td>${c.name}</td>
        <td>${c.email}</td>
        <td>${money(c.balance)}</td>
        <td>${activeCustomerIds.has(c.customerId)
          ? '<span class="badge success">Active Session</span>'
          : '<span class="badge warning">Idle</span>'}</td>
      </tr>`).join("");

  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="5">Error: ${err.message}</td></tr>`;
  }
}
/**
 * Switches the active dashboard tab.
 * @param {string} tabId - e.g. "overview", "start-session"
 */
function switchTab(tabId) {
  // Hide all tab sections
  document.querySelectorAll(".tab-section").forEach(section => {
    section.classList.add("hidden");
  });

  // Deactivate all nav tab buttons
  document.querySelectorAll(".nav-tab").forEach(btn => {
    btn.classList.remove("active");
  });

  // Show the selected tab
  const target = document.getElementById(`tab-${tabId}`);
  if (target) target.classList.remove("hidden");

  // Highlight the matching nav button
  document.querySelectorAll(".nav-tab").forEach(btn => {
    if (btn.dataset.tab === tabId) btn.classList.add("active");
  });
}

document.getElementById("loginBtn").addEventListener("click", handleLogin);
document.getElementById("showRegisterBtn").addEventListener("click", showRegister);
document.getElementById("registerBtn").addEventListener("click", handleRegister);
document.getElementById("showLoginBtn").addEventListener("click", showLogin);
document.getElementById("logoutBtn").addEventListener("click", handleLogout);
document.getElementById("startSessionBtn").addEventListener("click", handleStartSession);
document.getElementById("endSessionBtn").addEventListener("click", handleEndSession);
document.getElementById("topupBtn").addEventListener("click", handleTopup);
document.getElementById("searchBtn").addEventListener("click", handleSearch);

// Allow Enter key in search input
document.getElementById("searchInput").addEventListener("keydown", e => {
  if (e.key === "Enter") handleSearch();
});

// Allow Enter key in login input
document.getElementById("loginId").addEventListener("keydown", e => {
  if (e.key === "Enter") handleLogin();
});

// Tab switching — nav tabs and quick action buttons
document.querySelectorAll(".nav-tab, .qbtn").forEach(btn => {
  btn.addEventListener("click", () => {
    const tabId = btn.dataset.tab;
    if (tabId) switchTab(tabId);
  });
});
