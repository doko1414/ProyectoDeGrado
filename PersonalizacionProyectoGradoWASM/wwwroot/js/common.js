    function setActiveLink(clickedLink) {
        const links = document.querySelectorAll('.navbar .nav-link');
        links.forEach(link => {
        link.classList.remove('active-link');
    link.classList.remove('active');
    link.classList.add('inactive-link');
        });
    clickedLink.classList.remove('inactive-link');
    clickedLink.classList.add('active-link');
    }

window.ShowToastr = (type, message) => {
    if (type === "success") {
        toastr.success(message, "Operación Correcta", { timeOut: 10000 });
    }
    if (type === "error") {
        toastr.error(message, "Operación Fallida", { timeOut: 10000 });
    }
}

window.ShowSwal = (type, message) => {
    if (type === "success") {
        Swal.fire(
            'Success Notification',
            message,
            'success'
        );
    }
    if (type === "error") {
        Swal.fire(
            'Error Notification',
            message,
            'error'
        );
    }
}

function MostrarModalConfirmacionBorrado() {
    $('#modalConfirmacionBorrado').modal('show');
}

function OcultarModalConfirmacionBorrado() {
    $('#modalConfirmacionBorrado').modal('hide');
}

function crearGraficoPedidos(pendientes, enProceso, enviados, entregados, cancelados) {
    var ctx = document.getElementById('pedidosChart');
    if (ctx) {
        new Chart(ctx, {
            type: 'pie',
            data: {
                labels: ['Pendientes', 'En Proceso', 'Enviados', 'Entregados', 'Cancelados'],
                datasets: [{
                    data: [pendientes, enProceso, enviados, entregados, cancelados],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.8)',
                        'rgba(75, 192, 192, 0.8)',
                        'rgba(255, 206, 86, 0.8)',
                        'rgba(75, 192, 192, 0.8)',
                        'rgba(255, 99, 132, 0.8)'
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(255, 99, 132, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false, // Esto permite que controlemos el tamaño
                aspectRatio: 5, // Ajusta este valor para cambiar la proporción alto/ancho
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Distribución de Pedidos por Estado'
                    }
                }
            }
        });
    } else {
        console.error('No se encontró el elemento canvas para el gráfico de pedidos');
    }
}
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
function mostrarModelo3D(rutaModelo3D) {
    console.log("Ruta del modelo:", rutaModelo3D);
    $('#modelo3DModal').modal('show');
    $('#modelo3DModal').on('shown.bs.modal', function () {
        var canvas = document.getElementById("renderCanvas");
        var engine = new BABYLON.Engine(canvas, true);
        var scene;
        try {
            scene = createScene();
        } catch (error) {
            console.error("Error creating scene:", error);
            toastr.error("Error al crear la escena 3D");
            return;
        }

        function createScene() {
            var scene = new BABYLON.Scene(engine);
            var camera = new BABYLON.ArcRotateCamera("camera", -Math.PI / 2, Math.PI / 2.5, 3, BABYLON.Vector3.Zero(), scene);
            camera.attachControl(canvas, true);
            var light = new BABYLON.HemisphericLight("light", new BABYLON.Vector3(0, 1, 0), scene);

            BABYLON.SceneLoader.ImportMesh("", "", rutaModelo3D, scene,
                function (meshes) {
                    scene.createDefaultCameraOrLight(true, true, true);
                    scene.createDefaultEnvironment();
                    if (meshes.length > 0) {
                        scene.activeCamera.alpha = Math.PI / 2;
                        scene.activeCamera.beta = Math.PI / 2;
                    }
                },
                null,
                function (scene, message, exception) {
                    console.error("Error loading model:", message, exception);
                    toastr.error("Error al cargar el modelo 3D: " + message);
                }
            );

            return scene;
        }

        if (scene) {
            engine.runRenderLoop(function () {
                scene.render();
            });

            window.addEventListener("resize", function () {
                engine.resize();
            });
        }
    });

    $('#modelo3DModal').on('hidden.bs.modal', function () {
        if (typeof engine !== 'undefined' && engine) {
            engine.dispose();
        }
    });
}

console.log("Cargando funciones de modelo 3D...");
let scene, engine, canvas;
const accesoriosMesh = {};
let bicicletaMesh;

window.inicializarModeloBicicleta = function (rutaModeloBicicleta) {
    console.log("Inicializando modelo de bicicleta:", rutaModeloBicicleta);
    canvas = document.getElementById("renderCanvas");
    engine = new BABYLON.Engine(canvas, true);
    scene = createScene();

    BABYLON.SceneLoader.ImportMesh("", "", rutaModeloBicicleta, scene,
        function (meshes) {
            if (meshes.length > 0) {
                bicicletaMesh = meshes[0];
                bicicletaMesh.position = BABYLON.Vector3.Zero();
                bicicletaMesh.scaling = new BABYLON.Vector3(0.2, 0.2, 0.2);

                // Centramos la cámara en el modelo de la bicicleta
                var camera = new BABYLON.ArcRotateCamera("camera", -Math.PI / 2, Math.PI / 2.5, 10, bicicletaMesh.position, scene);
                camera.setTarget(bicicletaMesh.position);
                camera.attachControl(canvas, true);
                camera.lowerRadiusLimit = 5;
                camera.upperRadiusLimit = 20;

                var light = new BABYLON.HemisphericLight("light", new BABYLON.Vector3(0, 1, 0), scene);
                light.intensity = 0.7;

                var directionalLight = new BABYLON.DirectionalLight("directionalLight", new BABYLON.Vector3(-1, -2, -1), scene);
                directionalLight.intensity = 0.5;

                // Creamos un entorno simple sin necesidad de cargar archivos externos
                scene.createDefaultEnvironment({
                    createSkybox: false,
                    createGround: false,
                    cameraContrast: 2.5,
                    cameraExposure: 1
                });

                // Ajustamos la posición de la cámara después de cargar el modelo
                ajustarCamara();
                scene.debugLayer.show();
            }
            adjustLighting();
        },
        null,
        function (scene, message, exception) {
            console.error("Error al cargar el modelo de bicicleta:", message, exception);
        }
    );

    engine.runRenderLoop(function () {
        if (scene && scene.activeCamera) {
            scene.render();
        }
    });

    window.addEventListener("resize", function () {
        engine.resize();
    });
};
window.addEventListener("keydown", function (ev) {
    if (ev.keyCode === 73) { // 73 es el código de la tecla 'I'
        if (scene.debugLayer.isVisible()) {
            scene.debugLayer.hide();
        } else {
            scene.debugLayer.show();
        }
    }
});
function ajustarCamara() {
    if (bicicletaMesh) {
        // Calculamos el bounding box del modelo
        var boundingInfo = bicicletaMesh.getHierarchyBoundingVectors(true);
        var center = BABYLON.Vector3.Center(boundingInfo.min, boundingInfo.max);

        // Ajustamos la posición de la cámara
        if (scene.activeCamera) {
            scene.activeCamera.setTarget(center);

            // Ajustamos la distancia de la cámara basándonos en el tamaño del modelo
            var diagonal = BABYLON.Vector3.Distance(boundingInfo.min, boundingInfo.max);
            scene.activeCamera.radius = diagonal * 1.5; // Ajusta este multiplicador según sea necesario
        }
    }
}

window.actualizarModeloBicicleta = function (accesorios) {
    console.log("Actualizando modelo de bicicleta con accesorios:", accesorios);
    if (!scene) {
        console.error("La escena no está inicializada");
        return;
    }

    accesorios.forEach(function (accesorio) {
        if (accesorio.tipo !== "Bicicleta" && accesorio.rutaImagen) {
            if (accesoriosMesh[accesorio.tipo]) {
                accesoriosMesh[accesorio.tipo].dispose();
            }

            BABYLON.SceneLoader.ImportMesh("", "", accesorio.rutaImagen, scene,
                function (meshes) {
                    if (meshes.length > 0) {
                        const accesorioMesh = meshes[0];
                        accesoriosMesh[accesorio.tipo] = accesorioMesh;

                        accesorioMesh.scaling = new BABYLON.Vector3(1, 1, 1);

                        switch (accesorio.tipo) {
                            case "Canasta":
                                switch (accesorio.descripcion) {
                                    case "Chill'ka":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -33.5, 0);

                                        break;
                                    case "Mimbre Chino":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 0, 0);

                                        break;
                                    case "Mimbre Chileno":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -23, -1);

                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Montura":
                                switch (accesorio.descripcion) {
                                    case "Camel":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -20, 0);
                                        break;
                                    case "Camel con Blanco":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -13.45, 0);
                                        break;
                                    case "Marron con crema":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 0.9, 0);
                                        break;
                                    case "Negro":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 1, 0.1);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Bocina":
                                switch (accesorio.descripcion) {
                                    case "Plateado":
                                        accesorioMesh.position = new BABYLON.Vector3(-13.32, 0, 0);
                                        break;
                                    case "Dorado":
                                        accesorioMesh.position = new BABYLON.Vector3(-22.3, 0, 0);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Espejos":
                                switch (accesorio.descripcion) {
                                    case "Retrovisor Negro":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 0, 0);
                                        break;
                                    case "Blanco":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -6, 0);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Mangos":
                                switch (accesorio.descripcion) {
                                    case "Negro":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 0, 0);
                                        break;
                                    case "Caucho amarillo":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 1, 1.1);
                                        break;
                                    case "Caucho Naranja":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -15.8, 0);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Timbre":
                                switch (accesorio.descripcion) {
                                    case "Metal dorado":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -2.5, 0);
                                        break;
                                    case "Metal plateado":
                                        accesorioMesh.position = new BABYLON.Vector3(0, 0, 0);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                            case "Asiento para Niño ":
                                switch (accesorio.descripcion) {
                                    case "Azul":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -30, 0);
                                        break;
                                    case "Rojo":
                                        accesorioMesh.position = new BABYLON.Vector3(0, -11.0, 0);
                                        break;
                                }
                                accesorioMesh.rotation = new BABYLON.Vector3(0, 0, 0);
                                break;
                        }

                        accesorioMesh.parent = bicicletaMesh;
                    }

                    ajustarCamara();
                },
                null,
                function (scene, message, exception) {
                    console.error("Error al cargar el accesorio:", accesorio.tipo, message, exception);
                }
            );
        }
    });
};

window.cambiarColorBicicleta = function (color) {
    console.log("Cambiando color de la bicicleta a:", color);
    if (scene) {
        var frameMesh = scene.getMeshByName("OBJ.016_primitive2"); // Asegúrate de que este nombre coincida con el de tu modelo
        if (frameMesh) {
            console.log("Mesh encontrado:", frameMesh.name);
            if (!frameMesh.material) {
                frameMesh.material = new BABYLON.PBRMaterial("marcoMaterial", scene);
                console.log("Nuevo material PBR creado para el marco");
            }

            var colorBabylon = BABYLON.Color3.FromHexString(color);

            frameMesh.material.albedoColor = colorBabylon;
            frameMesh.material.metallic = 1; // Ajusta este valor entre 0 y 1
            frameMesh.material.roughness = 1; // Ajusta este valor entre 0 y 1
            frameMesh.material.emissiveColor = colorBabylon.scale(0.2); // Añade un poco de emisión para realzar el color

            console.log("Color del marco cambiado a", color);
        } else {
            console.log("Mesh del marco no encontrado, cambiando color de toda la bicicleta");
            if (bicicletaMesh) {
                var newMaterial = new BABYLON.PBRMaterial("bicicletaMaterial", scene);

                var colorBabylon = BABYLON.Color3.FromHexString(color);

                newMaterial.albedoColor = colorBabylon;
                newMaterial.metallic = 1; // Ajusta este valor entre 0 y 1
                newMaterial.roughness = 1; // Ajusta este valor entre 0 y 1
                newMaterial.emissiveColor = colorBabylon.scale(0.2);

                bicicletaMesh.material = newMaterial;

                if (bicicletaMesh.getChildMeshes) {
                    var childMeshes = bicicletaMesh.getChildMeshes();
                    for (var i = 0; i < childMeshes.length; i++) {
                        childMeshes[i].material = newMaterial;
                    }
                }
                console.log("Color de toda la bicicleta cambiado a", color);
            } else {
                console.error("No se pudo encontrar ni el marco ni la bicicleta completa");
            }
        }

        // Ajustar la iluminación de la escena
        adjustLighting();
    } else {
        console.error("La escena no está inicializada");
    }
};
function adjustLighting() {
    // Ajustar la luz existente
    var hemisphericLight = scene.getLightByName("light");
    if (hemisphericLight) {
        hemisphericLight.intensity = 0.7;
        hemisphericLight.diffuse = new BABYLON.Color3(1, 1, 1);
    }

    // Añadir una luz direccional si no existe
    var directionalLight = scene.getLightByName("directionalLight");
    if (!directionalLight) {
        directionalLight = new BABYLON.DirectionalLight("directionalLight", new BABYLON.Vector3(-1, -2, -1), scene);
    }
    directionalLight.intensity = 0.5;

    // Añadir una luz puntual para resaltar el modelo
    var pointLight = scene.getLightByName("pointLight");
    if (!pointLight) {
        pointLight = new BABYLON.PointLight("pointLight", new BABYLON.Vector3(0, 5, -10), scene);
    }
    pointLight.intensity = 0.3;
    // Añadir una luz ambiental si no existe
    if (!scene.getLightByName("ambientLight")) {
        var ambientLight = new BABYLON.HemisphericLight("ambientLight", new BABYLON.Vector3(0, 1, 0), scene);
        ambientLight.intensity = 0.2;
    }
}
function createScene() {
    const scene = new BABYLON.Scene(engine);
    scene.clearColor = BABYLON.Color3.FromHexString("#EDECE0");
    return scene;
}

console.log("Funciones de modelo 3D cargadas correctamente.");
