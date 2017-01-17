
/* Function to detect whether the current browser context supports opengl */ 
function webglAvailable() {
    try {
        var canvas = document.createElement("canvas");
        return !!
            window.WebGLRenderingContext &&
            (canvas.getContext("webgl") ||
                canvas.getContext("experimental-webgl"));
    } catch (e) {
        return false;
    }
}

var scene = new THREE.Scene();
objects = [];
var width = 400;
var height = 300;
camera = new THREE.PerspectiveCamera(60, window.innerWidth / window.innerHeight, 1, 10000);
camera.position.z = 3200;
renderer = new THREE.WebGLRenderer();

stats = new Stats();
if (webglAvailable()) {

    var material = new THREE.MeshNormalMaterial();
    var loader = new THREE.JSONLoader();
    loader.load('content/Suzanne.js', function (geometry) {

        geometry.computeVertexNormals();
        for (var i = 0; i < 2500; i++) {
            var mesh = new THREE.Mesh(geometry, material);
            mesh.position.x = Math.random() * 8000 - 4000;
            mesh.position.y = Math.random() * 8000 - 4000;
            mesh.position.z = Math.random() * 8000 - 4000;
            mesh.rotation.x = Math.random() * 2 * Math.PI;
            mesh.rotation.y = Math.random() * 2 * Math.PI;
            mesh.scale.x = mesh.scale.y = mesh.scale.z = Math.random() * 50 + 100;
            objects.push(mesh);
            scene.add(mesh);
        }

        renderer.setClearColor(0xffffff);
        renderer.setPixelRatio(window.devicePixelRatio);
        renderer.setSize(window.innerWidth, window.innerHeight);
        $("#webgl").append(renderer.domElement);

        $("#webgl").append(stats.dom);
    });

    
    animate();
} else {
    $("#webgl").append("WebGL is not supported by this browser. =(");
}

function animate() {
    requestAnimationFrame(animate);
    render();
    stats.update();
}

function render() {
    camera.lookAt(scene.position);
    for (var i = 0, il = objects.length; i < il; i++) {
        objects[i].rotation.x += 0.01;
        objects[i].rotation.y += 0.02;
    }
    renderer.render(scene, camera);
}


