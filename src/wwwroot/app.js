function send() {
	const rating = document.getElementById("data").value;
	const url = new URL("/api", location.origin);
	url.searchParams.append("data", rating);

	fetch(url)
		.then(response => response.text())
		.then(text => {
			document.getElementById("prediction").innerHTML = text;
			document.getElementsByClassName("prediction")[0].style.visibility = "visible";
		});
}

function send_score(event) {
	const text = document.getElementById("data").value;
	const score = event.target.value;

	const request = {
		text: text,
		score: score
	};
	const url = new URL("/api", location.origin);

	text.value = "";
	document.getElementsByClassName("prediction")[0].style.visibility = "hidden";

	fetch(url, { method: "POST", body: JSON.stringify(request), headers: { 'Content-Type': 'application/json'}  })
}
	
