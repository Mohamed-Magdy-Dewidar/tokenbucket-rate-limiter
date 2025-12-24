

# 🛡️ Tiered Token Bucket Rate Limiter

### DDoS Mitigation for Microservices using ASP.NET Core & Python

## 📖 Overview

This project demonstrates a robust **Application-Layer (Layer 7) Rate Limiter** designed to protect microservices from Distributed Denial of Service (DDoS) attacks.

Implemented in **C# (ASP.NET Core)**, the system acts as an API Gateway that enforces traffic quotas using the **Token Bucket Algorithm**. It features a **Tiered Security Policy** system that dynamically adjusts limits based on user identity (Free vs. VIP), proving how business logic can integrate with security architecture.

To validate the defense mechanism, the project includes a **multi-threaded Attack Simulation** tool written in Python.

---

## 🏗️ Architecture

The system consists of three main components:

1. **The Shield (API Gateway):** A reverse proxy that intercepts all traffic. It holds the "Token Buckets" in memory and enforces rate limits before forwarding requests.
2. **The Target (Core Service):** A protected microservice that simulates high-latency processing.
3. **The Attacker (Load Tester):** A Python script that launches concurrent HTTP floods to test the system's resilience.

---

## ✨ Key Features

* **Token Bucket Algorithm:** Implements "Lazy Refill" logic for O(1) performance efficiency.
* **Thread-Safe Implementation:** Uses C# `lock` primitives to handle high-concurrency race conditions.
* **Tiered Rate Limiting:**
* **Free Tier:** 10 Request Burst / 1 Refill per sec.
* **Gold Tier (VIP):** 50 Request Burst / 5 Refill per sec.


* **Real-Time Dashboard:** A visual web monitor (Chart.js) showing Allowed vs. Blocked traffic live.
* **Attack Simulation:** A configurable Python script to stress-test the Gateway.

---

## 🛠️ Technologies Used

* **Backend:** ASP.NET Core Web API (.NET 9)
* **Scripting:** Python 3 (Requests, Threading)
* **Frontend:** HTML5, Chart.js (for the Dashboard)
* **Tools:** Visual Studio Code / Visual Studio 2022

---

## 📂 Project Structure

```text
RateLimiterProject/
│
├── src/
│   ├── ApiGateway/           # The Shield (Port 5000/5218)
│   │   ├── Middleware/       # RateLimitingMiddleware.cs
│   │   ├── Services/         # TokenBucket.cs logic
│   │   └── wwwroot/          # Dashboard HTML/JS
│   │
│   └── CoreService/          # The Target (Port 5001)
│       └── Controllers/      # DataController.cs (Simulated Lag)
│
├── tests/
│   └── AttackSimulation/     # The Attacker
│       └── attacker.py       # Multi-threaded load tester
│
└── README.md

```

---

## 🚀 Getting Started

### Prerequisites

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
* [Python 3.x](https://www.python.org/downloads/)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/tiered-rate-limiter.git
cd tiered-rate-limiter

```


2. **Install Python Dependencies** (for the attacker script)
```bash
pip install requests

```



---

## ⚡ How to Run the Demo

### Step 1: Start the Core Service (The Victim)

Open a terminal in the `src/CoreService` folder:

```bash
cd src/CoreService
dotnet run

```

*It will listen on port 5001.*

### Step 2: Start the API Gateway (The Shield)

Open a new terminal in the `src/ApiGateway` folder:

```bash
cd src/ApiGateway
dotnet run

```

*It will listen on port 5000 (or 5218 depending on launchSettings).*

### Step 3: Open the Dashboard

Open your browser and navigate to: `http://localhost:5218` (or the port shown in your Gateway terminal).
*You should see the "API Gateway Live Monitor" with 0 traffic.*

### Step 4: Launch the Attack 💥

Open a third terminal in `tests/AttackSimulation`:

**Scenario A: Free Tier Attack (Default)**
Run the script without modification.

```bash
python attacker.py

```

* **Expected Result:** ~10 Requests Allowed, ~90 Blocked. The Dashboard turns **RED**.

**Scenario B: Gold VIP Attack**
Edit `attacker.py` and uncomment the API Key line:

```python
API_KEY = "GOLD-USER-777"

```

Run the script again.

* **Expected Result:** ~58 Requests Allowed. The Dashboard turns **GREEN**.

---

## 📊 Results Snapshot

| Metric | Free Tier (Anonymous) | Gold Tier (VIP Key) |
| --- | --- | --- |
| **Burst Capacity** | 10 Tokens | 50 Tokens |
| **Refill Rate** | 1 req/sec | 5 req/sec |
| **Result (100 reqs)** | 🛡️ **~90% Blocked** | ⚡ **~58% Allowed** |

---

## 🔮 Future Improvements

* **Distributed Caching:** Currently, buckets are stored in-memory. In a scaled environment with multiple Gateways, state should be moved to **Redis** to prevent "split-brain" rate limiting.
* **IP Whitelisting:** Add a middleware layer to permanently ban IPs that repeatedly hit the 429 limit.

---

## 📜 License

This project is licensed under the MIT License - see the LICENSE file for details.

---
