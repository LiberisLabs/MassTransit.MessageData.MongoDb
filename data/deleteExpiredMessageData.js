(function() {
	var docs = db.getMongo().getDB("docs");
	var now = new Date().toISOString();

	var cursor = docs.fs.files.find({"metadata.expiration" : {$lte : new Date(now)}});
	
	cursor.forEach(function (toDelete) {
		var id = toDelete._id;
		docs.fs.chunks.remove({files_id : id});
		docs.fs.files.remove({_id : id});
	});
})();