# Threx - Process Monitor & Enforcer

Threx is a lightweight Windows console application built in C# that monitors running processes on a system and enforces application usage rules. It automatically terminates unapproved processes, logs all actions to a file, and sends the log file via email when the program exits (either by user command or system shutdown).

---

## Features

- Process Monitoring  
  Scans all running processes at fixed intervals (every 30 seconds by default).

- Allowed Applications  
  Only processes explicitly listed in `allowedApps` are allowed to run without termination.

- Auto-Termination of Unapproved Processes  
  - Kills predefined processes from `processesToStop`.
  - Kills any other process not belonging to Microsoft/Windows unless whitelisted.

- Logging  
  - Logs all terminated processes with timestamps and process IDs.
  - Log file is stored at:
    C:\Users\<YourUsername>\Documents\projects\supervisor\process_log.txt

- Email Report on Exit  
  - Sends the log file to a specified email address (simeonmnaan@gmail.com) when:
    - The user stops the program by pressing `P`.
    - The console closes (Ctrl+C or system shutdown).

---

## Requirements

1. .NET Framework or .NET Core  
   This project can be built with .NET Framework 4.7.2+ or .NET Core 3.1/5.0+.

2. SMTP Email Configuration  
   - A Gmail account for sending emails.
   - If using Gmail with 2FA, generate an App Password (https://support.google.com/accounts/answer/185833?hl=en).
   - Update these credentials in the `SendLogFile()` method:
     ```csharp
     string senderEmail = "your_email@gmail.com";
     string senderPassword = "your_app_password";
     string recipientEmail = "simeonmnaan@gmail.com";
     ```

3. Permissions  
   The program may need Administrator privileges to terminate certain processes.

---

## Setup and Usage

### 1. Clone or Copy the Project
Copy the source code into your desired directory.

### 2. Configure Email Credentials
Edit the `SendLogFile()` method in `Threx.cs` to include your Gmail and app password.

### 3. Build the Application
Using the .NET CLI:


### work in progress
