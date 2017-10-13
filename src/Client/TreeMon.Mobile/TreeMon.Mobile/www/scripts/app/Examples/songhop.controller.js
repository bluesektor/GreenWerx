
angular.module('songhop.controllers', ['ionic', 'songhop.services'])

/*
//Controller for the discover page
/ */
.controller('DiscoverCtrl', function($scope, $ionicLoading, $timeout, Recommendations, User) {

  // helper functions for loading
  var showLoading = function() {
    $ionicLoading.show({
      template: '<i class="ion-loading-c"></i>',
      noBackdrop: true
    });
  }

  var hideLoading = function() {
    $ionicLoading.hide();
  }


  // set loading to true first time while we retrieve songs from server.
  showLoading();

  // first we'll need to initialize the Rec service, get our first songs, etc
  Recommendations.init()
    .then(function(){

      $scope.currentSong = Recommendations.queue[0];

      return Recommendations.playCurrentSong();

    })
    .then(function(){
      // turn loading off
      hideLoading();
      $scope.currentSong.loaded = true;
    });


  // fired when we favorite / skip a song.
  $scope.sendFeedback = function (bool) {

    // first, add to favorites if they favorited
    if (bool) User.addSongToFavorites($scope.currentSong);

    // set variable for the correct animation sequence
    $scope.currentSong.rated = bool;
    $scope.currentSong.hide = true;

    // prepare the next song
    Recommendations.nextSong();

    // update current song in scope, timeout to allow animation to complete
    $timeout(function() {
      $scope.currentSong = Recommendations.queue[0];
      $scope.currentSong.loaded = false;

    }, 250);

    Recommendations.playCurrentSong().then(function() {
      $scope.currentSong.loaded = true;

    });

  }


  // used for retrieving the next album image.
  // if there isn't an album image available next, return empty string.
  $scope.nextAlbumImg = function() {
    if (Recommendations.queue.length > 1) {
      return Recommendations.queue[1].image_large;
    }

    return '';
  }

})


//Controller for the favorites page
//
.controller('FavoritesCtrl', function($scope, $window, User) {
  // get the list of our favorites from the user service
  $scope.favorites = User.favorites;
  $scope.username = User.username;

  $scope.removeSong = function(song, index) {
    User.removeSongFromFavorites(song, index);
  }

  $scope.openSong = function(song) {
    $window.open(song.open_url, "_system");
  }

})


/*
Controller for our tab bar
*/
.controller('TabsCtrl', function($scope, $window, User, Recommendations) {
  // expose the number of new favorites to the scope
  $scope.favCount = User.favoriteCount;

  // method to reset new favorites to 0 when we click the fav tab
  $scope.enteringFavorites = function() {
    User.newFavorites = 0;
    Recommendations.haltAudio();
  }

  $scope.leavingFavorites = function() {
    Recommendations.init();
  }

  $scope.logout = function() {
    User.DestroySession();

    // instead of using $state.go, we're going to redirect.
    // reason: we need to ensure views aren't cached.
    $window.location.href = '/';
  }

})


/*
Controller for the splash page
*/
.controller('SplashCtrl', function($scope, $state, User) {

  // get the list of our favorites from the user service
  $scope.submitForm = function(username, signingUp) {
    User.auth(username, signingUp).then(function(){
        // session is now set, so lets redirect to discover page
        console.log('************************************************** TODO remove the comments below.');
     // $state.go('tab.discover');

    }, function() {
      // error handling here
      alert('Hmm... try another username.');

    });
  }

});