# SkySoft Internet Cafe Management System

A C# .NET console application for managing customers, PCs and sessions in an internet cafe.
Built as part of the CST2550 Group Coursework at Middlesex University.

## Requirements

- Visual Studio 2022
- .NET 10.0 SDK
- SQL Server Express
- SQL Server Management Studio (SSMS)

## Database Setup

1. Open SQL Server Management Studio
2. Connect to `localhost\SQLEXPRESS` using Windows Authentication
3. Click New Query
4. Open and run the `database.sql` file found in the `src/InternetCafeManagementSystem` folder
5. This will create the `InternetCafeDB` database and insert sample data

## How to Run

1. Clone the repository:
```
git clone https://github.com/momusse/SkySoft-InternetCafeManagementSystem
```

2. Open the solution file in Visual Studio:
```
src/InternetCafeManagementSystem/InternetCafeManagementSystem.slnx
```

3. Make sure the database is set up (see above)

4. Press `F5` or run in terminal:
```
cd src/InternetCafeManagementSystem/InternetCafeManagementSystem
dotnet run
```

## How to Use

Once running you will see the main menu:

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

## Sample Data

The following sample data is preloaded by the SQL script:

**Customers:**
- C001 — Hasan
- C002 — Ali
- C003 — Sara

**PCs:**
- PC01 — £5.00/hr
- PC02 — £5.00/hr
- PC03 — £7.50/hr

## Project Structure
```
InternetCafeManagementSystem/
├── Data/
│   └── DatabaseHelper.cs        — SQL database operations
├── DataStructures/
│   ├── CustomHashTable.cs       — Custom hash table implementation
│   └── CustomLinkedList.cs      — Custom linked list implementation
├── Models/
│   ├── Customer.cs              — Customer model
│   ├── PC.cs                    — PC model
│   └── Session.cs               — Session model
├── Services/
│   └── InternetCafeService.cs   — Business logic layer
└── Program.cs                   — Entry point and menu
```

## Data Structures

- **CustomHashTable** — Used to store customers. Provides O(1) average time complexity for lookups using separate chaining for collision resolution.
- **CustomLinkedList** — Used to store PCs and Sessions. Provides dynamic sizing with O(n) traversal.