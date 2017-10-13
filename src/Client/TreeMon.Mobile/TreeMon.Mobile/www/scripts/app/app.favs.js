
// Ionic Starter App

// angular.module is a global place for creating, registering and retrieving Angular modules
// 'starter' is the name of this angular module example (also set in a <body> attribute in index.html)
// the 2nd parameter is an array of 'requires'
angular.module('songhop', ['ionic', 'songhop.controllers'])

.run(function ($ionicPlatform, $rootScope, $state, User) {
    $ionicPlatform.ready(function () {
        //// Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
        //// for form inputs)
        //if (window.cordova && window.cordova.plugins.Keyboard) {
        //    cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
        //}
        if (window.StatusBar) {
            StatusBar.styleDefault();
        }
    });
})


.config(function ($stateProvider, $urlRouterProvider) {

    // Ionic uses AngularUI Router which uses the concept of states
    // Learn more here: https://github.com/angular-ui/ui-router
    // Set up the various states which the app can be in.
    // Each state's controller can be found in controllers.js
    $stateProvider

    // splash page
    .state('splash', {
        url: '/',
        templateUrl: '/content/appfavs/templates/splash.html',
        controller: 'SplashCtrl',
        onEnter: function ($state, User) {
            console.log('************************************************** TODO remove the comments below.');
          //  User.CheckSession().then(function (hasSession) {
          //      if (hasSession) $state.go('tab.discover');
          //  });
        }
    })

    .state('register',{
        url: '/register',
        templateUrl: '/content/appfavs/templates/register.html',
        controller: 'RegisterCtrl',
        onEnter: function ($state, User) {
            console.log('************************************************** TODO remove the comments below.');
           // User.CheckSession().then(function (hasSession) {
            //    if (hasSession) $state.go('tab.discover');
           // });
        }
    })

    // setup an abstract state for the tabs directive
    .state('tab', {
        url: '/tab',
        abstract: true,
        templateUrl: '/content/appfavs/templates/tabs.html',
        controller: 'TabsCtrl',
        // don't load the state until we've populated our User, if necessary.
        resolve: {
            populateSession: function (User) {
                return User.CheckSession();
            }
        },
        onEnter: function ($state, User) {
            User.CheckSession().then(function (hasSession) {
                if (!hasSession) $state.go('splash');
            });
        }
    })

    // Each tab has its own nav history stack:

    .state('tab.discover', {
        url: '/discover',
        views: {
            'tab-discover': {
                templateUrl: '/content/appfavs/templates/discover.html',
                controller: 'DiscoverCtrl'
            }
        }
    })

    .state('tab.favorites', {
        url: '/favorites',
        views: {
            'tab-favorites': {
                templateUrl: '/content/appfavs/templates/favorites.html',
                controller: 'FavoritesCtrl'
            }
        }
    })
    // if none of the above states are matched, use this as the fallback
    $urlRouterProvider.otherwise('/');

})


.constant('SERVER', {
    url: 'http://localhost:49094/'
    //url: ''
});