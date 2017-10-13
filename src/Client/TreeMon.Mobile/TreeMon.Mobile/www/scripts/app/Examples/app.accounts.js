'use strict';
(function () {

    var app = angular.module("accounts", ['ionic']);

    var MarketingController = function ($scope, $http) {

    };


    //app.controller("RegistrationController", ["$scope", "$http", RegistrationController]);
    
    app.factory('User', function ($q, $http, $localstorage, SERVER) {

        var o = {
            username: false,
            session_id: false,
            favorites: [],
            newFavorites: 0
        }

        // check if there's a user session present
        o.CheckSession = function () {
            var defer = $q.defer();

            if (o.session_id) {
                // if this session is already initialized in the service
                defer.resolve(true);

            } else {
                // detect if there's a session in localstorage from previous use.
                // if it is, pull into our service
                var user = $localstorage.getObject('user');

                if (user.username) {
                    // if there's a user, lets grab their favorites from the server
                    o.setSession(user.username, user.session_id);
                    o.populateFavorites().then(function () {
                        defer.resolve(true);
                    });

                } else {
                    // no user info in localstorage, reject
                    defer.resolve(false);
                }

            }

            return defer.promise;
        }


        // set session data
        o.setSession = function (username, session_id, favorites) {
            if (username) o.username = username;
            if (session_id) o.session_id = session_id;
            if (favorites) o.favorites = favorites;

            // set data in localstorage object
            $localstorage.setObject('user', { username: username, session_id: session_id });
        }

        // wipe out our session data
        o.DestroySession = function () {
            $localstorage.setObject('user', {});
            o.username = false;
            o.session_id = false;
            o.favorites = [];
            o.newFavorites = 0;
        }

        // log this user in
        o.auth = function (username, signingUp) {

            var authRoute;

            if (signingUp) {
                authRoute = 'signup';
            } else {
                authRoute = '/api/Account/Login'
            }

            var server = SERVER.url;
            if (server === '')
                server = 'http://localhost:58093/';

            return $http.post(server + '/' + authRoute, { username: username })
              .success(function (data) {
                  o.setSession(data.username, data.session_id, data.favorites);
              });
        }

        // gets the entire list of this user's favs from server
        o.populateFavorites = function () {
            return $http({
                method: 'GET',
                url: SERVER.url + '/favorites',
                params: { session_id: o.session_id }
            }).success(function (data) {
                // merge data into the queue
                o.favorites = data;
            });
        }

        o.addSongToFavorites = function (song) {
            // make sure there's a song to add
            if (!song) return false;

            // add to favorites array
            o.favorites.unshift(song);
            o.newFavorites++;

            // persist this to the server
            return $http.post(SERVER.url + '/favorites', { session_id: o.session_id, song_id: song.song_id });
        }

        o.removeSongFromFavorites = function (song, index) {
            // make sure there's a song to add
            if (!song) return false;

            // add to favorites array
            o.favorites.splice(index, 1);

            // persist this to the server
            return $http({
                method: 'DELETE',
                url: SERVER.url + '/favorites',
                params: { session_id: o.session_id, song_id: song.song_id }
            });

        }

        o.favoriteCount = function () {
            return o.newFavorites;
        }

        return o;
    });

})();
