window.sessionTimeoutManager = {
  dotNetRef: null,
  activityEvents: ["mousedown", "mousemove", "keydown", "scroll", "touchstart"],
  isTracking: false,
  activityHandler: null,

  initialize: function (dotNetReference) {
    this.dotNetRef = dotNetReference;

    // Create the activity handler once and store it
    this.activityHandler = () => {
      this.recordActivity();
    };

    this.startTracking();
    console.log("Session timeout tracking initialized");
  },

  startTracking: function () {
    if (this.isTracking || !this.activityHandler) return;

    this.activityEvents.forEach((event) => {
      document.addEventListener(event, this.activityHandler, { passive: true });
    });

    this.isTracking = true;
    console.log("Activity tracking started");
  },

  stopTracking: function () {
    if (!this.isTracking || !this.activityHandler) return;

    this.activityEvents.forEach((event) => {
      document.removeEventListener(event, this.activityHandler);
    });

    this.isTracking = false;
    console.log("Activity tracking stopped");
  },

  recordActivity: function () {
    if (this.dotNetRef) {
      try {
        this.dotNetRef.invokeMethodAsync("RecordActivity");
      } catch (error) {
        console.error("Error recording activity:", error);
      }
    }
  },

  dispose: function () {
    this.stopTracking();
    this.dotNetRef = null;
    this.activityHandler = null;
    console.log("Session timeout manager disposed");
  },
};
