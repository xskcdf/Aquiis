// File download and view functionality for Blazor applications

/**
 * Downloads a file from base64 data
 * @param {string} filename - The name of the file to download
 * @param {string} base64Data - Base64 encoded file data
 * @param {string} mimeType - MIME type of the file (e.g., 'application/pdf')
 */
window.downloadFile = function (filename, base64Data, mimeType) {
  try {
    // Convert base64 to binary
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);

    // Create blob and download
    const blob = new Blob([byteArray], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    // Clean up the URL object
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error("Error downloading file:", error);
    alert("Error downloading file. Please try again.");
  }
};

/**
 * Opens a file in a new browser tab for viewing
 * @param {string} base64Data - Base64 encoded file data
 * @param {string} mimeType - MIME type of the file (e.g., 'application/pdf')
 */
window.viewFile = function (base64Data, mimeType) {
  try {
    // Convert base64 to binary
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);

    // Create blob and open in new tab
    const blob = new Blob([byteArray], { type: mimeType });
    const url = window.URL.createObjectURL(blob);

    // Open in new window/tab
    const newWindow = window.open(url, "_blank");

    // If popup was blocked, provide alternative
    if (
      !newWindow ||
      newWindow.closed ||
      typeof newWindow.closed === "undefined"
    ) {
      alert("Please allow popups for this site to view files.");
    }

    // Note: We don't revoke the URL immediately as the new window needs it
    // The browser will clean it up when the window is closed
  } catch (error) {
    console.error("Error viewing file:", error);
    alert("Error viewing file. Please try again.");
  }
};

/**
 * Clicks an element by its ID (useful for triggering hidden file inputs)
 * @param {string} elementId - The ID of the element to click
 */
window.clickElementById = function (elementId) {
  try {
    const element = document.getElementById(elementId);
    if (element) {
      element.click();
    } else {
      console.error(`Element with ID '${elementId}' not found`);
    }
  } catch (error) {
    console.error("Error clicking element:", error);
  }
};
