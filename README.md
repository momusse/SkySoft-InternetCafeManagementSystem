# SkySoft Internet Cafe Management System

A full-stack internet cafe management system built in **C# .NET** with a connected **web-based customer portal**.  
Developed as part of the CST2550 Group Coursework at Middlesex University.

The system consists of two parts:
- A **C# console application** for staff — managing customers, PCs and sessions with a custom data structure implementation and SQL Server database
- A **customer-facing web portal** — a live browser interface connected to the same database via a built-in REST API

## 🎥 Backend Video Demonstration

[▶ Watch Backend Demo](https://drive.google.com/file/d/1pFkpDaAha5UuyIE0cJGs3bOFwxKIzi71/view?usp=sharing)

This video demonstrates:

* Running the C# console application
* API server starting on `http://localhost:5000`
* Console menu functionality (Options 1–11)
* Database interaction with SQL Server
* Core business logic and data structure usage

---

## Requirements

- Visual Studio 2022
- .NET 10.0 SDK
- SQL Server Express
- SQL Server Management Studio (SSMS)
- A modern web browser — Chrome, Edge or Firefox (for the customer portal)

---

## Database Setup

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to `localhost\SQLEXPRESS` using **Windows Authentication**
3. Click **New Query**
4. Open the `database.sql` file found in the `sql/` folder of this repository
5. Paste the contents into the query window and press **F5** to execute
6. This creates the `InternetCafeDB` database and inserts sample data

---

## How to Run

### Step 1 — Clone the repository

```
git clone https://github.com/momusse/SkySoft-InternetCafeManagementSystem
```

### Step 2 — Open the solution in Visual Studio

```
src/InternetCafeManagementSystem/InternetCafeManagementSystem/InternetCafeManagementSystem.slnx
```

### Step 3 — Make sure the database is set up (see above)

### Step 4 — Run the backend

Press **F5** in Visual Studio, or run in terminal:

```
cd src/InternetCafeManagementSystem/InternetCafeManagementSystem
dotnet run
```

When the application starts, you will see:

```
✓ Web API started on http://localhost:5000
  Customer portal (index.html) can now connect to live data.
```

The staff console menu (Options 1–11) will then appear as normal.

### Step 5 — Open the customer portal

1. Navigate to the `frontend/` folder in the repository
2. Right-click `index.html` → **Open with** → Chrome or Edge

The portal will load and display a **green connected banner** confirming it is reading and writing live data from the SQL database.

> The backend must be running before you open the frontend. Without it, no data will load.

---

## How to Use

### Staff Console App

Once running, the main menu appears:

| Option | Description |
|--------|-------------|
| 1 | Start a new session for a customer |
| 2 | End an active session by Session ID |
| 3 | Search for a customer by ID |
| 4 | Search for customers by name |
| 5 | View all sessions |
| 6 | View active sessions only |
| 7 | View PC availability |
| 8 | View session history for a customer |
| 9 | Top up a customer balance |
| 10 | Add a new customer |
| 11 | Exit |

### Customer Portal

The web portal gives customers access to their own account through a browser. It connects live to the same SQL database via the REST API built into the backend.

| Feature | Console Equivalent |
|---------|-------------------|
| Sign in with Customer ID | Option 3 |
| Register a new account | Option 10 |
| View dashboard stats | Options 5, 6, 7 |
| Start a session (place an order) | Option 1 |
| End a session (complete order, calculates cost) | Option 2 |
| Search customers by ID or name | Options 3 & 4 |
| View personal order history | Option 8 |
| Top up account balance | Option 9 |
| View PC availability | Option 7 |
| Sign Out | Option 11 |

---

## Sample Data

The following sample data is preloaded by the SQL script:

**Customers:**
- `C001` — Hasan
- `C002` — Ali
- `C003` — Sara

**PCs:**
- `PC01` — £5.00/hr
- `PC02` — £5.00/hr
- `PC03` — £7.50/hr

---

## Project Structure

```
SkySoft-InternetCafeManagementSystem/
│
├── frontend/
│   ├── index.html               — Customer portal page structure
│   ├── styles.css               — All styling and responsive design
│   └── script.js                — All JavaScript logic and API calls
│
├── sql/
│   └── database.sql             — SQL script to create and seed the database
│
├── src/
│   └── InternetCafeManagementSystem/
│       └── InternetCafeManagementSystem/
│           ├── Api/
│           │   └── CafeApiServer.cs     — REST API layer (connects frontend to backend)
│           ├── Data/
│           │   └── DatabaseHelper.cs    — SQL database read/write operations
│           ├── DataStructures/
│           │   ├── CustomHashTable.cs   — Custom hash table (O(1) average lookup)
│           │   └── CustomLinkedList.cs  — Custom singly linked list (O(n) traversal)
│           ├── Models/
│           │   ├── Customer.cs          — Customer model
│           │   ├── PC.cs                — PC model
│           │   └── Session.cs           — Session model with cost calculation
│           ├── Services/
│           │   └── InternetCafeService.cs — Core business logic layer
│           └── Program.cs               — Entry point, console menu and API startup
│
└── README.md
```

---

## Data Structures

- **CustomHashTable** — Stores customers using `CustomerID` as the key. Provides **O(1) average** time complexity for lookups, additions and existence checks. Uses separate chaining for collision resolution.

- **CustomLinkedList** — Stores PCs and Sessions as a singly linked list. Provides dynamic sizing with **O(n)** traversal for searching and filtering. Implements `IEnumerable` to support foreach loops.

---

## Architecture

```
  Customer Portal (browser)
         │
         │  HTTP fetch → localhost:5000
         ▼
  CafeApiServer.cs  (REST API — runs alongside console app)
         │
         ▼
  InternetCafeService.cs  (business logic)
         │
    ┌────┴────┐
    ▼         ▼
CustomHashTable  CustomLinkedList
 (Customers)     (PCs + Sessions)
    │
    ▼
DatabaseHelper.cs
    │
    ▼
SQL Server — InternetCafeDB
```

All data entered through either the staff console or the customer portal is persisted to the same SQL Server database. Changes made in one interface are immediately reflected in the other.

---

## API Endpoints

The backend exposes the following REST endpoints on `http://localhost:5000`:

| Method | Endpoint | Description | Console Option |
|--------|----------|-------------|----------------|
| GET | `/api/stats` | Dashboard statistics | 5, 6, 7 |
| GET | `/api/customers` | All customers | — |
| GET | `/api/customers/{id}` | Customer by ID | 3 |
| GET | `/api/customers/search?name=` | Search by name | 4 |
| POST | `/api/customers` | Add new customer | 10 |
| POST | `/api/customers/{id}/topup` | Top up balance | 9 |
| GET | `/api/sessions` | All sessions | 5 |
| GET | `/api/sessions/active` | Active sessions | 6 |
| GET | `/api/sessions/history/{id}` | Customer history | 8 |
| POST | `/api/sessions/start` | Start a session | 1 |
| POST | `/api/sessions/end` | End a session | 2 |
| GET | `/api/pcs` | PC availability | 7 |
