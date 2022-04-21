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

function feedContractList() {
    if (this.status !== 200) {
        console.log("Contracts not retrieved. Check the error in the Network or Console tab.");
    } else {
        // The result is contained in "this.responseText". First step: transform it into a js object.
        const responseObject = JSON.parse(this.responseText);
        // Second step: extract the contract names from the list.
        let contracts = responseObject.map(function(contract) {
            return contract.name;
        });
        // Third step: fill the datalist in the html.
        let listContainer = document.getElementById("contractsList");
        // Empty the list in case it had already been filled.
        listContainer.innerHTML = "";
        for (let i=0; i<contracts.length; i++) {
            let currentContract = contracts[i];
            // Create a <option> tag that will contain the contract value.
            let option = document.createElement("option");
            // <option>'s value needs to be in a "value" attribute.
            option.setAttribute("value", currentContract);
            // When the <option> is complete, add it to the list.
            listContainer.appendChild(option);
        }
    }
}

function findPathway() {
    console.log('Retrieving pathway');
    const targetUrl = "http://localhost:8733/Design_Time_Addresses/Router/Service1/api/pathway";
    const requestType = "GET";
    let params = $("form").serialize();
    
    const onFinish = feedContractList;

    callAPI(targetUrl, requestType, params, onFinish);
    document.getElementById("result").textContent = "bonjour";
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

    caller.send();
}
