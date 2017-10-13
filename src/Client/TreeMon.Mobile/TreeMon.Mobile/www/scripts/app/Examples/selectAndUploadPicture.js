.controller('CameraCtrl', function ($scope, $cordovaCamera, $ionicLoading, $localstorage) {
    $scope.data = { "ImageURI" :  "Select Image" };
    $scope.takePicture = function() {
        var options = {
            quality: 50,
            destinationType: Camera.DestinationType.FILE_URL,
            sourceType: Camera.PictureSourceType.CAMERA
        };
        $cordovaCamera.getPicture(options).then(
          function(imageData) {
              $scope.picData = imageData;
              $scope.ftLoad = true;
              $localstorage.set('fotoUp', imageData);
              $ionicLoading.show({template: 'Foto acquisita...', duration:500});
          },
          function(err){
              $ionicLoading.show({template: 'Errore di caricamento...', duration:500});
          })
    }

    $scope.selectPicture = function() { 
        var options = {
            quality: 50,
            destinationType: Camera.DestinationType.FILE_URI,
            sourceType: Camera.PictureSourceType.PHOTOLIBRARY
        };

        $cordovaCamera.getPicture(options).then(
          function(imageURI) {
              window.resolveLocalFileSystemURI(imageURI, function(fileEntry) {
                  $scope.picData = fileEntry.nativeURL;
                  $scope.ftLoad = true;
                  var image = document.getElementById('myImage');
                  image.src = fileEntry.nativeURL;
              });
              $ionicLoading.show({template: 'Foto acquisita...', duration:500});
          },
          function(err){
              $ionicLoading.show({template: 'Errore di caricamento...', duration:500});
          })
    };

    $scope.uploadPicture = function() {
        $ionicLoading.show({template: 'Sto inviando la foto...'});
        var fileURL = $scope.picData;
        var options = new FileUploadOptions();
        options.fileKey = "file";
        options.fileName = fileURL.substr(fileURL.lastIndexOf('/') + 1);
        options.mimeType = "image/jpeg";
        options.chunkedMode = true;

        var params = {};
        params.value1 = "someparams";
        params.value2 = "otherparams";

        options.params = params;

        var ft = new FileTransfer();
        ft.upload(fileURL, encodeURI("http://www.yourdomain.com/upload.php"), viewUploadedPictures, function(error) {$ionicLoading.show({template: 'Errore di connessione...'});
            $ionicLoading.hide();}, options);
    }

    var viewUploadedPictures = function() {
        $ionicLoading.show({template: 'Sto cercando le tue foto...'});
        server = "http://www.yourdomain.com/upload.php";
        if (server) {
            var xmlhttp = new XMLHttpRequest();
            xmlhttp.onreadystatechange=function(){
                if(xmlhttp.readyState === 4){
                    if (xmlhttp.status === 200) {					 
                        document.getElementById('server_images').innerHTML = xmlhttp.responseText;
                    }
                    else { $ionicLoading.show({template: 'Errore durante il caricamento...', duration: 1000});
                        return false;
                    }
                }
            };
            xmlhttp.open("GET", server , true);
            xmlhttp.send()}	;
        $ionicLoading.hide();
    }

    $scope.viewPictures = function() {
        $ionicLoading.show({template: 'Sto cercando le tue foto...'});
        server = "http://www.yourdomain.com/upload.php";
        if (server) {
            var xmlhttp = new XMLHttpRequest();
            xmlhttp.onreadystatechange=function(){
                if(xmlhttp.readyState === 4){
                    if (xmlhttp.status === 200) {					 
                        document.getElementById('server_images').innerHTML = xmlhttp.responseText;
                    }
                    else { $ionicLoading.show({template: 'Errore durante il caricamento...', duration: 1000});
                        return false;
                    }
                }
            };
            xmlhttp.open("GET", server , true);
            xmlhttp.send()}	;
        $ionicLoading.hide();
    }
})