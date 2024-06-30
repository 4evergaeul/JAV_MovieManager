export function getMovies() {
    return fetch("http://localhost:5100/movies", {
        method: "GET",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
};

export function getMostRecentMovies() {
    return fetch("http://localhost:5100/movies/recent", {
        method: "GET",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
};

export function getMovieYears() {
    return fetch("http://localhost:5100/movies/years", {
        method: "GET",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
};

export function getMoivesByFilter(filterType, filters, isAndOperator) {
    return fetch("http://localhost:5100/movies/filters", {
        method: "POST",
        mode: "cors",
        body: JSON.stringify({
            filterType: filterType,
            filters: filters,
            isAndOperator: isAndOperator
        }),
        headers: {
            "Content-Type": "application/json"
        }
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getMoviesWildcardSearch(searchString, searchString2, searchType) {
    return fetch("http://localhost:5100/movies/wildcard", {
        method: "POST",
        mode: "cors",
        body: JSON.stringify({
            searchString: searchString,
            searchString2: searchString2,
            searchType: searchType
        }),
        headers: {
            "Content-Type": "application/json"
        }
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getMoviesQuerySearch(searchString) {
    return fetch("http://localhost:5100/movies/querySearch", {
        method: "POST",
        mode: "cors",
        body: JSON.stringify({ searchString: searchString }),
        headers: {
            "Content-Type": "application/json"
        }
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getLikedMovies() {
    return fetch("http://localhost:5100/movies/like", {
        method: "GET",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getMovieDetails(movieViewModel) {
    return fetch("http://localhost:5100/movies/details", {
        method: "POST",
        mode: "cors",
        body: JSON.stringify({
            imdbId: movieViewModel?.imdbId,
            title: movieViewModel?.title,
            posterFileLocation: movieViewModel?.posterFileLocation,
            fanArtLocation: movieViewModel?.fanArtLocation,
            movieLocation: movieViewModel?.movieLocation,
            dateAdded: movieViewModel?.dateAdded,
            director: movieViewModel?.director ?? ""
        }),
        headers: {
            "Content-Type": "application/json"
        }
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function likeMovie(searchString) {
    return fetch("http://localhost:5100/movies/like/" + searchString, {
        method: "PUT",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function scanAndAddNewMovies(days) {
    return fetch("http://localhost:5100/movies/addnew/" + days, {
        method: "PUT",
        mode: "cors",
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();;
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function deleteMovies() {
    return fetch("http://localhost:5100/movies/delete/", {
        method: "DELETE",
        mode: "cors",
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();;
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}


export function getAllActorNames() {
    return fetch("http://localhost:5100/actors/names", {
        method: "GET",
        mode: "cors",
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getActorsByRanges(heightLower, heightUpper, cupLower, cupUpper, age) {
    return fetch("http://localhost:5100/actors/ranges", {
        method: "POST",
        mode: "cors",
        body: JSON.stringify({
            heightLower: heightLower,
            heightUpper: heightUpper,
            cupLower: cupLower,
            cupUpper: cupUpper,
            age: age
        }),
        headers: {
            "Content-Type": "application/json"
        }
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getActorByName(searchString) {
    return fetch("http://localhost:5100/actors/" + searchString, {
        method: "GET",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getActorByNames(names) {
    return fetch("http://localhost:5100/actors/getbynames", {
        method: "POST",
        mode: "cors",
        body: JSON.stringify(names),
        headers: {
            "Content-Type": "application/json"
        }
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getLikedActors() {
    return fetch("http://localhost:5100/actors/like", {
        method: "GET",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function likeActor(searchString) {
    return fetch("http://localhost:5100/actors/like/" + searchString, {
        method: "PUT",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getAllGenres() {
    return fetch("http://localhost:5100/genres", {
        method: "GET",
        mode: "cors",
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getGenreByName(searchString) {
    return fetch("http://localhost:5100/genres/" + searchString, {
        method: "GET",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getAllGenreNames() {
    return fetch("http://localhost:5100/genres/names", {
        method: "GET",
        mode: "cors",
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getLikedGenres() {
    return fetch("http://localhost:5100/genres/like", {
        method: "GET",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function likeGenre(searchString) {
    return fetch("http://localhost:5100/genres/like/" + searchString, {
        method: "PUT",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getAllTags() {
    return fetch("http://localhost:5100/tags", {
        method: "GET",
        mode: "cors",
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getTagByName(searchString) {
    return fetch("http://localhost:5100/tags/" + searchString, {
        method: "GET",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getAllTagNames() {
    return fetch("http://localhost:5100/tags/names", {
        method: "GET",
        mode: "cors",
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getLikedTags() {
    return fetch("http://localhost:5100/tags/like", {
        method: "GET",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function likeTag(searchString) {
    return fetch("http://localhost:5100/tags/like/" + searchString, {
        method: "PUT",
        mode: "cors"
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getAllDirectorNames() {
    return fetch("http://localhost:5100/directors/names", {
        method: "GET",
        mode: "cors",
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function createPotPlayerPlayList(movies, playListName) {
    return fetch("http://localhost:5100/playlist/create/" + playListName, {
        method: "POST",
        mode: "cors",
        body: JSON.stringify(movies),
        headers: {
            "Content-Type": "application/json"
        }
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function createPotPlayerPlayListByActors(actors, playListName) {
    return fetch("http://localhost:5100/playlist/createbyactors/" + playListName, {
        method: "POST",
        mode: "cors",
        body: JSON.stringify(actors),
        headers: {
            "Content-Type": "application/json"
        }
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function addToDefaultPotPlayerPlayList(movies) {
    return fetch("http://localhost:5100/playlist/append/default", {
        method: "PUT",
        mode: "cors",
        body: JSON.stringify(movies),
        headers: {
            "Content-Type": "application/json"
        }
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function getUserSettings() {
    return fetch("http://localhost:5100/usersettings", {
        method: "GET",
        mode: "cors",
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
        return resp.json();
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}

export function updateUserSettings(userSettings) {
    return fetch("http://localhost:5100/usersettings/update", {
        method: "PUT",
        mode: "cors",
        body: JSON.stringify(userSettings),
        headers: {
            "Content-Type": "application/json"
        }
    }).then(resp => {
        if (!resp.ok) {
            throw resp;
        }
    }).then(resp => {
        return resp;
    }).catch(error => {
        console.log(error);
    });
}