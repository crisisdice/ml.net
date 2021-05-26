var connection = new signalR.HubConnectionBuilder().withUrl("/socket-logging").build();

connection.on("ReceiveMessage", function (message) {
     const log_entry = document.createElement("div");
     const log_preface = document.createElement("span");

     log_preface.className = `logging-${message.level.toLowerCase()}`;
     log_entry.className = "logentry";

     log_preface.textContent = `${message.preface}`;
     log_entry.textContent = `${message.entry}`;

     log_entry.prepend(log_preface);
     document.getElementById("messagesList").appendChild(log_entry);
 });

 connection.start().then(function () { console.log("started") }).catch(function (err) {
     return console.error(err.toString());
});

function send_data() {
	const rating = document.getElementById("data").value;
	const model = document.getElementById("training-model").value;

	if (rating === "")
		return;

	const url = new URL("/api", location.origin);
	url.searchParams.append("data", rating);
	url.searchParams.append("model", model);

	fetch(url)
		.then(response => response.text())
		.then(text => {
			document.getElementById("prediction").innerHTML = text;
			document.getElementsByClassName("prediction")[0].style.visibility = "visible";
		});
}

function send_score(event) {
	const text = document.getElementById("data").value;
	const correct = event.target.value === "1";
	const emotion = document.getElementById("prediction").innerHTML === "😀" ? true : false;

	const request = {
		text: text,
		score: (correct ? emotion : !emotion) ? "1" : "0"
	};

	const url = new URL("/api/data", location.origin);

	text.value = "";
	document.getElementsByClassName("prediction")[0].style.visibility = "hidden";

	fetch(url, { method: "POST", body: JSON.stringify(request), headers: { 'Content-Type': "application/json" } });
}

function train() {
	const model = document.getElementById("training-model").value;

	const request = {
		model: model
	};

	const url = new URL("/api/training", location.origin);

	fetch(url, { method: "POST", body: JSON.stringify(request), headers: { 'Content-Type': "application/json" } });
}
