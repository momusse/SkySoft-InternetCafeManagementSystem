# SkySoft Internet Cafe — Customer Portal (Frontend)

This folder contains the customer-facing web frontend for the **SkySoft Internet Cafe Management System**, built as part of the CST2550 Group Coursework at Middlesex University.

The frontend is a fully interactive web interface built in HTML, CSS and JavaScript. It connects directly to the C# backend via a REST API, reading and writing live data from the SQL Server database.

---

## 🌐 Quick Design Preview (No Setup Required)

If you just want to see the visual design and layout of the customer portal without setting up the backend, you can view a static prototype here:

**👉 [View Design Prototype](https://marufspace.xo.je/skysoft/)**

> ⚠️ **Important:** This preview link is a **design prototype only**. It does not connect to a real database and uses placeholder data for visual demonstration purposes only.
>
> To use the fully working system with live data from the SQL database, follow the full setup instructions below.

---

## Requirements

The frontend **will not fetch or display real data** without the following being installed and running:

- **Visual Studio 2022**
- **.NET 10.0 SDK**
- **SQL Server Express** (`localhost\SQLEXPRESS`)
- **SQL Server Management Studio (SSMS)**
- A modern web browser (Chrome, Edge, Firefox)

Without SQL Server Express running and the database set up, the frontend will show a red error banner and no data will load. The sign-in will not work and all features will be unavailable.

---

## Languages Used

- HTML
- CSS
- JavaScript

---

## Features

The customer portal mirrors the core functionality of the C# console application and maps directly to its menu options:

| Feature | Console Option |
|---------|---------------|
| Sign in with Customer ID | Option 3 — Search by ID |
| Register a new account | Option 10 — Add Customer |
| View dashboard stats (total customers, active sessions, available PCs, total sessions) | Options 5, 6, 7 |
| Start a session (place an order on a machine) | Option 1 — Start Session |
| End a session (complete order, calculates cost) | Option 2 — End Session |
| Search customers by ID or name | Options 3 & 4 |
| View personal order history | Option 8 — Session History |
| Top up account balance | Option 9 — Top Up Balance |
| View PC availability | Option 7 — PC Availability |
| Sign Out | Option 11 — Exit  |

---

## Input Validation

The frontend applies the same validation rules as the C# backend (`ContainsBannedWord` in `Program.cs`):

- Required field checks on all inputs
- Numeric validation for balance and top-up amounts (positive values only)
- Unique Customer ID enforcement (duplicate IDs rejected)
- Banned and offensive word filtering — identical word list to the backend
- Visual feedback with shake animation and red highlight on invalid fields
- Real-time validation on the name field as you type

---

## How It Connects to the Backend

When the C# console app runs, it simultaneously starts a REST API on `http://localhost:5000` via `CafeApiServer.cs`. The frontend communicates with this API using standard `fetch()` calls in JavaScript.

All data flows through the existing `InternetCafeService.cs` — the API adds no new business logic, it simply exposes the service methods over HTTP.

```
index.html / script.js
       │
       │  HTTP fetch (localhost:5000)
       ▼
CafeApiServer.cs  ──►  InternetCafeService.cs  ──►  SQL Database
```

The dashboard displays a **green banner** when successfully connected to the backend, and a **red banner** if the backend is not running.

---

## How to Run (Full Working System)

### Step 1 — Set up the database

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to `localhost\SQLEXPRESS` using **Windows Authentication**
3. Click **New Query**
4. Open the `database.sql` file from the `sql/` folder of the repo
5. Paste the contents into the query window and press **F5** (Execute)
6. This creates the `InternetCafeDB` database and inserts sample data

### Step 2 — Run the C# backend

1. Open Visual Studio 2022
2. Go to **File → Open → Project/Solution**
3. Navigate to `src/InternetCafeManagementSystem/InternetCafeManagementSystem/`
4. Open `InternetCafeManagementSystem.slnx`
5. Press **F5** to run

You will see this message in the console confirming the API is ready:

```
✓ Web API started on http://localhost:5000
```

The console menu (Options 1–11) will then appear as normal.

### Step 3 — Open the customer portal

1. Open **File Explorer**
2. Navigate to the `frontend/` folder in the repo
3. Right-click `index.html` → **Open with** → Chrome or Edge

The page will load and show a **green connected banner** at the top confirming it is pulling live data from your SQL database.

Sign in using one of the sample customer IDs:
- `C001` — Hasan
- `C002` — Ali
- `C003` — Sara

---

## File Structure

```
frontend/
├── index.html    — Page structure and layout (no inline styles or scripts)
├── styles.css    — All styling and responsive design
└── script.js     — All JavaScript logic, API calls and input validation
```

---

## Notes

- The frontend must be opened **as a local file** directly in the browser (not through a separate web server)
- The C# backend must be running **before** opening the frontend, otherwise no data will load
- Data entered through the frontend (new customers, sessions, top-ups) is saved directly to the SQL database and will be visible in the console app immediately
- The design follows the same visual style as the staff management panel for a consistent look across both interfaces
