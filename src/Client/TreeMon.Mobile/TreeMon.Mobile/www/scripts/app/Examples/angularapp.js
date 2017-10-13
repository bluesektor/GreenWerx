'use strict';
(function () {

    var app = angular.module("OmniviewModule", []);


    var DashboardController = function ($scope, $http) {

        $scope.fileCheckboxModel = {
            Properties: false,
            Notes: false,
            SFID : false
        };

        $scope.GetParseFileResults = function () {


            var myUrl = "/OmniView/Default/ReconcileFiles" + $scope.fileCheckboxModel.Properties + "/" + $scope.fileCheckboxModel.Notes;
            $http.get("/OmniView/Default/ReconcileFiles", { params: { Properties: $scope.fileCheckboxModel.Properties, Notes: $scope.fileCheckboxModel.Notes, SFID: $scope.fileCheckboxModel.SFID } })
                .then(onParseFileApiResultsComplete, onError);

        };
        var onParseFileApiResultsComplete = function (response) {
            $scope.JSONReconcileResults = response.data;

        };



        $scope.GetErrorLogDataByTime = function (datetimelogged) {
            alert('working');
            if (datetimelogged !== null) {
            } else {

                datetimelogged = new Date($.now());
            }
            $http.get("/v1/Dashboard/CheckErrorLog/" + datetimelogged)
                .then(onErrorLogApiResultsComplete, onError);

        };
        $scope.GetErrorLogDataByCount = function (errorsToDisplay) {
            alert('working');
            $http.get("/v1/Dashboard/CheckErrorLogByCount/" + errorsToDisplay)
                .then(onErrorLogApiResultsComplete, onError);

        };
        var onErrorLogApiResultsComplete = function (response) {
            $scope.JsonErrorData = response.data;

        };



        var onError = function () {
            alert('got error');
            $scope.apiError = "Could not fetch the data.";
        };



        $scope.testAnjular = function () {
            alert('angular is working');
        }


    };

    var ToolsController = function ($scope, $http) {

        $scope.fileCheckboxModel = {
            Properties: false,
            Notes: false,
            SFID: false
        };

        $scope.textBoxContact =
            {
                ContactName: ""
                .SalesForceId = ""
            };


        $scope.GetContactInfoFromSF = function () {

            $http.get("/v1/Contact/SF/SearchContact/" + $scope.textBoxContact.ContactName + "/" + $scope.textBoxContact.SalesForceIdv )
                .then(onParseFileApiResultsComplete, onError);

        };
        var onParseFileApiResultsComplete = function (response) {
            $scope.JSONReconcileResults = response.data;

        };


        $scope.GetParseFileResults = function () {


            var myUrl = "/OmniView/Default/ReconcileFiles" + $scope.fileCheckboxModel.Properties + "/" + $scope.fileCheckboxModel.Notes;
            $http.get("/OmniView/Default/ReconcileFiles", { params: { Properties: $scope.fileCheckboxModel.Properties, Notes: $scope.fileCheckboxModel.Notes, SFID: $scope.fileCheckboxModel.SFID } })
                .then(onParseFileApiResultsComplete, onError);

        };
        var onParseFileApiResultsComplete = function (response) {
            $scope.JSONReconcileResults = response.data;

        };
    }

    var MarketingController = function ($scope, $http) {

        var marketSummary = this;
        marketSummary.ListingId = 0;
        marketSummary.Listing = null
        marketSummary.MarketingSummaryLoaded = false;
        marketSummary.Brokers = null;
        marketSummary.SummaryData = null; 
        marketSummary.SummaryComments = null;
        marketSummary.SummaryNotes = null;
        marketSummary.InterestedBuyers = null;
        marketSummary.IncludePhoneNumber = false;
        marketSummary.Offers = null;
        marketSummary.AverageOffer = 0;
        marketSummary.EscrowComments = null;


        $scope.Initialize = function () {
           
            marketSummary.ListingId = getParameterByName('lid');  //16072 , 5269
            console.log("Initializing listing:" + marketSummary.ListingId);
            $http.get("/v1/Listings/GetListing/" + marketSummary.ListingId ).then(onMarketingReportGetListingComplete, onError);
        };

        var onMarketingReportGetListingComplete = function (response) {

            marketSummary.Listing = response.data;
            marketSummary.Brokers = marketSummary.Listing.Brokers;

            marketSummary.MarketingSummaryLoaded = true;
            
            console.log("MarketingSummaryLoaded:" + marketSummary.MarketingSummaryLoaded);
            //  respone.data.ListPrice
            //$scope.JsonErrorData = response.data;
            if (marketSummary.Listing.ListPrice > 0) {
                //  listing.ListPrice = //TODO remove decimals
                //<h2>{{ listing.ListPrice | currency}}</h2>
            }
            else {
                marketSummary.Listing.ListPrice = "Unlisted";
            }
            //    res = "Unlisted");
        };


        $scope.GetReportData = function ($event, dataToDisplay) {

            if (document.getElementById(dataToDisplay).checked)
                console.log("show:" + dataToDisplay);
            else
                console.log("hide:" + dataToDisplay);

            // var ctl = $('#' + dataToDisplay);
            // ctl.prop("checked",  !ctl.prop("checked"));
            var checkbox = $event.target;
            checkbox.checked = true;
            //var action = (checkbox.checked ? 'add' : 'remove');

            switch (dataToDisplay) {

                case 'MarketingSummary':
                    $http.get("/v1/Listing/MarketSummaryData/" + marketSummary.ListingId).then( function (response) {
                        marketSummary.SummaryData = response.data; }, onError);

                    $http.get("/v1/Listing/MarketSummaryComments/" + marketSummary.ListingId).then(function (response) {
                        marketSummary.SummaryComments = response.data; }, onError);
                    break;

                case 'MarketingUpdate':
                    $http.get("/v1/Listing/MarketSummaryNotes/" + marketSummary.ListingId).then(function (response) {
                        marketSummary.SummaryNotes =  response.data;
                    }, onError);
                    break;
                case 'InterestedBuyers':
                    $http.get("/v1/Listing/InterestedBuyers/" + marketSummary.ListingId).then(function (response) {
                        marketSummary.InterestedBuyers = response.data;
                    }, onError);

                    break;
                case 'IncludePhoneNumber':
                    marketSummary.IncludePhoneNumber = !marketSummary.IncludePhoneNumber;
                    break;
                    
                case 'OfferSummary':
                    $http.get("/v1/Listing/Offers/" + marketSummary.ListingId).then(function (response) {
                        marketSummary.Offers = response.data;

                        if (marketSummary.Offers.length == 0)
                        { return; }

                        var sum = 0;
                        for(var i = 0; i < marketSummary.Offers.length; i++ )
                        {
                            sum += parseInt( marketSummary.Offers[i].Price,10);
                        }
                        marketSummary.AverageOffer = sum / marketSummary.Offers.length;
                    }, onError);

                    break;
                case 'EscrowTracking':

                    $http.get("/v1/Listing/EscrowComments/" + marketSummary.ListingId).then(function (response) {
                        marketSummary.EscrowComments = response.data;
                    }, onError);
                    break;

                case 'ContactInformation':
                    break;
                case 'HideSensitiveBuyerInformation':
                    
                    break;
            }
        };

        var onMarketingReportDataApiResultsComplete = function (response) {

            console.log(response);
            
            //switch (dataToDisplay) {
            //    case 'MarketSummaryData':
            //        //   marketSummary.SummaryData = response; //Displays in timeline 
            //        break;
            //    case 'MarketSummaryComments':
            //        //   marketSummary.SummaryComments = response;//Displays in marketingsummarycomments 
            //        break;
            //}


            marketSummary.MarketingSummaryLoaded = true;
            console.log("MarketingSummaryLoaded:" + marketSummary.MarketingSummaryLoaded);
        };

        var onError = function () {
           
            $scope.apiError = "Could not fetch the data.";
        };

        $scope.testMarketing = function () {
            alert('angular is working');
        }
    };

    var compareTo = function () {
        return {
            require: "ngModel",
            scope: {
                otherModelValue: "=compareTo"
            },
            link: function (scope, element, attributes, ngModel) {

                ngModel.$validators.compareTo = function (modelValue) {
                    return modelValue == scope.otherModelValue;
                };

                scope.$watch("otherModelValue", function () {
                    ngModel.$validate();
                });
            }
        };
    };

    app.directive("compareTo", compareTo);

    app.controller("DashboardController", ["$scope", "$http", DashboardController]);
    app.controller("MarketingController", ["$scope", "$http", MarketingController]);
    app.controller("ToolsController", ["$scope", "$http", ToolsController]);

})();
