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

/**
 * Export functions for saving analysis results
 */

// Save data as a file and trigger browser download
function saveFile(data, fileName, mimeType) {
    const blob = new Blob([data], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.style.display = 'none';
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
    return true;
}

// Export data to CSV format
function exportToCsv(data, fileName) {
    try {
        return saveFile(data, fileName, 'text/csv;charset=utf-8;');
    } catch (error) {
        console.error('Error exporting to CSV:', error);
        return false;
    }
}

// Export data to JSON format
function exportToJson(data, fileName) {
    try {
        return saveFile(data, fileName, 'application/json');
    } catch (error) {
        console.error('Error exporting to JSON:', error);
        return false;
    }
}

// Export HTML content to PDF and download
function exportToPdf(title, htmlContent, fileName) {
    try {
        // Create a styled HTML document
        const styleSheet = `
            <style>
                body { font-family: Arial, sans-serif; margin: 20px; }
                h1 { color: #333366; margin-bottom: 20px; }
                h2 { color: #333366; margin-top: 30px; margin-bottom: 10px; }
                table { border-collapse: collapse; width: 100%; margin: 15px 0; }
                th, td { border: 1px solid #ddd; padding: 8px; }
                th { background-color: #f2f2f2; text-align: left; }
                .metric { font-size: 18px; font-weight: bold; margin: 10px 0; }
                .img-container { margin: 20px 0; max-width: 100%; }
                img { max-width: 100%; height: auto; }
            </style>
        `;
        
        const fullHtml = `
            <!DOCTYPE html>
            <html>
            <head>
                <title>${title}</title>
                ${styleSheet}
            </head>
            <body>
                ${htmlContent}
            </body>
            </html>
        `;
        
        // Use html2pdf.js library to convert HTML to PDF if available
        if (typeof html2pdf === 'undefined') {
            // Fallback to printing the HTML directly if html2pdf is not available
            const printWindow = window.open('', '_blank');
            printWindow.document.write(fullHtml);
            printWindow.document.close();
            printWindow.focus();
            setTimeout(() => {
                printWindow.print();
                printWindow.close();
            }, 250);
            return true;
        } else {
            // Use html2pdf if available
            const element = document.createElement('div');
            element.innerHTML = htmlContent;
            document.body.appendChild(element);
            
            const opt = {
                margin: 10,
                filename: fileName,
                image: { type: 'jpeg', quality: 0.98 },
                html2canvas: { scale: 2 },
                jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
            };
            
            html2pdf().set(opt).from(element).save().then(() => {
                document.body.removeChild(element);
            });
            return true;
        }
    } catch (error) {
        console.error('Error exporting to PDF:', error);
        return false;
    }
} 