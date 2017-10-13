'use strict';

(function () {
var siteApp = angular.module('application', []);


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
                   config.headers['Authorization'] = "TreeMon " + token;
                   return config;
               }
           };
    });

    siteApp.config(function ($httpProvider) {
        $httpProvider.interceptors.push('httpRequestInterceptor');
    });
})();

