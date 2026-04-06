# SkySoft - Internet Cafe Management System (Frontend)

This folder contains the web-based frontend interface for the SkySoft Internet Cafe Management System.  
It has been developed using HTML, CSS, and JavaScript as part of the CST2550 Group Coursework at Middlesex University.

## Overview

The frontend provides a visual user interface that mirrors the functionality of the C# console application.  
It is designed to improve usability by presenting the system features in a structured and interactive web layout.

## Languages Used

- HTML5
- CSS3
- JavaScript

## Features

The frontend implements the same core features as the console application:

- Add Customer
- Start Session
- End Session
- Search Customer (by ID and Name)
- View All Sessions
- View Active Sessions
- View PC Availability
- View Customer Session History
- Top Up Customer Balance

## Input Validation

The interface includes user input validation to improve data integrity:

- Required field validation
- Numeric validation for balances
- Unique Customer ID checks
- Banned/restricted word filtering
- Visual feedback (error messages and animations)

## Data Handling

- Data is currently stored **temporarily in browser memory (JavaScript arrays)**
- No persistent storage is used in this layer
- Data will reset on page refresh

## Relationship to Backend

This frontend is designed as a **prototype interface** for the system.

The main system logic and database operations are implemented in the C# backend located in the `src/` folder.

At this stage:
- The frontend is **not directly connected** to the backend or SQL database
- It simulates functionality based on the console application design

Future improvement could include integrating the frontend with a C# Web API for full system interaction.

## How to Run

1. Navigate to the `frontend/` folder
2. Open the `index.html` file in any modern web browser (Chrome, Edge, etc.)

No additional setup or installation is required.

## Notes

- This interface was developed to demonstrate how a console-based system can be translated into a graphical web interface
- The design is based on the existing menu structure and database model from the backend system
