'use strict';

(function () {
	var siteApp = angular.module('application', ['ionic']);


    siteApp.controller('AppController',

        function EventController($scope, appService) {
     
            $scope.RunInstaller = false;
            $scope.ActiveDbProvider = "Select Your Provider";//sqlite,mysql, mssql..
            $scope.ActiveDatabase = "";
            $scope.ActiveDbUser = "";
            $scope.Settings = [];

            $scope.AccountName= "";
            $scope.AccountEmail= "";
            $scope.AccountIsPrivate = "";

            $scope.UserName = "";
            $scope.UserEmail = "";
            $scope.UserIsPrivate = "";
            $scope.UserPassword = "";
            $scope.UserPasswordConfirm = "";
            $scope.UserSecurityQuestion = "";
            $scope.UserSecurityAnswer = "";

            var dataTypes = ['String', 'Numeric', 'Decimal', 'Date Time', 'True/False'];

            //this is passing in an anonymous callback function.
            appService.getApplicationSettings( function (Options)
            {
                 // $scope.Settings = Options.Settings;
                $scope.RunInstaller = Options.RunInstaller;
                $scope.DataTypes = dataTypes;
            });
        }
    );

    siteApp.factory('appService', function ($http, $q, $log) {
            return {
                getApplicationSettings: function (cbSuccess) {

                    $http({ method: 'GET', url: '/api/Apps/Info' }).
                    success(function (data, status, headers, config) {   // $log.info(data, status, headers, config);

                        if (data.Status != "OK") {
                           // ShowMessage(data.Status, data.Message);
                            return false;
                        }
                        //Pass the data back, need to convert to object first..
                        cbSuccess( JSON.parse( data.Result ));
                    }).
                    error(function (data, status, headers, config) {
                        //  ShowMessage("error",  status);
                    });
                }
           };
    });

    //This add the authorization token to all the angular requests.
    siteApp.factory('httpRequestInterceptor', function () {
        return {
               request: function (config) {
                   var token = Cookies.get('Authorization'); //                   sessionStorage.getItem('Authorization');
                   config.headers['Authorization'] = "AppDomain " + token;
                   return config;
               }
           };
    });

    siteApp.config(function ($stateProvider, $urlRouterProvider,$httpProvider) {
    	$httpProvider.interceptors.push('httpRequestInterceptor');

    	// Ionic uses AngularUI Router which uses the concept of states
    	// Learn more here: https://github.com/angular-ui/ui-router
    	// Set up the various states which the app can be in.
    	// Each state's controller can be found in controllers.js
    	//$stateProvider
    	//.state('splash', {		//// splash page
		//	url: '/',
		//	templateUrl: '/templates/splash.html',
		//	controller: 'SplashCtrl',
		//	onEnter: function ($state, User) {
		//		User.CheckSession().then(function (hasSession) {
		//			if (hasSession) $state.go('tab.discover');
		//		});
		//	}
		//})
		//// setup an abstract state for the tabs directive
		//.state('tab', {
		//	url: '/tab',
		//	abstract: true,
		//	templateUrl: '/templates/tabs.html',
		//	controller: 'TabsCtrl',
		//	// don't load the state until we've populated our User, if necessary.
		//	resolve: {
		//		populateSession: function (User) {
		//			return User.CheckSession();
		//		}
		//	},
		//	onEnter: function ($state, User) {
		//		User.CheckSession().then(function (hasSession) {
		//			if (!hasSession) $state.go('splash');
		//		});
		//	}
		//})
		//// Each tab has its own nav history stack:
		//.state('tab.discover', {
		//	url: '/discover',
		//	views: {
		//		'tab-discover': {
		//			templateUrl: '/templates/discover.html',
		//			controller: 'DiscoverCtrl'
		//		}
		//	}
		//})

		//.state('tab.favorites', {
		//	url: '/favorites',
		//	views: {
		//		'tab-favorites': {
		//			templateUrl: '/templates/favorites.html',
		//			controller: 'FavoritesCtrl'
		//		}
		//	}
		//})
    	// if none of the above states are matched, use this as the fallback
    	$urlRouterProvider.otherwise('/');


    });

    siteApp.run(function ($ionicPlatform, $rootScope, $state, User) {
    	$ionicPlatform.ready(function () {
    		// Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
    		// for form inputs)
    		if (window.cordova && window.cordova.plugins.Keyboard) {
    			cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
    		}
    		if (window.StatusBar) {
    			StatusBar.styleDefault();
    		}
    	});
    });

    //siteApp.config(function ($httpProvider) {
    //    $httpProvider.interceptors.push('httpRequestInterceptor');
    //});
})();

