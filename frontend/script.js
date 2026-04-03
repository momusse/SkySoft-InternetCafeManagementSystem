const customers = [
  { customerId: "C001", name: "Hasan", email: "hasan@email.com", balance: 10.0 },
  { customerId: "C002", name: "Ali", email: "ali@email.com", balance: 15.0 },
  { customerId: "C003", name: "Sara", email: "sara@email.com", balance: 20.0 },
  { customerId: "C005", name: "Joe", email: "joe@email.com", balance: 100.0 },
  { customerId: "C006", name: "Maruf", email: "maruf@gmail.com", balance: 15.0 },
  { customerId: "P1", name: "Nagi", email: "nagi@email.com", balance: 101.0 }
];

const pcs = [
  { pcId: "PC01", status: "Occupied" },
  { pcId: "PC02", status: "Available" },
  { pcId: "PC03", status: "Available" },
  { pcId: "PC04", status: "Available" }
];

const sessions = [
  { sessionId: "S193940", customerId: "C001", pcId: "PC01", startTime: "2026-03-31 18:39", endTime: "2026-03-31 18:40", cost: 0.09 },
  { sessionId: "S191110", customerId: "C001", pcId: "PC01", startTime: "2026-03-31 19:11", endTime: null, cost: 0.11 },
  { sessionId: "S192040", customerId: "C001", pcId: "PC01", startTime: "2026-03-30 19:20", endTime: "2026-03-30 19:22", cost: 0.16 },
  { sessionId: "S194548", customerId: "C001", pcId: "PC01", startTime: "2026-03-30 19:45", endTime: "2026-03-30 19:49", cost: 0.27 },
  { sessionId: "S194730", customerId: "P1", pcId: "PC02", startTime: "2026-03-30 19:47", endTime: "2026-03-30 19:49", cost: 0.14 }
];

const bannedWords = [
  "fuck","fucking","bitch","porn","sex","sexy","slut","whore","asshole","bastard","dick","cock","pussy","nude","naked","xxx","shit","bullshit","motherfucker","admin","fake"
];

const money = (value) => `£${Number(value).toFixed(2)}`;

function normalizeText(value) {
  return String(value)
    .toLowerCase()
    .replace(/[^a-z0-9]/g, "");
}

function containsBannedWord(value) {
  const cleanValue = normalizeText(value);
  return bannedWords.some((word) => cleanValue.includes(normalizeText(word)));
}

function showMessage(id, text, type) {
  const el = document.getElementById(id);
  el.className = `message show ${type}`;
  el.textContent = text;
}

function customerIsActive(customerId) {
  return sessions.some((s) => s.customerId === customerId && !s.endTime);
}

function updateClock() {
  const now = new Date();
  document.getElementById("liveClock").textContent = now.toLocaleString();
}

function populateCustomerSelects() {
  const selects = ["topupCustomer", "sessionCustomer", "historyCustomer"];
  selects.forEach((id) => {
    const select = document.getElementById(id);
    select.innerHTML = customers
      .map((c) => `<option value="${c.customerId}">${c.customerId} - ${c.name}</option>`)
      .join("");
  });
}

function populatePcSelect() {
  const select = document.getElementById("sessionPc");
  const availablePcs = pcs.filter((pc) => pc.status === "Available");
  select.innerHTML = availablePcs
    .map((pc) => `<option value="${pc.pcId}">${pc.pcId}</option>`)
    .join("");
}

function populateActiveSessionSelect() {
  const select = document.getElementById("activeSessionSelect");
  const active = sessions.filter((s) => !s.endTime);

  if (!active.length) {
    select.innerHTML = '<option value="">No active sessions</option>';
    return;
  }

  select.innerHTML = active
    .map((s) => `<option value="${s.sessionId}">${s.sessionId} - ${s.customerId} - ${s.pcId}</option>`)
    .join("");
}

function renderStats() {
  document.getElementById("customerCount").textContent = customers.length;
  document.getElementById("activeSessionCount").textContent = sessions.filter((s) => !s.endTime).length;
  document.getElementById("availablePcCount").textContent = pcs.filter((pc) => pc.status === "Available").length;
  document.getElementById("sessionCount").textContent = sessions.length;
}

function renderAllSessions() {
  const tbody = document.getElementById("allSessionsTable");
  tbody.innerHTML = sessions
    .map(
      (s) => `
        <tr>
          <td>${s.sessionId}</td>
          <td>${s.customerId}</td>
          <td>${s.pcId}</td>
          <td>${s.startTime}</td>
          <td>${s.endTime || '<span class="badge warning">In progress</span>'}</td>
          <td>${money(s.cost)}</td>
        </tr>
      `
    )
    .join("");
}

function renderActiveSessions() {
  const tbody = document.getElementById("activeSessionsTable");
  const active = sessions.filter((s) => !s.endTime);

  if (!active.length) {
    tbody.innerHTML = '<tr><td colspan="5">No active sessions right now.</td></tr>';
    return;
  }

  tbody.innerHTML = active
    .map(
      (s) => `
        <tr>
          <td>${s.sessionId}</td>
          <td>${s.customerId}</td>
          <td>${s.pcId}</td>
          <td>In progress</td>
          <td><span class="badge success">Active</span></td>
        </tr>
      `
    )
    .join("");
}

function renderPcGrid() {
  const grid = document.getElementById("pcGrid");
  grid.innerHTML = pcs
    .map(
      (pc) => `
        <div class="pc-card">
          <h4>${pc.pcId}</h4>
          <p>Status: ${
            pc.status === "Available"
              ? '<span class="badge success">Available</span>'
              : '<span class="badge danger">Occupied</span>'
          }</p>
          <p>${
            pc.status === "Available"
              ? "Ready for a new customer session."
              : "Currently assigned to an active session."
          }</p>
        </div>
      `
    )
    .join("");
}

function searchCustomers() {
  const type = document.getElementById("searchType").value;
  const rawValue = document.getElementById("searchInput").value.trim();
  const value = rawValue.toLowerCase();
  const tbody = document.getElementById("searchResults");

  if (!rawValue) {
    tbody.innerHTML = '<tr><td colspan="5">Please enter a search value.</td></tr>';
    return;
  }

 if (containsBannedWord(rawValue)) {
  const input = document.getElementById("searchInput");

  triggerInputError(input);

  tbody.innerHTML =
    '<tr><td colspan="5">Search rejected. Please use a valid customer ID or name.</td></tr>';
  return;
}

  const results = customers.filter((c) => {
    if (type === "id") return c.customerId.toLowerCase().includes(value);
    return c.name.toLowerCase().includes(value);
  });

  if (!results.length) {
    tbody.innerHTML = '<tr><td colspan="5">No customer found.</td></tr>';
    return;
  }

  tbody.innerHTML = results
    .map(
      (c) => `
        <tr>
          <td>${c.customerId}</td>
          <td>${c.name}</td>
          <td>${c.email}</td>
          <td>${money(c.balance)}</td>
          <td>${
            customerIsActive(c.customerId)
              ? '<span class="badge success">Active Session</span>'
              : '<span class="badge warning">Idle</span>'
          }</td>
        </tr>
      `
    )
    .join("");
}

function triggerInputError(...elements) {
  elements.forEach((el) => {
    if (!el) return;
    el.classList.add("input-error");

    setTimeout(() => {
      el.classList.remove("input-error");
    }, 500);
  });
}

function getRestrictedFields(fields) {
  const invalidFields = [];

  fields.forEach((field) => {
    if (containsBannedWord(field.value)) {
      invalidFields.push(field);
    }
  });

  return invalidFields;
}

function loadHistory() {
  const customerId = document.getElementById("historyCustomer").value;
  const rows = sessions.filter((s) => s.customerId === customerId);
  const tbody = document.getElementById("historyTable");
  const selectedCustomer = customers.find((c) => c.customerId === customerId);

  document.getElementById("historyInfo").value = selectedCustomer
    ? `${selectedCustomer.customerId} - ${selectedCustomer.name}`
    : "";

  if (!rows.length) {
    tbody.innerHTML = '<tr><td colspan="5">No session history for this customer.</td></tr>';
    return;
  }

  tbody.innerHTML = rows
    .map(
      (s) => `
        <tr>
          <td>${s.sessionId}</td>
          <td>${s.pcId}</td>
          <td>${s.startTime}</td>
          <td>${s.endTime || "In progress"}</td>
          <td>${money(s.cost)}</td>
        </tr>
      `
    )
    .join("");
}
document.getElementById("customerForm").addEventListener("submit", (e) => {
  e.preventDefault();

  const customerIdInput = document.getElementById("customerId");
  const customerNameInput = document.getElementById("customerName");
  const customerEmailInput = document.getElementById("customerEmail");
  const balanceInput = document.getElementById("customerBalance");

  const customerId = customerIdInput.value.trim();
  const name = customerNameInput.value.trim();
  const email = customerEmailInput.value.trim();
  const balance = Number(balanceInput.value);

  if (!customerId || !name || !email || Number.isNaN(balance)) {
    showMessage("customerMessage", "Please complete all fields.", "error");
    return;
  }

  const restrictedFields = getRestrictedFields([
    { element: customerIdInput, label: "Customer ID", value: customerId },
    { element: customerNameInput, label: "Customer Name", value: name },
    { element: customerEmailInput, label: "Email", value: email }
  ]);

  if (restrictedFields.length > 0) {
    triggerInputError(...restrictedFields.map((field) => field.element));

    const fieldNames = restrictedFields.map((field) => field.label).join(", ");

    showMessage(
      "customerMessage",
      `Input rejected. Restricted or offensive word was found in: ${fieldNames}.`,
      "error"
    );
    return;
  }

  if (customers.some((c) => c.customerId.toLowerCase() === customerId.toLowerCase())) {
    triggerInputError(customerIdInput);
    showMessage("customerMessage", "Customer ID already exists. IDs must be unique.", "error");
    return;
  }

  customers.push({ customerId, name, email, balance });
  populateCustomerSelects();
  renderStats();

  showMessage(
    "customerMessage",
    `Customer added successfully: ${customerId} - ${name} - Balance ${money(balance)}`,
    "success"
  );

  e.target.reset();
});

document.getElementById("customerName").addEventListener("input", (e) => {
  if (containsBannedWord(e.target.value)) {
    e.target.classList.add("input-error");
  } else {
    e.target.classList.remove("input-error");
  }
});

document.getElementById("topupForm").addEventListener("submit", (e) => {
  e.preventDefault();

  const customerId = document.getElementById("topupCustomer").value;
  const amount = Number(document.getElementById("topupAmount").value);
  const customer = customers.find((c) => c.customerId === customerId);

  if (!customer || Number.isNaN(amount) || amount <= 0) {
    showMessage("topupMessage", "Please enter a valid top-up amount.", "error");
    return;
  }

  customer.balance += amount;
  showMessage("topupMessage", `${customer.name}'s balance was updated. New balance: ${money(customer.balance)}`, "success");
  e.target.reset();
});

document.getElementById("startSessionForm").addEventListener("submit", (e) => {
  e.preventDefault();

  const customerId = document.getElementById("sessionCustomer").value;
  const pcId = document.getElementById("sessionPc").value;
  const pc = pcs.find((p) => p.pcId === pcId);

  if (!customerId || !pcId || !pc || pc.status !== "Available") {
    showMessage("sessionStartMessage", "Please select a valid available PC.", "error");
    return;
  }

  const sessionId = `S${Date.now().toString().slice(-6)}`;

  sessions.unshift({
    sessionId,
    customerId,
    pcId,
    startTime: new Date().toLocaleString(),
    endTime: null,
    cost: 0.0
  });

  pc.status = "Occupied";
  renderAllSessions();
  renderActiveSessions();
  renderPcGrid();
  renderStats();
  populatePcSelect();
  populateActiveSessionSelect();

  showMessage("sessionStartMessage", `Session ${sessionId} started for ${customerId} on ${pcId}.`, "success");
});

document.getElementById("endSessionForm").addEventListener("submit", (e) => {
  e.preventDefault();

  const sessionId = document.getElementById("activeSessionSelect").value;
  const session = sessions.find((s) => s.sessionId === sessionId);

  if (!session || session.endTime) {
    showMessage("sessionEndMessage", "No valid active session selected.", "error");
    return;
  }

  session.endTime = new Date().toLocaleString();
  session.cost = 0.5;

  const pc = pcs.find((p) => p.pcId === session.pcId);
  if (pc) pc.status = "Available";

  renderAllSessions();
  renderActiveSessions();
  renderPcGrid();
  renderStats();
  populatePcSelect();
  populateActiveSessionSelect();

  showMessage("sessionEndMessage", `Session ${session.sessionId} ended successfully. Cost: ${money(session.cost)}.`, "success");
});

document.getElementById("searchButton").addEventListener("click", searchCustomers);
document.getElementById("historyButton").addEventListener("click", loadHistory);

updateClock();
setInterval(updateClock, 1000);
populateCustomerSelects();
populatePcSelect();
populateActiveSessionSelect();
renderStats();
renderAllSessions();
renderActiveSessions();
renderPcGrid();
loadHistory();