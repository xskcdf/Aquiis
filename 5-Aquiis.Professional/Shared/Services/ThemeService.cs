namespace Aquiis.Professional.Shared.Services;

public class ThemeService
{
    private string _currentTheme = "light";
    
    public event Action? OnThemeChanged;
    
    public string CurrentTheme => _currentTheme;
    
    public void SetTheme(string theme)
    {
        if (theme != "light" && theme != "dark")
        {
            throw new ArgumentException("Theme must be 'light' or 'dark'", nameof(theme));
        }
        
        _currentTheme = theme;
        OnThemeChanged?.Invoke();
    }
    
    public void ToggleTheme()
    {
        SetTheme(_currentTheme == "light" ? "dark" : "light");
    }
    
    public string GetNextTheme()
    {
        return _currentTheme == "light" ? "dark" : "light";
    }
}
