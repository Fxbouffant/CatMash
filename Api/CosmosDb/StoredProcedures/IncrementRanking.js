function incrementRanking(catId) {
    if (typeof catId !== "string") throw new Error("CatId is not valid");

    var collection = getContext().getCollection();

    var getRequestStatus = collection.queryDocuments(collection.getSelfLink(),
        "SELECT * FROM c",
        {},
        function (err, items, responseOptions) {
            if (err) throw new Error("Unable to get cat ranking with id " + catId + ". Error: " + err.Message);
            if (items.length !== 1) throw new Error("CatId " + catId + " not found");
            updateRanking(items[0]);
        });

    if (!getRequestStatus) throw new Error("Unable to get cat ranking with id " + catId + ".");

    function updateRanking(catRanking) {
        catRanking.counter += 1;
        var updateRequestStatus = collection.upsertDocument(collection.getSelfLink(), catRanking);

        if (!updateRequestStatus) throw new Error("Unable to UpdateRanking with id " + catRanking.catId + ".")
    }
}