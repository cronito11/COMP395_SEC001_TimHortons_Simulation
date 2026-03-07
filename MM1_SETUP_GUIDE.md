# M/M/1 Queue Simulation - Setup Guide

## Your Excel Data Summary

### Inter-Arrival Times (from your Excel):
- **Mean**: 104.8077 seconds
- **Arrival Rate (?)**: 34.35 cars/hour
- **Distribution**: Available as observed data in bins

### Service Times (from your Excel):
- **Mean**: 113.8846 seconds  
- **Service Rate (?)**: 31.6 cars/hour
- **Distribution**: Available as observed data in bins

### ?? IMPORTANT NOTES:
1. Your observed data shows **? > ?** (34.35 > 31.6), creating an **UNSTABLE** queue!
2. For stable M/M/1: ? must be > ?
3. **Recommended adjustment**: Set ? = 30 and ? = 35 for experiments

---

## Updated Code Files

### 1. **ArrivalProcess.cs**
- ? Set default `arrivalRateAsCarsPerHour = 34.35` (from your data)
- ? Set default strategy to `ExponentialIntervalTime` for M/M/1
- ? Added observed distribution arrays from your Excel bins
- ? Added `totalArrivals` counter
- ? Added `UpdateArrivalRate()` method for UI control
- ? Fixed exponential distribution calculation (Lambda = rate/3600 for seconds)
- ? Fixed observed distribution (removed *60, now directly in seconds)

**Excel Data Already Entered:**
```csharp
public float[] xs = { 9f, 50f, 100f, 150f, 200f, 250f, 283f };
public float[] ys = { 0f, 0.08f, 0.38f, 0.67f, 0.81f, 0.96f, 1f };
```

### 2. **ServiceProcess.cs**
- ? Set default `serviceRateAsCarsPerHour = 31.6` (from your data)
- ? Set default strategy to `ExponentialIntervalTime` for M/M/1
- ? Added observed distribution arrays for service times
- ? Added `totalServiced` counter and `totalServiceTime` tracking
- ? Added `UpdateServiceRate()` method for UI control
- ? Added `ChangeServiceStrategy()` method for UI
- ? Fixed exponential distribution calculation

**Excel Data Already Entered:**
```csharp
public float[] serviceTimesXs = { 5f, 50f, 100f, 150f, 200f, 250f, 300f, 399f };
public float[] serviceTimesYs = { 0f, 0.18f, 0.38f, 0.58f, 0.73f, 0.85f, 0.96f, 1f };
```

### 3. **QueueManager.cs** (Enhanced)
- ? Added M/M/1 statistics tracking:
  - Current queue length
  - Average queue length (Lq)
  - Max queue length
  - Average wait time (Wq)
  - Total customers served
  - Utilization (? = ?/?)
- ? Added `ResetStatistics()` method
- ? All statistics accessible via public getter methods

### 4. **MM1StatisticsDisplay.cs** (NEW)
Displays real-time M/M/1 statistics:
- ? (Arrival Rate)
- ? (Service Rate)  
- ? (Utilization) with unstable warning
- Current/Average/Max queue length
- Average wait time
- Total arrivals/serviced
- Simulation time
- **Theoretical M/M/1 values** (Lq and Wq)

### 5. **MM1ParameterController.cs** (NEW)
Complete UI controller for experiments:
- Arrival strategy dropdown
- Service strategy dropdown
- Arrival rate slider + input field
- Service rate slider + input field
- Reset button
- Start/Stop button
- Real-time parameter updates

### 6. **MM1DataLogger.cs** (NEW)
Exports simulation data to CSV:
- Logs every X seconds
- Creates CSV file with all M/M/1 metrics
- Auto-exports on application quit
- Data ready for Excel analysis and your .docx report

---

## How to Use in Unity

### Step 1: Setup Inspector Values

**GameObject with ArrivalProcess:**
- `Arrival Rate As Cars Per Hour`: **34.35** (or adjust to 30 for stability)
- `Arrival Interval Time Strategy`: **ExponentialIntervalTime**
- The `xs` and `ys` arrays are already set from your Excel data

**GameObject with ServiceProcess:**
- `Service Rate As Cars Per Hour`: **31.6** (or adjust to 35 for stability)
- `Service Interval Time Strategy`: **ExponentialIntervalTime**
- The `serviceTimesXs` and `serviceTimesYs` arrays are already set

### Step 2: Add UI Components (Your Canvas)

**Option A: Use MM1ParameterController** (Recommended)
1. Create empty GameObject, add `MM1ParameterController.cs`
2. Assign UI elements in Inspector:
   - Dropdowns for strategies
   - Sliders for rates (range 5-60)
   - Input fields for precise values
   - Reset and Start/Stop buttons

**Option B: Use Your Own UI**
- All parameters are `public` and serialized
- Call methods like:
  - `arrivalProcess.ChangeArrivalStrategy(...)`
  - `serviceProcess.ChangeServiceStrategy(...)`
  - `arrivalProcess.UpdateArrivalRate()`
  - `queueManager.ResetStatistics()`

### Step 3: Add Statistics Display

1. Create empty GameObject, add `MM1StatisticsDisplay.cs`
2. Create TextMeshProUGUI elements for each stat
3. Assign references in Inspector
4. Stats auto-update every 0.5 seconds

### Step 4: Add Data Logger (for .docx report)

1. Create empty GameObject, add `MM1DataLogger.cs`
2. Set `loggingInterval` (default 5 seconds)
3. Set `logFileName` 
4. Data auto-exports to: `Application.persistentDataPath`
   - Windows: `C:\Users\<username>\AppData\LocalLow\<company>\<project>\`
5. Import CSV into Excel for your report

---

## Experiment Scenarios for Your Report

Based on your requirement to test different parameters, try these:

| Scenario | ? (arr/hr) | ? (serv/hr) | ? | Expected Lq | Strategy |
|----------|-----------|-------------|---|-------------|----------|
| **Stable Light** | 20 | 35 | 0.57 | 0.76 | Exponential |
| **Stable Moderate** | 30 | 35 | 0.86 | 5.29 | Exponential |
| **Near Capacity** | 34 | 35 | 0.97 | 31.19 | Exponential |
| **Your Observed Data** | 34.35 | 31.6 | 1.09 | ? | Observed |

**For each scenario:**
1. Set parameters in Inspector
2. Run simulation for 5-10 minutes
3. Take screenshots
4. Export CSV data
5. Compare observed vs theoretical values

---

## M/M/1 Theoretical Formulas (Already Implemented)

```
? = ? / ?                    (Utilization)
Lq = ?² / (1 - ?)           (Avg queue length)
Ls = ? / (1 - ?)            (Avg in system)
Wq = ? / (?(1 - ?))         (Avg wait time in queue)
Ws = 1 / (? - ?)            (Avg time in system)
```

All calculated automatically in `MM1StatisticsDisplay.cs`!

---

## Files Modified/Created

**Modified:**
1. ? `Assets\Scripts\ArrivalProcess.cs` - M/M/1 parameters + Excel data
2. ? `Assets\Scripts\ServiceProcess.cs` - M/M/1 parameters + Excel data  
3. ? `Assets\Scripts\QueueManager.cs` - Statistics tracking

**New Files:**
4. ? `Assets\Scripts\MM1StatisticsDisplay.cs` - Real-time stats display
5. ? `Assets\Scripts\MM1ParameterController.cs` - UI parameter controls
6. ? `Assets\Scripts\MM1DataLogger.cs` - CSV data export

All variables remain `[SerializeField]` or `public` for your custom UI!

---

## Next Steps for Your Assignment

1. ? Code is updated with your Excel data
2. ?? Create UI in Unity (or use provided scripts)
3. ?? Run experiments with different ? and ? values
4. ?? Take screenshots for .docx report
5. ?? Export CSV data for analysis
6. ?? Build for Windows (Exe) or WebGL
7. ?? Zip project (exclude Library folder)
