(function() {
	var docs = db.getMongo().getDB("masstransit");

	var cursor = docs.fs.files.find({"metadata.expiration" : {$lte : new Date()}});
	
	cursor.forEach(function (toDelete) {
		var id = toDelete._id;
		docs.fs.chunks.remove({files_id : id});
		docs.fs.files.remove({_id : id});
	});
})();