let map;
let markers = {};

let mapUser, directionsService, directionsRenderer;
let markersUser = {};

function initMap() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(position => {
            const userLat = position.coords.latitude;
            const userLng = position.coords.longitude;

            // Initialize the map centered on the user's current location
            map = new google.maps.Map(document.getElementById('map'), {
                center: { lat: userLat, lng: userLng },
                zoom: 11 // Adjust zoom level to your preference
            });

            // Optionally, add a custom marker for the user's initial location
            const userId = "55";
            markers[userId] = new google.maps.Marker({
                position: { lat: userLat, lng: userLng },
                map: map,
                title: "Your Location",
                icon: {
                    //url: 'https://maps.google.com/mapfiles/kml/shapes/target.png', // Use custom URL or point symbol
                    url: 'https://maps.google.com/mapfiles/kml/shapes/man.png', // Use custom URL or point symbol
                    scaledSize: new google.maps.Size(40, 40) // Adjust size to your preference
                }
            });
        }, () => {
            // If location access is denied or unavailable, fallback to default center
            map = new google.maps.Map(document.getElementById('map'), {
                center: { lat: 0, lng: 0 }, // Fallback center (you can change this)
                zoom: 2
            });
            alert("Unable to retrieve your location.");
        });
    } else {
        // If the browser doesn't support Geolocation, set a default center
        map = new google.maps.Map(document.getElementById('map'), {
            center: { lat: 0, lng: 0 }, // Fallback center (you can change this)
            zoom: 2
        });
        alert("Geolocation is not supported by this browser.");
    }
}

// Function to update the map with new user location
function updateMap(userId, vehicalNumber, latitude, longitude) {
    if (markers[userId]) {
        // Update marker position
        markers[userId].setPosition({ lat: latitude, lng: longitude });
    } else {
        // Create a new marker for other users
        markers[userId] = new google.maps.Marker({
            position: { lat: latitude, lng: longitude },
            map: map,
            title: vehicalNumber.toString(),
            icon: {
                url: 'https://img.icons8.com/ios-filled/50/000000/ambulance.png', // Different icon for other users (optional)
                scaledSize: new google.maps.Size(30, 30) // Adjust size for other users
            }
        });
    }
}

function initUserMap() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(position => {
            const userLat = position.coords.latitude;
            const userLng = position.coords.longitude;

            // Initialize the map centered on the user's current location
            mapUser = new google.maps.Map(document.getElementById('mapUser'), {
                center: { lat: userLat, lng: userLng },
                zoom: 11 // Adjust zoom level to your preference
            });

            directionsService = new google.maps.DirectionsService();
            directionsRenderer = new google.maps.DirectionsRenderer();

            directionsRenderer.setMap(mapUser);

            // Optionally, add a custom marker for the user's initial location
            const userId = "55";
            markersUser[userId] = new google.maps.Marker({
                position: { lat: userLat, lng: userLng },
                map: mapUser,
                title: "Your Location",
                icon: {
                    url: 'https://maps.google.com/mapfiles/kml/shapes/man.png', // Use custom URL or point symbol
                    scaledSize: new google.maps.Size(40, 40) // Adjust size to your preference
                }
            });
        }, () => {
            // If location access is denied or unavailable, fallback to default center
            mapUser = new google.maps.Map(document.getElementById('mapUser'), {
                center: { lat: 0, lng: 0 }, // Fallback center (you can change this)
                zoom: 2
            });
            alert("Unable to retrieve your location.");
        });
    } else {
        // If the browser doesn't support Geolocation, set a default center
        map = new google.maps.Map(document.getElementById('map'), {
            center: { lat: 0, lng: 0 }, // Fallback center (you can change this)
            zoom: 2
        });
        alert("Geolocation is not supported by this browser.");
    }
}

// Function to update the map with new user location
function updateMapUser(userId, vehicalNumber, latitude, longitude) {
    if (markersUser[userId]) {
        // Update marker position
        markersUser[userId].setPosition({ lat: latitude, lng: longitude });
    } else {
        // Create a new marker for other users
        markersUser[userId] = new google.maps.Marker({
            position: { lat: latitude, lng: longitude },
            map: mapUser,
            title: vehicalNumber.toString(),
            icon: {
                url: 'https://img.icons8.com/ios-filled/50/000000/ambulance.png', // Different icon for other users (optional)
                scaledSize: new google.maps.Size(30, 30) // Adjust size for other users
            }
        });
    }


    //24.884194, 67.182833
    const destination = { lat: latitude, lng: longitude };

    // Check if the browser supports the Geolocation API
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            // Use the current position as the origin
            const origin = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            //const destination = { lat: latitude, lng: longitude }; // Your destination coordinates

            const service = new google.maps.DistanceMatrixService();
            service.getDistanceMatrix(
                {
                    origins: [origin],
                    destinations: [destination],
                    travelMode: 'DRIVING', // Mode of travel, can be changed if needed
                },
                function (response, status) {
                    if (status === 'OK') {
                        const result = response.rows[0].elements[0];
                        const distance = result.distance.text; // Distance between points
                        const duration = result.duration.text; // Estimated time of arrival (ETA)

                        $('#estDis').text(distance);
                        $('#estTime').text(duration);

                        // Display route on the map
                        displayRoute(origin, destination);
                    } else {
                        console.error('Error calculating distance:', status);
                    }
                }
            );
        }, function (error) {
            console.error('Error getting location:', error);
        });
    } else {
        console.error('Geolocation is not supported by this browser.');
    }

}

// Function to display the route on the map
function displayRoute(origin, destination) {
    directionsService.route(
        {
            origin: origin,
            destination: destination,
            travelMode: google.maps.TravelMode.DRIVING, // Specify travel mode
        },
        function (response, status) {
            console.log(response)
            if (status === 'OK') {
                directionsRenderer.setDirections(response); // Render the route on the map
            } else {
                console.error('Directions request failed due to ' + status);
            }
        }
    );
}

// SignalR connection
const connection = new signalR.HubConnectionBuilder().withUrl("/locationHub").build();

// Start the SignalR connection
connection.start().then(() => {
    console.log('Google Map Connection started');
}).catch(err => console.error(err));

// Function to get the location of the user and send it with the unique ID
async function sendLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(position => {
            const latitude = position.coords.latitude;
            const longitude = position.coords.longitude;

            connection.invoke("SendLocation", latitude, longitude)
                .catch(err => console.error(err));
        });
    } else {
        alert("Geolocation is not supported by this browser.");
    }
}

// Example: Call sendLocation every 5 seconds
setInterval(sendLocation, 4000);

// Receive location updates from other users
connection.on("ReceiveLocation", (userId, vehicalNumber, latitude, longitude) => {
    if (window.location.href.includes('RequestTracking')) {
        if ($('#driverIdForTracking').length > 0) {
            var driver = parseInt($('#driverIdForTracking').text());
            console.log(driver)
            if (driver == userId) {
                console.log(`Received location from ${userId}: (${latitude}, ${longitude})`);
                updateMapUser(userId, vehicalNumber, latitude, longitude);
            }
        }
    } else {
        console.log(`Received location from ${userId}: (${latitude}, ${longitude})`);
        updateMap(userId, vehicalNumber, latitude, longitude);
    }
});