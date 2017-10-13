var app = angular.module('myApp', ['ionic']);

app.config(function ($stateProvider, $urlRouterProvider) {
    $stateProvider
    .state('index', {
        url: '/',
        templateUrl: 'home.html'
    })
    .state('music', {
        url: '/second',
        templateUrl: 'second.html'
    });
    $urlRouterProvider.otherwise('/');
});

app.controller('MainCtrl', function ($scope) {

});