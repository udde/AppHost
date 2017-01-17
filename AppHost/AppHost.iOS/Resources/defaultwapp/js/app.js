function DownloadImageHandler(data, success, path) {
    if (success) {
        console.log("DownloadImageHandler Succeded to Downlaod: " + data + " and saved it to: " + path);

        $('.upload-preview').css('background-image', 'url(' + path + ')', 'background-size', '100%', 'height', '500px');

    } else {
        console.log("DownloadImageHandler Failed to Downlaod: " + data);
    }
}

function NativeReady()
{
    //NativeFunc("Test0");
    //NativeFuncData("Test1", "data0");
    //NativeFuncDataCallback("Test2", "data1", "callback");
    NativeFuncDataCallback("getMetaData", "", OnMetadata);
    NativeFuncDataCallback("getGeoPositionStatus", "", OnGeoPositionStatus);
}

function OnMetadata(data)
{
    var list = $("#metadata");
    for (var key in data) {
        var item = "<li>" + key + ": " + data[key] + "</li>";
        list.append(item);
    }
}

function OnGeoPositionStatus(data)
{
    var list = $("#geoPosStatus");
    for (var key in data) {
        var item = "<li>" + key + ": " + data[key] + "</li>";
        list.append(item);
    }
}

function OnGeoPosCurrent(data)
{
    var container = $("#geoPosCurrent");

    var list = "<ul>";
    for (var key in data) {
        var item = "<li>" + key + ": " + data[key] + "</li>";
        list += item;
    }
    list += "</ul>";

    container.html(list);
}

function OnGeoPosListener(data)
{
    //$("#geoPosListenStatus").html(JSON.stringify(data));
}

function OnGeoPosStartListening(data)
{
    $("#geoPosListenStatus").html(JSON.stringify(data));
}

function OnGeoPosChanged(data)
{
    var container = $("#geoPos");

    var list = "<ul>";
    for (var key in data) {
        var item = "<li>" + key + ": " + data[key] + "</li>";
        list += item;
    }
    list += "</ul>";

    container.html(list);
}

$(document).ready(function () {

    $("#geoPosCurrentButton").on("click", function () {

        console.log("get-geo-position Loading location ...");

        $("#geoPosCurrent").html("Loading location ...");

        NativeFuncDataCallback("getGeoPos", "", OnGeoPosCurrent);
    });

    $("#geoPosListenerButton").on("click", function () {

        console.log("get-geo-position Loading location ...");

        $("#geoPosListenStatus").html("Setting up listener ...");

        NativeFuncDataCallback("geoPosStartListening", { minTime: "1", minDistance: "1", includeHeading: "true" }, OnGeoPosStartListening);

        NativeFuncListener("registerGeoPosListener", "", OnGeoPosListener, OnGeoPosChanged);
    });






    $("#image-button").on("click", function () {
        console.log("Image button click!");

        var url = $('#image-url-input').val();

        AppHost.DownloadImage("DownloadImageHandler", url);
    });

    $('#image-url-input').val("http://tux.valierya.com/~zapfyr/temp/249918.jpg");

    $("#click").on("click", function () {

        console.log("click");

        $.get("content/data.json", function (data) {
            $("#results").append(JSON.stringify(data));
        })

    });
	
	$("#click1").on("click", function () {
		console.log("click 1");

		$.get("http://tux.valierya.com/~zapfyr/demo/content/data1.json", function (data) {
			$("#results1").append(JSON.stringify(data));
		});
	});

    $("#get-geo-position").on("click", function () {

        console.log("get-geo-position Loading location ...");

        $("#geo-results").html("Loading location ...");

        if (navigator.geolocation) {

            console.log("get-geo-position Still loading location ...");

            $("#geo-results").html("Still loading location ...");

            navigator.geolocation.getCurrentPosition(function (position) {
                $("#geo-results").html("Latitude: " + position.coords.latitude + "<br>Longitude: " + position.coords.longitude);

                console.log("get-geo-position: Latitude: " + position.coords.latitude + " Longitude: " + position.coords.longitude);
            });
        } else {
            $("#geo-results").html("Geolocation is not supported by this browser.");
        }
    });

    $("#getplatform").on("click", function () {
        //Check if on browser
        if (typeof AppHost === "undefined") {
            console.log("On browser.");
        } else {
            console.log(AppHost.GetPlatform());
        }
        
    });

    $("#getasseturl").on("click", function () {

        console.log(AppHost.GetAssetURL());

    });
    $("#getgeoavailable").on("click", function () {

       console.log(AppHost.IsGeoAvailable());

    });

    $("#getgeolocation").on("click", function () {
        
        
        var geo = JSON.parse(AppHost.GetGeolocation());
        console.log("Lat: " + geo.latitude + ", Longitude: " + geo.longitude);

    });

    $("#getbattery").on("click", function () {

        var battery = AppHost.GetBatteryStatus();
        console.log("Battery percent is: " + battery);

    });

    $("#vibration").on("click", function () {

        AppHost.TriggerVibration()
        console.log("Vibration triggered.");

    });
    $("#takephoto").on("click", function () {

        AppHost.TakePhoto(function (photo) {
            //do something with photo.
        })

    });

    $("#getgeolocations").on("click", function () {

        AppHost.OnLocationUpdate(function (geopos) {
            var geo = JSON.parse(geopos);
            console.log("Lat: " + geo.latitude + ", Longitude: " + geo.longitude);
        });

    });
});