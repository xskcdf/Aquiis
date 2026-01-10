namespace Aquiis.SimpleStart.Shared.Services;

public class SessionTimeoutService
{
    private Timer? _warningTimer;
    private Timer? _logoutTimer;
    private DateTime _lastActivity;
    private readonly object _lock = new();

    public event Action? OnWarningTriggered;
    public event Action<int>? OnWarningCountdown; // Remaining seconds
    public event Action? OnTimeout;

    public TimeSpan InactivityTimeout { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan WarningDuration { get; set; } = TimeSpan.FromMinutes(2);
    public bool IsEnabled { get; set; } = true;
    public bool IsWarningActive { get; private set; }
    public int WarningSecondsRemaining { get; private set; }

    public SessionTimeoutService()
    {
        _lastActivity = DateTime.UtcNow;
    }

    public void Start()
    {
        if (!IsEnabled) return;

        lock (_lock)
        {
            ResetActivity();
            StartMonitoring();
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            _warningTimer?.Dispose();
            _logoutTimer?.Dispose();
            _warningTimer = null;
            _logoutTimer = null;
            IsWarningActive = false;
        }
    }

    public void RecordActivity()
    {
        if (!IsEnabled) return;

        lock (_lock)
        {
            _lastActivity = DateTime.UtcNow;

            // If warning is active, cancel it and restart monitoring
            if (IsWarningActive)
            {
                CancelWarning();
                StartMonitoring();
            }
        }
    }

    public void ExtendSession()
    {
        RecordActivity();
    }

    private void StartMonitoring()
    {
        _warningTimer?.Dispose();
        _logoutTimer?.Dispose();

        var warningTime = InactivityTimeout - WarningDuration;
        
        _warningTimer = new Timer(
            _ => TriggerWarning(),
            null,
            warningTime,
            Timeout.InfiniteTimeSpan
        );
    }

    private void TriggerWarning()
    {
        lock (_lock)
        {
            if (!IsEnabled) return;

            IsWarningActive = true;
            WarningSecondsRemaining = (int)WarningDuration.TotalSeconds;

            OnWarningTriggered?.Invoke();

            // Start countdown timer
            _logoutTimer = new Timer(
                _ => CountdownTick(),
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1)
            );
        }
    }

    private void CountdownTick()
    {
        lock (_lock)
        {
            WarningSecondsRemaining--;
            OnWarningCountdown?.Invoke(WarningSecondsRemaining);

            if (WarningSecondsRemaining <= 0)
            {
                TriggerTimeout();
            }
        }
    }

    private void TriggerTimeout()
    {
        lock (_lock)
        {
            IsWarningActive = false;
            Stop();
            OnTimeout?.Invoke();
        }
    }

    private void CancelWarning()
    {
        IsWarningActive = false;
        _warningTimer?.Dispose();
        _logoutTimer?.Dispose();
        _warningTimer = null;
        _logoutTimer = null;
    }

    private void ResetActivity()
    {
        _lastActivity = DateTime.UtcNow;
        IsWarningActive = false;
    }

    public void Dispose()
    {
        Stop();
    }
}
