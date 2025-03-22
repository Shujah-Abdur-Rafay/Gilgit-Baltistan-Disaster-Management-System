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
        let map = L.map(elementId).setView([35.3212, 74.3481], 6);
        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png").addTo(map);
    }
};
