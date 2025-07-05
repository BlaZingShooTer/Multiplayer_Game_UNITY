# Multiplayer_Game_UNITY
# 🕹️ 2D Multiplayer Game in Unity

This is a 2D multiplayer game built in Unity using **Netcode for GameObjects** and **Unity Gaming Services**. It features a host-client architecture where one player acts as the host (server + player), and others can join via lobby or join codes.

---

## 🚀 Features

- 🔗 **Host-Client Architecture**: One player hosts the game while others join as clients
- 🧠 **Game Objective**: Players collect coins to unlock the ability to shoot and heal themselves
- 🎮 **Real-Time Multiplayer**: Movement and interactions are synced across all players
- 🔄 **Network Synchronization**: Player movement, shooting, health, and pickups synced via Netcode
- 🧩 Built with **Unity 202X.X** and **Netcode for GameObjects**

---

## 🎮 Controls

| Action            | Control      |
|-------------------|--------------|
| Move              | `W` `A` `S` `D` |
| Shoot             | `Space`      |
| Aim               | Mouse Cursor |
| Heal (when available) | Auto or action-triggered depending on gameplay setup |

> 🪙 Collect coins to gain the ability to **shoot** and **heal**

---

## 🛠️ Tech Stack

| Tool | Description |
|------|-------------|
| **Unity** | Game engine (2D setup) |
| **Netcode for GameObjects** | Networking and synchronization |
| **Unity Gaming Services** | Lobby, Relay, and Multiplayer Backend |
| **C#** | Game logic and scripting |
| **GIT** | Version control |

---

## 📷 Screenshots / GIFs

*(Add screenshots or gameplay GIFs here if available)*

---

## 🧩 How It Works

1. **Player clicks "Host"** → Initializes the game and acts as the server
2. **Other players click "Join"** → Enter join code or connect via lobby
3. Game session starts once connected
4. Players collect coins to unlock actions, then aim and shoot at opponents

---

## 🧪 Getting Started (Local Testing)

1. Clone this repository
2. Open the project in Unity (version X.XX.X or later)
3. Go to `Scenes/MultiplayerScene.unity`
4. Press **Play** and choose **Host** or **Client**
5. For actual network testing, use **Relay/Lobby** from Unity Gaming Services

---

## 📝 Contributions

This project was created as a personal learning experience to explore real-time networking in Unity using official tools. Future improvements may include:

- Additional gameplay mechanics
- Deploy the game on server
- Better disconnection handling and error flow

---


