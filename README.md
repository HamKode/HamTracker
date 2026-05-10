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
git clone https://github.com/HamKode/HamTracker

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


<img width="1348" height="849" alt="Screenshot 2026-05-10 114221" src="https://github.com/user-attachments/assets/e01d2124-5ec0-41b2-b426-504683fcf68d" />
<img width="1176" height="746" alt="Screenshot 2026-05-10 233219" src="https://github.com/user-attachments/assets/4a6d7833-c183-43a9-ae01-f86685c6ac0d" />
<img width="1767" height="942" alt="Screenshot 2026-05-10 114340" src="https://github.com/user-attachments/assets/8ef22a1b-f9a5-4dcd-a2fe-14d50610ea74" />
<img width="1767" height="1075" alt="Screenshot 2026-05-10 114317" src="https://github.com/user-attachments/assets/cbbc034e-5937-42c0-8846-7be2d75a2669" />
<img width="879" height="625" alt="Screenshot 2026-05-10 232939" src="https://github.com/user-attachments/assets/dd5f9c68-d30b-4bc3-aef6-9ce7b7c5d23a" />
<img width="1348" height="849" alt="Screenshot 2026-05-10 114221" src="https://github.com/user-attachments/assets/a75392d4-5aa5-4f6d-b0f3-6340e41060d2" />

---

## 📜 License

This project is for educational purposes. See [LICENSE](LICENSE) for details.
