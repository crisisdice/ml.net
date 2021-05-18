function send() {
	let url = new URL("/api", location.origin);
	let rating = document.getElementById("data").value;
	
	url.searchParams.append("data", rating);

	fetch(url)
		.then(response => response.text())
		.then(text => {
			document.getElementById("prediction").innerHTML = text;
		});
}
	
