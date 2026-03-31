CREATE DATABASE InternetCafeDB;
GO

USE InternetCafeDB;
GO

CREATE TABLE Customers (
    CustomerID VARCHAR(10) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Balance DECIMAL(10, 2) NOT NULL
);
GO

CREATE TABLE PCs (
    PCID VARCHAR(10) PRIMARY KEY,
    HourlyRate DECIMAL(10, 2) NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Sessions (
    SessionID VARCHAR(50) PRIMARY KEY,
    CustomerID VARCHAR(10) NOT NULL,
    PCID VARCHAR(10) NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NULL,
    Cost DECIMAL(10, 2) NULL,
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
    FOREIGN KEY (PCID) REFERENCES PCs(PCID)
);
GO

-- Sample data
INSERT INTO Customers VALUES ('C001', 'Hasan', 'hasan@email.com', 10.00);
INSERT INTO Customers VALUES ('C002', 'Ali', 'ali@email.com', 15.00);
INSERT INTO Customers VALUES ('C003', 'Sara', 'sara@email.com', 20.00);

INSERT INTO PCs VALUES ('PC01', 5.00, 1);
INSERT INTO PCs VALUES ('PC02', 5.00, 1);
INSERT INTO PCs VALUES ('PC03', 7.50, 1);
GO