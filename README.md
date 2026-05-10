# 🔍 HamTracker

A Windows desktop application for **project & task management with digital evidence tracking** — built for investigators, auditors, and professionals who need tamper-proof work logs.

---

## ✨ Features

- 🔐 **User Authentication** — Login system with role-based access
- 📁 **Project Management** — Create and manage client projects (Active / Paused / Completed)
- ✅ **Task Tracking** — Tasks with statuses: ToDo → InProgress → Done
- 🖼️ **Evidence Uploads** — Attach files to tasks with SHA-256 hash verification
- 📊 **Dashboard** — Live stats and project overview
- 📅 **Timeline View** — Visual task timeline per project
- 📄 **Report Generation** — Export project reports as PDF or text
- 🗂️ **Audit Logs** — Full activity history for accountability

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Language | C# (.NET Framework 4.8) |
| UI | Windows Forms (WinForms) |
| Database | SQLite (`System.Data.SQLite`) |
| PDF Reports | PDFsharp 6.2 |
| Hashing | SHA-256 via `HashService` |
| Serialization | Newtonsoft.Json |

---

## 🚀 Getting Started

### Prerequisites
- Windows 10/11
- Visual Studio 2022 (with .NET Desktop workload)
- .NET Framework 4.8

### Setup

```bash
# 1. Clone the repo
git clone https://github.com/<your-username>/HamTracker.git

# 2. Open solution
HamTrackerSolution.slnx  →  Open in Visual Studio

# 3. Restore NuGet packages
Tools → NuGet Package Manager → Restore Packages

# 4. Build & Run
Press F5
```

> The SQLite database (`hamtracker.db`) is auto-created on first run via `DatabaseManager.InitializeDatabase()`.

### Default Login
| Username | Password | Role |
|----------|----------|------|
| `admin`  | `admin123` | Admin |

---

## 📂 Project Structure

```
HamTracker/
├── Database/       # Repository classes (CRUD operations)
├── Forms/          # All WinForms UI panels and dialogs
├── Models/         # Data models (Project, TaskItem, User, Evidence)
├── Services/       # Business logic (HashService, ReportService, EvidenceService)
├── Properties/     # Assembly info & resources
└── Program.cs      # Entry point
```

---

## 📸 Screenshots

> *(Add screenshots here after first run)*

---

## 📜 License

This project is for educational purposes. See [LICENSE](LICENSE) for details.
