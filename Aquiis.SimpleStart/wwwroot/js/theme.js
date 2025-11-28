window.themeManager = {
  setTheme: function (theme) {
    console.log("Setting theme to:", theme);
    document.documentElement.setAttribute("data-bs-theme", theme);
    localStorage.setItem("theme", theme);

    // Force browser to recalculate CSS custom properties
    // by triggering a reflow on the root element
    document.documentElement.style.display = "none";
    void document.documentElement.offsetHeight; // Trigger reflow
    document.documentElement.style.display = "";

    console.log(
      "Theme set. Current attribute:",
      document.documentElement.getAttribute("data-bs-theme")
    );
  },

  getTheme: function () {
    const theme = localStorage.getItem("theme") || "light";
    console.log("Getting theme from localStorage:", theme);
    return theme;
  },

  initTheme: function () {
    const savedTheme = this.getTheme();
    this.setTheme(savedTheme);
    return savedTheme;
  },
};

// Initialize theme IMMEDIATELY (before DOMContentLoaded) to prevent flash
if (typeof localStorage !== "undefined") {
  const savedTheme = localStorage.getItem("theme") || "light";
  console.log("Initial theme load:", savedTheme);
  document.documentElement.setAttribute("data-bs-theme", savedTheme);

  // Watch for Blazor navigation and re-apply theme
  // This handles Interactive Server mode where components persist
  const observer = new MutationObserver(function (mutations) {
    const currentTheme = document.documentElement.getAttribute("data-bs-theme");
    if (currentTheme) {
      // Re-trigger reflow to ensure CSS variables are applied
      document.documentElement.style.display = "none";
      void document.documentElement.offsetHeight;
      document.documentElement.style.display = "";
      console.log("Theme re-applied after DOM mutation:", currentTheme);
    }
  });

  // Start observing after a short delay to let Blazor initialize
  setTimeout(() => {
    observer.observe(document.body, {
      childList: true,
      subtree: true,
    });
  }, 1000);
}
