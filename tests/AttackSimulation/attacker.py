import requests
import threading
import time
import sys

# --- CONFIGURATION ---
TARGET_URL = "http://localhost:5218/api/content/4" 
TOTAL_REQUESTS = 100
CONCURRENCY = 20

# ---------------------------------------------------------
# 🔑 IDENTITY SWITCH (Uncomment the one you want to test)
# ---------------------------------------------------------

# SCENARIO 1: The "Free" User (Capacity: 10)
# API_KEY = None 

# SCENARIO 2: The "Gold" User (Capacity: 50)
API_KEY = "GOLD-USER-777" 

# ---------------------------------------------------------

lock = threading.Lock()
stats = {"200": 0, "429": 0, "Fail": 0}

def attack(worker_id):
    try:
        # Prepare Headers
        start = time.time()
        headers = {}
        if API_KEY:
            headers["X-Api-Key"] = API_KEY

        # Send Request with Headers
        response = requests.get(TARGET_URL, headers=headers, timeout=5, verify=False)
        duration = (time.time() - start) * 1000

        with lock:
            if response.status_code == 200:
                stats["200"] += 1
                # Optional: Print success only occasionally to reduce clutter
                if stats["200"] % 10 == 0: 
                    print(f"✅ Worker {worker_id}: Request Allowed ({duration:.0f}ms)")
            
            elif response.status_code == 429:
                stats["429"] += 1
                print(f"⛔ Worker {worker_id}: BLOCKED by Rate Limiter")
            
            else:
                stats["Fail"] += 1
                print(f"⚠️ Worker {worker_id}: Unexpected Status {response.status_code}")

    except Exception as e:
        with lock:
            stats["Fail"] += 1
            print(f"❌ Worker {worker_id}: Connection Failed ({str(e)})")

# --- EXECUTION ---
print(f"\n🚀 STARTING DDoS SIMULATION")
print(f"🎯 Target: {TARGET_URL}")
print(f"💥 Payload: {TOTAL_REQUESTS} requests")
print("-" * 40)

threads = []
start_time = time.time()

# Launch Threads
for i in range(TOTAL_REQUESTS):
    t = threading.Thread(target=attack, args=(i,))
    threads.append(t)
    t.start()
    
    # Slight delay to batch threads (simulates realistic burst)
    if i % CONCURRENCY == 0:
        time.sleep(0.05) 

# Wait for completion
for t in threads:
    t.join()

duration = time.time() - start_time

# --- REPORT CARD ---
print("\n" + "="*40)
print(f"📊 ATTACK SUMMARY")
print("="*40)
print(f"⏱️  Duration:      {duration:.2f} seconds")
print(f"🚀 Total Sent:    {TOTAL_REQUESTS}")
print(f"🟢 PASSED (200):  {stats['200']}  <-- Legitimate Traffic")
print(f"🔴 BLOCKED (429): {stats['429']}  <-- Mitigated Attacks")
print(f"❌ FAILED:        {stats['Fail']}")
print("="*40)