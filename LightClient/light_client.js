let map = new ol.Map({
    target: 'map', // <-- This is the id of the div in which the map will be built.
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        })
    ],

    view: new ol.View({
        center: ol.proj.fromLonLat([7.0985774, 43.6365619]), // <-- Those are the GPS coordinates to center the map to.
        zoom: 10 // You can adjust the default zoom.
    })

});

let layers = [];

function printItinerary() {
    while(layers.length) {
        map.removeLayer(layers.pop())
    }
    if (this.status !== 200) {
        console.log("Contracts not retrieved. Check the error in the Network or Console tab.");
    } else {
        const responseObject = this.response;
        console.log(responseObject);

        if (responseObject[responseObject.length-1]['error'] === 'true') {
            document.getElementById("result").textContent = responseObject[responseObject.length-1]['textError'];
            return;
        }
        for (let i = 0; i < responseObject.length-1; i++) {
            let color = i % 2 === 0 ? '#002aff' : '#ff0000'
            let coords = responseObject[i]['features'][0]['geometry']['coordinates'];
            let lineString = new ol.geom.LineString(coords);

            lineString.transform('EPSG:4326', 'EPSG:3857');

            let feature = new ol.Feature({
                geometry: lineString,
                name: 'Line'
            });

            let lineStyle = new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: color,
                    width: 3
                })
            });

            let source = new ol.source.Vector({
                features: [feature]
            });

            let vector = new ol.layer.Vector({
                source: source,
                style: [lineStyle]
            });

            layers.push(vector);
            map.addLayer(vector);
        }
        showInstructions(responseObject);
    }
}

function showInstructions(responseObject) {
    let steps = "";
    for (let i = 0; i < responseObject.length-1; i++) {
        let instructions = responseObject[i]['features'][0]['properties']['segments'][0]['steps']
        for (let j = 0; j < instructions.length; j++) {
            steps += "- " + instructions[j]['instruction'] + "\n";
        }
    }
    let paragraph = document.getElementById("result");
    paragraph.textContent = steps;
    paragraph.setAttribute('style', 'white-space: pre-line;');

}

function findPathway() {
    document.getElementById("result").textContent = "";
    const targetUrl = "http://localhost:8733/Design_Time_Addresses/Router/RoutingService/rest/pathway";
    const requestType = "GET";
    if (document.getElementById('start').value === "" || document.getElementById('end').value === "") {
        setTimeout(
            () => document.getElementById("result").textContent = "Please fill both start and end destination fields",
            100);
        return;
    } else {
        setTimeout(() => {
            console.log('Retrieving pathway');
            document.getElementById("result").textContent = "Retrieving pathway...";
        }, 100);
    }
    let params = $("form").serialize();

    const onFinish = printItinerary;

    callAPI(targetUrl, requestType, params, onFinish);
}

function callAPI(url, requestType, params, finishHandler) {
    let fullUrl = url;

    if (params) {
        fullUrl += "?" + params;
    }

    console.log(fullUrl);
    // The js class used to call external servers is XMLHttpRequest.
    const caller = new XMLHttpRequest();
    caller.open(requestType, fullUrl, true);
    // The header set below limits the elements we are OK to retrieve from the server.
    caller.setRequestHeader ("Accept", "application/json");
    // onload shall contain the function that will be called when the call is finished.
    caller.onload=finishHandler;
    caller.responseType = 'json';
    caller.send();
}
