/// <reference path="/about.html" />
angular.module('ionicApp', ['ionic'])

.config(function ($stateProvider, $urlRouterProvider) {

    $stateProvider
      .state('tabs', {
          url: "/tab",
          abstract: true,
          templateUrl: "tabs.html"
      })
      .state('tabs.home', {
          url: "/home",
          views: {
              'home-tab': {
                  templateUrl: "../../content/examples.templates/tabs/home.html",
                  controller: 'HomeTabCtrl'
              }
          }
      })
      .state('tabs.facts', {
          url: "/facts",
          views: {
              'home-tab': {
                  templateUrl: "../../content/examples.templates/tabs/facts.html"
              }
          }
      })
      .state('tabs.facts2', {
          url: "/facts2",
          views: {
              'home-tab': {
                  templateUrl: "../../content/examples.templates/tabs/facts2.html"
              }
          }
      })
      .state('tabs.about', {
          url: "/about",
          views: {
              'about-tab': {
                  templateUrl: "../../content/examples.templates/tabs/about.html"
              }
          }
      })
      .state('tabs.navstack', {
          url: "/navstack",
          views: {
              'about-tab': {
                  templateUrl: "../../content/examples.templates/tabs/nav-stack.html"
              }
          }
      })
      .state('tabs.contact', {
          url: "/contact",
          views: {
              'contact-tab': {
                  templateUrl: "../../content/examples.templates/tabs/contact.html"
              }
          }
      });


    $urlRouterProvider.otherwise("/tab/home");

})

.controller('HomeTabCtrl', function ($scope) {
    console.log('HomeTabCtrl');
});