//window.turfInterop = {
//    calculateDistance: function (point1, point2) {
//        var from = turf.point(point1);
//        var to = turf.point(point2);
//        var options = { units: 'kilometers' };
//        return turf.distance(from, to, options);
//    }
//};



window.turfInterop = {
    calculateDistance: function (point1, point2) {
        return turf.distance(turf.point(point1), turf.point(point2), { units: "kilometers" });
    },

    bufferRegion: function (point, radius, units) {
        let buffered = turf.buffer(turf.point(point), radius, { units: units });
        return JSON.stringify(buffered);
    },

    clusterPoints: function (points, radius) {
        let fc = turf.featureCollection(points.map(p => turf.point(p)));
        let clustered = turf.clustersKmeans(fc, { numberOfClusters: Math.min(points.length, 5) });
        return clustered.features.length;
    },

    initializeMap: function (elementId) {
        // Create the base map with multiple layer options
        let map = L.map(elementId).setView([35.3212, 74.3481], 6);
        
        // Base layers
        let baseLayers = {
            "OpenStreetMap": L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png").addTo(map),
            "Satellite": L.tileLayer("https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}"),
            "Terrain": L.tileLayer("https://server.arcgisonline.com/ArcGIS/rest/services/World_Terrain_Base/MapServer/tile/{z}/{y}/{x}"),
            "Dark": L.tileLayer("https://tiles.stadiamaps.com/tiles/alidade_smooth_dark/{z}/{x}/{y}{r}.png")
        };
        
        // Layer control for base layers
        L.control.layers(baseLayers, {}).addTo(map);
        
        // Add scale control
        L.control.scale().addTo(map);
        
        // Check if Leaflet.draw is available
        if (typeof L.Control.Draw === 'function') {
            // Add drawing tools
            let drawnItems = new L.FeatureGroup();
            map.addLayer(drawnItems);
            
            let drawControl = new L.Control.Draw({
                edit: {
                    featureGroup: drawnItems
                },
                draw: {
                    polyline: true,
                    polygon: true,
                    rectangle: true,
                    circle: true,
                    marker: true
                }
            });
            map.addControl(drawControl);
            
            // Set up drawing event handlers
            map.on(L.Draw.Event.CREATED, function (e) {
                let layer = e.layer;
                drawnItems.addLayer(layer);
                
                // Convert the drawn item to GeoJSON
                let geojson = layer.toGeoJSON();
                
                // Trigger a custom event with the drawn item data
                const event = new CustomEvent('drawingCreated', { 
                    detail: { 
                        type: e.layerType,
                        geojson: JSON.stringify(geojson)
                    } 
                });
                document.dispatchEvent(event);
            });
            
            // Store drawing layer
            window.riskAtlasDrawnItems = drawnItems;
        } else {
            console.warn("Leaflet.Draw plugin not available. Drawing tools disabled.");
        }
        
        // Store map in global variable to access it later
        window.riskAtlasMap = map;
        
        return map;
    },
    
    // New advanced functions
    analyzeHazardRisk: function (points, hazardZones) {
        let pointsCollection = turf.featureCollection(points.map(p => turf.point([p.lng, p.lat], { name: p.name })));
        let hazardPolygons = turf.featureCollection(hazardZones.map(zone => 
            turf.polygon(zone.coordinates, { risk: zone.risk, type: zone.type })));
            
        let results = [];
        pointsCollection.features.forEach(point => {
            hazardPolygons.features.forEach(zone => {
                if (turf.booleanPointInPolygon(point, zone)) {
                    results.push({
                        pointName: point.properties.name,
                        hazardType: zone.properties.type,
                        riskLevel: zone.properties.risk
                    });
                }
            });
        });
        return results;
    },
    
    generateIsolines: function (point, breaks, options) {
        try {
            console.log("Generating isolines with:", { point, breaks, options });
            
            // Ensure breaks is an array
            if (!Array.isArray(breaks)) {
                console.error("Breaks is not an array:", breaks);
                breaks = [10, 20, 30, 40, 50]; // Default values
            }
            
            // Create a grid of points
            let bbox = turf.buffer(turf.point(point), options.maxDistance, { units: options.units });
            let grid = turf.pointGrid(turf.bbox(bbox), options.cellSize, { units: options.units });
            
            // Calculate distance from each grid point to the center
            grid.features.forEach(pt => {
                pt.properties.distance = turf.distance(pt, turf.point(point), { units: options.units });
            });
            
            // Generate isolines
            let isolines = [];
            
            breaks.forEach(brk => {
                // Ensure brk is a number
                brk = parseFloat(brk);
                
                // Create isolines one at a time with proper parameters
                try {
                    const isoOptions = {
                        zProperty: 'distance',
                        commonProperties: { distance: brk },
                        breaksValues: [brk]
                    };
                    
                    if (options.units) {
                        isoOptions.units = options.units;
                    }
                    
                    const isoline = turf.isolines(grid, isoOptions);
                    
                    if (isoline && isoline.features && isoline.features.length > 0) {
                        isolines.push(isoline.features[0]);
                    }
                } catch (e) {
                    console.error(`Error creating isoline for break ${brk}:`, e);
                }
            });
            
            if (isolines.length === 0) {
                // Return an empty feature collection if no isolines could be generated
                return JSON.stringify(turf.featureCollection([]));
            }
            
            return JSON.stringify(turf.featureCollection(isolines));
        } catch (e) {
            console.error("Error in generateIsolines:", e);
            return JSON.stringify(turf.featureCollection([]));
        }
    },
    
    calculateAreaAtRisk: function (hazardPolygons) {
        let totalArea = 0;
        hazardPolygons.forEach(polygon => {
            let area = turf.area(turf.polygon(polygon.coordinates));
            totalArea += area;
        });
        return totalArea / 1000000; // Convert to square kilometers
    },
    
    findSafeEvacuationRoutes: function (startPoint, endPoints, hazardZones) {
        let routes = [];
        
        endPoints.forEach(end => {
            // Create a line representing a direct route
            let line = turf.lineString([startPoint, end.coordinates]);
            
            // Check if route intersects with hazard zones
            let intersects = false;
            hazardZones.forEach(zone => {
                if (turf.booleanIntersects(line, turf.polygon(zone.coordinates))) {
                    intersects = true;
                }
            });
            
            routes.push({
                destination: end.name,
                coordinates: [startPoint, end.coordinates],
                safe: !intersects,
                distance: turf.length(line, { units: 'kilometers' })
            });
        });
        
        return routes;
    },
    
    renderMapWithLayers: function (elementId, mapData) {
        let map = window.riskAtlasMap || this.initializeMap(elementId);
        let layers = {};
        
        // Clear existing overlay layers but keep base layers
        map.eachLayer(function(layer) {
            if (layer instanceof L.TileLayer) {
                // This is a base layer, keep it
            } else if (!(layer instanceof L.FeatureGroup)) {
                // Not a base layer or the drawing feature group, remove it
                map.removeLayer(layer);
            }
        });
        
        // Add GeoJSON layers if specified
        if (mapData.geoJsonLayers) {
            mapData.geoJsonLayers.forEach(layer => {
                try {
                    let geoJson = typeof layer.data === 'string' ? JSON.parse(layer.data) : layer.data;
                    layers[layer.name] = L.geoJSON(geoJson, {
                        style: feature => ({
                            color: layer.color || '#3388ff',
                            weight: layer.weight || 3,
                            fillOpacity: layer.fillOpacity || 0.2
                        }),
                        onEachFeature: (feature, layer) => {
                            if (feature.properties) {
                                layer.bindPopup(Object.keys(feature.properties)
                                    .map(key => `<strong>${key}:</strong> ${feature.properties[key]}`)
                                    .join('<br>'));
                            }
                        }
                    }).addTo(map);
                } catch (e) {
                    console.error("Error adding GeoJSON layer:", e);
                }
            });
        }
        
        return true;
    },
    
    // NEW FEATURES IMPLEMENTATION
    
    // Geospatial Data Upload
    processGeospatialData: function(fileContent, fileType) {
        try {
            let geojson;
            
            if (fileType === "geojson") {
                geojson = JSON.parse(fileContent);
            } else if (fileType === "kml") {
                // Use toGeoJSON library to convert KML to GeoJSON
                const kmlDoc = new DOMParser().parseFromString(fileContent, 'text/xml');
                geojson = toGeoJSON.kml(kmlDoc);
            } else if (fileType === "gpx") {
                // Use toGeoJSON library to convert GPX to GeoJSON
                const gpxDoc = new DOMParser().parseFromString(fileContent, 'text/xml');
                geojson = toGeoJSON.gpx(gpxDoc);
            } else {
                throw new Error("Unsupported file type");
            }
            
            return {
                success: true,
                data: JSON.stringify(geojson)
            };
        } catch (e) {
            return {
                success: false,
                error: e.message
            };
        }
    },
    
    // Intersection & Overlay Analysis
    performIntersectionAnalysis: function(layer1GeoJson, layer2GeoJson) {
        try {
            // Parse GeoJSON if needed
            const geojson1 = typeof layer1GeoJson === 'string' ? JSON.parse(layer1GeoJson) : layer1GeoJson;
            const geojson2 = typeof layer2GeoJson === 'string' ? JSON.parse(layer2GeoJson) : layer2GeoJson;
            
            // Perform intersection analysis
            let intersections = [];
            let intersectionFeatures = [];
            
            // Loop through features in both layers
            geojson1.features.forEach(feature1 => {
                geojson2.features.forEach(feature2 => {
                    // Check for intersection
                    if (turf.booleanIntersects(feature1, feature2)) {
                        let intersection = turf.intersect(feature1, feature2);
                        
                        if (intersection) {
                            // Combine properties from both features
                            intersection.properties = {
                                ...feature1.properties,
                                ...feature2.properties,
                                layer1_id: feature1.id || "unknown",
                                layer2_id: feature2.id || "unknown",
                                intersection_area: turf.area(intersection) / 1000000 // sq km
                            };
                            
                            intersectionFeatures.push(intersection);
                            
                            // Add to results list
                            intersections.push({
                                layer1_properties: feature1.properties,
                                layer2_properties: feature2.properties,
                                intersection_area: intersection.properties.intersection_area
                            });
                        }
                    }
                });
            });
            
            // Create a feature collection of intersections
            const intersectionsGeoJson = turf.featureCollection(intersectionFeatures);
            
            return {
                success: true,
                intersections: intersections,
                geojson: JSON.stringify(intersectionsGeoJson)
            };
        } catch (e) {
            return {
                success: false,
                error: e.message
            };
        }
    },
    
    // Multi-Hazard Risk Zoning
    calculateMultiHazardRisk: function(hazardLayers) {
        try {
            // Create a grid across the total area
            let bounds = [70, 32, 80, 38]; // Approximate bounds for Gilgit-Baltistan region
            let grid = turf.pointGrid(bounds, 0.1); // 0.1 degree grid (approximately 10km)
            
            // Add risk scores for each point
            grid.features.forEach(point => {
                point.properties.risks = {};
                point.properties.totalRiskScore = 0;
                
                // Check each hazard layer
                hazardLayers.forEach(hazard => {
                    const hazardGeoJson = typeof hazard.data === 'string' ? JSON.parse(hazard.data) : hazard.data;
                    
                    // Check if point is in any of the features
                    let inHazardZone = false;
                    let highestRisk = 0;
                    
                    hazardGeoJson.features.forEach(feature => {
                        if (turf.booleanPointInPolygon(point, feature)) {
                            inHazardZone = true;
                            
                            // Get risk level from properties
                            let riskValue = 0;
                            if (feature.properties.risk) {
                                riskValue = feature.properties.risk === "High" ? 3 : 
                                            feature.properties.risk === "Medium" ? 2 : 
                                            feature.properties.risk === "Low" ? 1 : 0;
                            }
                            
                            highestRisk = Math.max(highestRisk, riskValue);
                        }
                    });
                    
                    // Set risk for this hazard type
                    point.properties.risks[hazard.name] = inHazardZone ? highestRisk : 0;
                    point.properties.totalRiskScore += point.properties.risks[hazard.name];
                });
                
                // Calculate overall risk category (1-10 scale)
                const maxPossibleScore = 3 * hazardLayers.length; // 3 (High) * number of hazard types
                point.properties.riskIndex = Math.round((point.properties.totalRiskScore / maxPossibleScore) * 10);
            });
            
            // Create risk classification
            let riskClasses = {
                "Very Low": [],
                "Low": [],
                "Moderate": [],
                "High": [],
                "Very High": []
            };
            
            grid.features.forEach(point => {
                const riskIndex = point.properties.riskIndex;
                
                if (riskIndex <= 2) {
                    riskClasses["Very Low"].push(point);
                } else if (riskIndex <= 4) {
                    riskClasses["Low"].push(point);
                } else if (riskIndex <= 6) {
                    riskClasses["Moderate"].push(point);
                } else if (riskIndex <= 8) {
                    riskClasses["High"].push(point);
                } else {
                    riskClasses["Very High"].push(point);
                }
            });
            
            // Create polygon for each risk class using concave hull
            let riskZones = [];
            
            for (const [riskLevel, points] of Object.entries(riskClasses)) {
                if (points.length > 3) { // Need at least 3 points for a polygon
                    try {
                        const pointCollection = turf.featureCollection(points);
                        const hull = turf.concave(pointCollection, { maxEdge: 2, units: 'kilometers' });
                        
                        if (hull) {
                            hull.properties = { riskLevel: riskLevel };
                            riskZones.push(hull);
                        }
                    } catch (e) {
                        console.warn(`Couldn't create hull for ${riskLevel}:`, e);
                    }
                }
            }
            
            // Create a feature collection of risk zones
            const riskZonesGeoJson = turf.featureCollection(riskZones);
            
            return {
                success: true,
                geojson: JSON.stringify(riskZonesGeoJson)
            };
        } catch (e) {
            return {
                success: false,
                error: e.message
            };
        }
    },
    
    // User Location Detection
    detectUserLocation: function() {
        return new Promise((resolve, reject) => {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    position => {
                        const userLocation = {
                            lng: position.coords.longitude,
                            lat: position.coords.latitude,
                            accuracy: position.coords.accuracy
                        };
                        
                        // If we have a map, show the user's location
                        if (window.riskAtlasMap) {
                            L.marker([userLocation.lat, userLocation.lng])
                                .addTo(window.riskAtlasMap)
                                .bindPopup('Your Location')
                                .openPopup();
                            
                            window.riskAtlasMap.setView([userLocation.lat, userLocation.lng], 12);
                        }
                        
                        resolve(userLocation);
                    },
                    error => {
                        reject({
                            success: false,
                            error: error.message
                        });
                    }
                );
            } else {
                reject({
                    success: false,
                    error: "Geolocation is not supported by this browser."
                });
            }
        });
    },
    
    // Search by Location or Coordinates
    searchByLocation: function(query) {
        return new Promise((resolve, reject) => {
            // Check if query is coordinates (e.g., "35.3212, 74.3481")
            const coordRegex = /^(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?)$/;
            const match = query.match(coordRegex);
            
            if (match) {
                // It's coordinates
                const lat = parseFloat(match[1]);
                const lng = parseFloat(match[3]);
                
                resolve({
                    success: true,
                    results: [{
                        name: `Coordinates (${lat}, ${lng})`,
                        location: { lat, lng }
                    }]
                });
            } else {
                // Use Nominatim for geocoding (OpenStreetMap)
                fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(query)}`)
                    .then(response => response.json())
                    .then(data => {
                        if (data && data.length > 0) {
                            const results = data.map(item => ({
                                name: item.display_name,
                                location: {
                                    lat: parseFloat(item.lat),
                                    lng: parseFloat(item.lon)
                                },
                                type: item.type,
                                importance: item.importance
                            }));
                            
                            resolve({
                                success: true,
                                results: results
                            });
                        } else {
                            resolve({
                                success: false,
                                error: "No results found for the search query."
                            });
                        }
                    })
                    .catch(error => {
                        reject({
                            success: false,
                            error: error.message
                        });
                    });
            }
        });
    },
    
    // Export Map and Data
    exportMapAsPDF: function() {
        return new Promise((resolve, reject) => {
            try {
                if (!window.riskAtlasMap) {
                    throw new Error("Map not initialized");
                }
                
                // Use html2canvas to capture the map
                html2canvas(document.getElementById('map'))
                    .then(canvas => {
                        // Create a new jsPDF instance
                        const pdf = new jspdf.jsPDF('landscape');
                        
                        // Add the map image to the PDF
                        const imgData = canvas.toDataURL('image/png');
                        pdf.addImage(imgData, 'PNG', 10, 10, 280, 150);
                        
                        // Add title and date
                        pdf.setFontSize(18);
                        pdf.text('Risk Atlas Map Export', 10, 180);
                        
                        pdf.setFontSize(12);
                        pdf.text(`Generated on ${new Date().toLocaleString()}`, 10, 190);
                        
                        // Save the PDF
                        pdf.save('risk-atlas-map.pdf');
                        
                        resolve({ success: true });
                    })
                    .catch(error => {
                        throw error;
                    });
            } catch (e) {
                reject({
                    success: false,
                    error: e.message
                });
            }
        });
    },
    
    exportAsGeoJSON: function() {
        try {
            if (!window.riskAtlasMap) {
                throw new Error("Map not initialized");
            }
            
            // Collect all GeoJSON layers
            let exportLayers = [];
            
            window.riskAtlasMap.eachLayer(layer => {
                if (layer instanceof L.GeoJSON) {
                    // This is a GeoJSON layer
                    const geojson = layer.toGeoJSON();
                    exportLayers.push(...geojson.features);
                }
            });
            
            // Add drawn items if any
            if (window.riskAtlasDrawnItems) {
                const drawnGeojson = window.riskAtlasDrawnItems.toGeoJSON();
                exportLayers.push(...drawnGeojson.features);
            }
            
            // Create a feature collection
            const exportData = turf.featureCollection(exportLayers);
            
            // Convert to string and trigger download
            const dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(exportData));
            const downloadAnchorNode = document.createElement('a');
            downloadAnchorNode.setAttribute("href", dataStr);
            downloadAnchorNode.setAttribute("download", "risk-atlas-export.geojson");
            document.body.appendChild(downloadAnchorNode);
            downloadAnchorNode.click();
            downloadAnchorNode.remove();
            
            return { success: true };
        } catch (e) {
            return {
                success: false,
                error: e.message
            };
        }
    },
    
    // Critical Infrastructure Analysis
    analyzeCriticalInfrastructure: function(infrastructureGeoJson, hazardGeoJson) {
        try {
            // Parse GeoJSON if needed
            const infrastructure = typeof infrastructureGeoJson === 'string' 
                ? JSON.parse(infrastructureGeoJson) : infrastructureGeoJson;
            const hazards = typeof hazardGeoJson === 'string' 
                ? JSON.parse(hazardGeoJson) : hazardGeoJson;
            
            // Results object
            let results = {
                totalInfrastructure: infrastructure.features.length,
                atRisk: 0,
                byType: {},
                byHazardType: {},
                detailedResults: []
            };
            
            // Analyze each infrastructure element
            infrastructure.features.forEach(infra => {
                const infraType = infra.properties.type || "Unknown";
                const infraName = infra.properties.name || "Unnamed";
                
                // Initialize counts for this type if not exists
                if (!results.byType[infraType]) {
                    results.byType[infraType] = {
                        total: 0,
                        atRisk: 0
                    };
                }
                results.byType[infraType].total++;
                
                // Check each hazard
                let atRisk = false;
                let hazardImpacts = [];
                
                hazards.features.forEach(hazard => {
                    const hazardType = hazard.properties.type || "Unknown";
                    const hazardRisk = hazard.properties.risk || "Unknown";
                    
                    // Initialize counts for this hazard type if not exists
                    if (!results.byHazardType[hazardType]) {
                        results.byHazardType[hazardType] = {
                            total: 0,
                            byRisk: {}
                        };
                    }
                    
                    // Check if infrastructure intersects with hazard
                    let intersects = false;
                    
                    if (infra.geometry.type === "Point") {
                        intersects = turf.booleanPointInPolygon(infra, hazard);
                    } else {
                        intersects = turf.booleanIntersects(infra, hazard);
                    }
                    
                    if (intersects) {
                        atRisk = true;
                        hazardImpacts.push({
                            hazardType: hazardType,
                            riskLevel: hazardRisk
                        });
                        
                        // Update counts
                        results.byHazardType[hazardType].total++;
                        
                        // Initialize risk counts
                        if (!results.byHazardType[hazardType].byRisk[hazardRisk]) {
                            results.byHazardType[hazardType].byRisk[hazardRisk] = 0;
                        }
                        results.byHazardType[hazardType].byRisk[hazardRisk]++;
                    }
                });
                
                // Add to detailed results if at risk
                if (atRisk) {
                    results.atRisk++;
                    results.byType[infraType].atRisk++;
                    
                    results.detailedResults.push({
                        name: infraName,
                        type: infraType,
                        hazards: hazardImpacts
                    });
                }
            });
            
            // Calculate percentages
            results.percentAtRisk = (results.atRisk / results.totalInfrastructure) * 100;
            
            Object.keys(results.byType).forEach(type => {
                results.byType[type].percentAtRisk = 
                    (results.byType[type].atRisk / results.byType[type].total) * 100;
            });
            
            return {
                success: true,
                results: results
            };
        } catch (e) {
            return {
                success: false,
                error: e.message
            };
        }
    },
    
    // Alert Notification System
    registerAlertAreas: function(areasGeoJson, thresholds) {
        try {
            // Store alert areas in global variable for simulating alerts
            window.alertAreas = {
                areas: typeof areasGeoJson === 'string' ? JSON.parse(areasGeoJson) : areasGeoJson,
                thresholds: thresholds || { rain: 100, earthquake: 5, flood: 3 }
            };
            
            return { success: true };
        } catch (e) {
            return {
                success: false,
                error: e.message
            };
        }
    },
    
    // Simulate alerts for testing
    simulateAlerts: function() {
        if (!window.alertAreas || !window.alertAreas.areas) {
            return { 
                success: false, 
                error: "No alert areas registered" 
            };
        }
        
        // Generate random alerts for testing
        const alertTypes = ["Earthquake", "Flood", "Landslide", "Avalanche"];
        const alerts = [];
        
        window.alertAreas.areas.features.forEach(area => {
            // Randomly decide if this area gets an alert (30% chance)
            if (Math.random() < 0.3) {
                const alertType = alertTypes[Math.floor(Math.random() * alertTypes.length)];
                const severity = Math.random() < 0.5 ? "Warning" : "Emergency";
                
                alerts.push({
                    type: alertType,
                    severity: severity,
                    area: area.properties.name || "Unnamed Area",
                    message: `${severity}: ${alertType} detected in ${area.properties.name || "an area"}`,
                    time: new Date().toISOString()
                });
            }
        });
        
        // Dispatch event with alerts
        const event = new CustomEvent('hazardAlerts', { 
            detail: { alerts: alerts } 
        });
        document.dispatchEvent(event);
        
        return { 
            success: true, 
            alerts: alerts 
        };
    },
    
    // Add administrative boundaries
    loadAdministrativeBoundaries: function() {
        // For Gilgit-Baltistan, Pakistan - simplified example
        const districts = [
            { name: "Gilgit", coordinates: [
                [74.0, 35.0], [74.5, 35.0], [74.5, 35.5], [74.0, 35.5], [74.0, 35.0]
            ]},
            { name: "Skardu", coordinates: [
                [75.0, 35.5], [76.0, 35.5], [76.0, 36.0], [75.0, 36.0], [75.0, 35.5]
            ]},
            { name: "Hunza", coordinates: [
                [72.5, 36.0], [73.5, 36.0], [73.5, 36.5], [72.5, 36.5], [72.5, 36.0]
            ]},
            { name: "Astore", coordinates: [
                [74.5, 35.0], [75.5, 35.0], [75.5, 35.5], [74.5, 35.5], [74.5, 35.0]
            ]}
        ];
        
        // Convert to GeoJSON
        const features = districts.map(district => ({
            type: "Feature",
            properties: { name: district.name, type: "District" },
            geometry: {
                type: "Polygon",
                coordinates: [district.coordinates]
            }
        }));
        
        const geoJson = {
            type: "FeatureCollection",
            features: features
        };
        
        return {
            success: true,
            geojson: JSON.stringify(geoJson)
        };
    },
    
    // Create legend for map
    addMapLegend: function(legendItems) {
        if (!window.riskAtlasMap) {
            return { success: false, error: "Map not initialized" };
        }
        
        // Check if legend control exists and remove it
        if (window.riskAtlasLegend) {
            window.riskAtlasLegend.remove();
        }
        
        // Create a legend control
        const legend = L.control({ position: 'bottomright' });
        
        legend.onAdd = function () {
            const div = L.DomUtil.create('div', 'info legend');
            div.style.backgroundColor = 'white';
            div.style.padding = '10px';
            div.style.borderRadius = '5px';
            div.style.boxShadow = '0 0 15px rgba(0,0,0,0.2)';
            
            div.innerHTML = '<h4 style="margin-top:0;">Legend</h4>';
            
            legendItems.forEach(item => {
                div.innerHTML += 
                    `<div style="display:flex;align-items:center;margin-bottom:5px;">
                        <span style="background:${item.color};width:20px;height:20px;display:inline-block;margin-right:5px;"></span>
                        <span>${item.label}</span>
                    </div>`;
            });
            
            return div;
        };
        
        legend.addTo(window.riskAtlasMap);
        window.riskAtlasLegend = legend;
        
        return { success: true };
    }
};
