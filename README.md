# 🧾 PREVNAME — Discord Username History Bot

**PrevName** is a lightweight and efficient Discord bot designed to **track global Discord username changes**.  
It focuses on **global usernames only** (not server nicknames) and provides a clean, reliable slash-command experience.

---

## ✨ Key Features

- 🧠 Global Discord username history tracking
- 🌍 Tracks **usernames only** (no guild nicknames)
- 📜 View previous usernames with clean embeds
- 🗑️ Clear history (restricted to system owner)
- 🧱 Lightweight, fast, and stable
- 🗄️ SQLite database (local & persistent)
- ⚡ 100% Slash Commands
- 🧼 Clean and minimal embeds

---

## 🧱 Project Structure

```
PrevNameBot/
├── Commands/              # Slash commands
├── Database/              # SQLite database & handlers
├── Events/                # Discord events (user update)
├── Program.cs             # Main entry point
├── config.json            # Bot configuration
├── PrevName.db            # SQLite database (auto-created)
└── README.md
```

---

## ⚙️ Requirements

- **.NET 7.0 or higher**
- A Discord bot application
- Linux or Windows VPS / machine
- Internet connection

---

## 📦 Installation

### 1️⃣ Install .NET (Linux)

```bash
sudo apt update
sudo apt install -y dotnet-sdk-7.0
```

### 2️⃣ Extract the project

```bash
unzip PrevNameBot.zip
cd PrevNameBot
```

### 3️⃣ Restore dependencies

```bash
dotnet restore
```

---

## 🔑 Configuration

Edit the `config.json` file:

```json
{
  "Token": "YOUR_BOT_TOKEN",
  "ClientId": "YOUR_CLIENT_ID",
  "SysId": "YOUR_USER_ID"
}
```

### 🔒 Notes
- `SysId` is the **only user allowed to clear history**
- **Never share your bot token**

---

## ▶️ Running the Bot

### Development
```bash
dotnet run
```

### Production (recommended)
```bash
dotnet publish -c Release
cd bin/Release/net7.0
dotnet PrevName.dll
```

---

## 🧾 Slash Commands

| Command | Description |
|-------|------------|
| `/prevname` | Display a user's global username history |
| `/prevname clear` | Clear all stored username history (**Sys only**) |

📌 History is **global only**  
📌 Server nicknames are **not tracked**

---

## 🗄️ Database

- Uses **SQLite**
- Automatically created on first launch
- Stores:
  - User ID
  - Previous usernames
  - Last known username
  - Timestamps

No external database required.

---

## 🔐 Required Permissions

- Read Messages
- Send Messages
- Embed Links
- Use Slash Commands

Administrator permission is **not required**.

---

## ⚠️ Important Notes

- Tracks **Discord usernames only**
- Does **not** track:
  - Server nicknames
  - Display names
- Bot must remain online to record changes
- One instance works across multiple servers

---

## 🛠️ Troubleshooting

- Slash commands not showing:
  - Restart the bot
  - Re-invite the bot with `applications.commands`
- History not saving:
  - Check file permissions
  - Ensure SQLite file is writable

---

## 📜 License

This project is intended for **private and educational use**.  
Redistribution or commercial use without permission is prohibited.

---

## 🤝 Support

For bugs or improvements:
- Check console logs
- Contact the developer directly if needed

---

⭐ If you find this project useful, keep it updated and secure.
