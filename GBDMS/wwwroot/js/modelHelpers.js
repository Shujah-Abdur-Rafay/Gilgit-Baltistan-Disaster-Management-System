/**
 * Helper functions for the Model.razor component
 */

// Trigger a click on the file input element
function triggerFileInput(element) {
    if (element) {
        element.click();
    }
}

// Initialize drag and drop handlers
function initializeDragDrop() {
    // This is called by the component on first render
    console.log("Drag and drop initialized");
    
    // Note: Most of the drag and drop handling is done directly in the Blazor component,
    // this function is here for any additional JavaScript initialization that might be needed
}

// Check if a file is already in use
function isFileInUse(filePath) {
    try {
        // This is a dummy implementation since we can't directly check file locks from JavaScript
        // The actual file lock checking happens on the server side
        return false;
    } catch (error) {
        console.error("Error checking if file is in use:", error);
        return true;
    }
}

// Generate a sample visualization for testing
function generateSampleVisualization(type) {
    // This function could be used to generate sample visualizations 
    // when the actual Python script is not run
    return `/model_output/sample_${type}.png`;
} 