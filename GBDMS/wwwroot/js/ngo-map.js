// NGO Map Implementation
let ngoMap;
let ngoMarkers = {};
let ngoLayers = {
    International: L.layerGroup(),
    National: L.layerGroup(),
    Local: L.layerGroup()
};

window.initNgoMap = function(lat, lng) {
    if (typeof L !== "undefined") {
        // Initialize the map
        ngoMap = L.map("ngoMap").setView([lat, lng], 8);
        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png").addTo(ngoMap);

        // Add layer groups to map
        ngoLayers.International.addTo(ngoMap);
        ngoLayers.National.addTo(ngoMap);
        ngoLayers.Local.addTo(ngoMap);

        // Add a legend
        let legend = L.control({position: 'bottomright'});
        legend.onAdd = function() {
            let div = L.DomUtil.create('div', 'info legend');
            div.innerHTML = `
                <div style="background-color: white; padding: 10px; border-radius: 5px; box-shadow: 0 1px 5px rgba(0,0,0,0.4)">
                    <h6 style="margin: 0 0 5px 0;">NGO Types</h6>
                    <div style="display: flex; align-items: center; margin: 3px 0">
                        <span style="background-color: rgba(25, 135, 84, 0.3); color: #155724; padding: 2px 5px; border-radius: 3px; margin-right: 5px;">International</span>
                    </div>
                    <div style="display: flex; align-items: center; margin: 3px 0">
                        <span style="background-color: #198754; color: white; padding: 2px 5px; border-radius: 3px; margin-right: 5px;">National</span>
                    </div>
                    <div style="display: flex; align-items: center; margin: 3px 0">
                        <span style="background-color: rgba(25, 135, 84, 0.15); color: #155724; padding: 2px 5px; border-radius: 3px; margin-right: 5px;">Local</span>
                    </div>
                </div>
            `;
            return div;
        };
        legend.addTo(ngoMap);
    }
};

window.addNgoMarker = function(id, name, type, district, focusArea, lat, lng) {
    if (ngoMap) {
        // Determine marker styling
        let markerColor;
        let layer;
        
        switch(type) {
            case "International":
                markerColor = "#198754"; // Green
                layer = ngoLayers.International;
                break;
            case "National":
                markerColor = "#198754"; // Green
                layer = ngoLayers.National;
                break;
            case "Local":
                markerColor = "#198754"; // Green
                layer = ngoLayers.Local;
                break;
            default:
                markerColor = "#6c757d"; // Gray
                layer = ngoLayers.Local;
        }

        // Create marker with custom icon
        let icon = L.divIcon({
            className: 'ngo-marker-icon',
            html: `<div style="background-color: ${markerColor}; width: 12px; height: 12px; border-radius: 50%; border: 2px solid white;"></div>`,
            iconSize: [16, 16],
            iconAnchor: [8, 8]
        });

        // Create and add marker
        let marker = L.marker([lat, lng], { icon: icon })
            .bindPopup(`
                <div class="ngo-popup">
                    <h6>${name}</h6>
                    <div class="ngo-popup-type">Type: ${type}</div>
                    <div>District: ${district}</div>
                    <div>Focus: ${focusArea}</div>
                </div>
            `);
        
        marker.addTo(layer);
        
        // Store the marker reference with its ID for future operations
        ngoMarkers[id] = { marker, layer };
    }
};

window.removeNgoMarker = function(id) {
    if (ngoMarkers[id]) {
        ngoMarkers[id].layer.removeLayer(ngoMarkers[id].marker);
        delete ngoMarkers[id];
    }
};

window.toggleNgoLayerVisibility = function(layerType, isVisible) {
    if (ngoMap) {
        if (isVisible) {
            ngoLayers[layerType].addTo(ngoMap);
        } else {
            ngoMap.removeLayer(ngoLayers[layerType]);
        }
    }
};

// Escape key handler for modal
let ngoComponentRef = null;

window.addEscapeKeyListener = function(componentRef) {
    ngoComponentRef = componentRef;

    // Remove existing listener if any
    document.removeEventListener('keydown', handleEscapeKey);

    // Add new listener
    document.addEventListener('keydown', handleEscapeKey);
};

function handleEscapeKey(event) {
    if (event.key === 'Escape' && ngoComponentRef) {
        ngoComponentRef.invokeMethodAsync('HandleEscapeKey');
    }
}