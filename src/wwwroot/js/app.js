var connection = new signalR.HubConnectionBuilder().withUrl("/socket-logging").build();

connection.on("ReceiveMessage", function (message) {
     var log_entry = document.createElement("div");
     var log_preface = document.createElement("span");

     log_preface.className = `logging-${message.level.toLowerCase()}`;
     log_entry.className = "logentry";

     log_preface.textContent = `${message.preface}`;
     log_entry.textContent = `${message.entry}`;

     log_entry.prepend(log_preface);
     document.getElementById("messagesList").appendChild(log_entry);

     console.log(message);
     console.log(log_entry);
 });

 connection.start().then(function () { console.log("started") }).catch(function (err) {
     return console.error(err.toString());
});

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

