window.initResponseMap = function(lat, lng) {
    if (typeof L !== "undefined") {
        // Initialize the map
        let responseMap = L.map("responseMap").setView([lat, lng], 8);
        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png").addTo(responseMap);

        // Create layer groups
        window.mapLayers = {
            resources: L.layerGroup().addTo(responseMap),
            evacuations: L.layerGroup().addTo(responseMap),
            roadClosures: L.layerGroup().addTo(responseMap)
        };

        // Add resource markers
        const resourceMarkers = [
            { lat: 35.9212, lng: 74.3481, name: "Search & Rescue Team Alpha", type: "SAR" },
            { lat: 35.8950, lng: 74.3571, name: "Medical Team Bravo", type: "Medical" },
            { lat: 35.9150, lng: 74.3651, name: "Evacuation Center 1", type: "Shelter" },
            { lat: 35.9015, lng: 74.3701, name: "Field Hospital", type: "Medical" },
            { lat: 35.8814, lng: 74.3751, name: "Supply Distribution Point", type: "Logistics" }
        ];

        resourceMarkers.forEach(point => {
            let icon = L.divIcon({
                className: "map-icon " + point.type.toLowerCase(),
                html: "<i class=\"bi " + getIconClass(point.type) + "\"></i>",
                iconSize: [24, 24]
            });

            L.marker([point.lat, point.lng], { icon: icon })
                .bindPopup("<b>" + point.name + "</b><br>Type: " + point.type)
                .addTo(window.mapLayers.resources);
        });

        // Add evacuation zones
        const evacuationZones = [
            [
                [35.9312, 74.3581],
                [35.9312, 74.3781],
                [35.9112, 74.3781],
                [35.9112, 74.3581]
            ]
        ];

        evacuationZones.forEach(zone => {
            L.polygon(zone, {
                color: "#ff9800",
                fillColor: "#ff9800",
                fillOpacity: 0.3
            }).bindPopup("Evacuation Zone A - Priority 1")
            .addTo(window.mapLayers.evacuations);
        });

        // Add road closures
        const roadClosures = [
            { 
                points: [[35.9050, 74.3381], [35.9150, 74.3481]],
                name: "Main Street Closure"
            },
            { 
                points: [[35.8950, 74.3681], [35.8850, 74.3581]],
                name: "Highway Junction Blockage"
            }
        ];

        roadClosures.forEach(closure => {
            L.polyline(closure.points, {
                color: "#f44336",
                weight: 5,
                dashArray: "10, 10"
            }).bindPopup(closure.name)
            .addTo(window.mapLayers.roadClosures);
        });
        
        function getIconClass(type) {
            switch(type) {
                case "SAR": return "bi-search";
                case "Medical": return "bi-hospital";
                case "Shelter": return "bi-house";
                case "Logistics": return "bi-truck";
                default: return "bi-geo-alt";
            }
        }
        
        window.responseMap = responseMap;
    }
};

window.updateResponseMapLayers = function(showResources, showEvacuations, showRoadClosures) {
    if (window.mapLayers) {
        if (showResources) {
            window.responseMap.addLayer(window.mapLayers.resources);
        } else {
            window.responseMap.removeLayer(window.mapLayers.resources);
        }
        
        if (showEvacuations) {
            window.responseMap.addLayer(window.mapLayers.evacuations);
        } else {
            window.responseMap.removeLayer(window.mapLayers.evacuations);
        }
        
        if (showRoadClosures) {
            window.responseMap.addLayer(window.mapLayers.roadClosures);
        } else {
            window.responseMap.removeLayer(window.mapLayers.roadClosures);
        }
    }
}; 