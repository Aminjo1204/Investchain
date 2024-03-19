

function myFunction(){

	var p = document.getElementById("info")
	var lol = document.getElementById("preis")
	var lol2 = document.getElementById("name")
	var lol3 = document.getElementById("nutzerNameInput").value;
	console.log(lol3);

	const options = {
		method: 'GET',
		headers: {
			'X-RapidAPI-Key': '7a79eb630emshabd241ebf54ecf6p1d1703jsnf99e83b9d89f',
			'X-RapidAPI-Host': 'apidojo-yahoo-finance-v1.p.rapidapi.com'
		}
	};
	
	fetch('https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v3/get-chart?interval=1mo&symbol='+ lol3 + '&range=5y&region=US&includePrePost=false&useYfid=true&includeAdjustedClose=true&events=capitalGain%2Cdiv%2Csplit', options)
		.then(response => response.json())
		.then(response => console.log(response))
		//.then((response) => {
			//p.innerText = JSON.stringify(response)
			
		//	const name = response.chart.result[0].meta.regularMarketPrice
		//	lol.innerText = name

		//	const preis = response.chart.result[0].meta.symbol
		//	lol2.innerText = preis
		//})
		.catch(err => console.error(err));

		//const n = data.chart.result[0].meta.regularMarketPrice
		//n.innerText = börsenName;
}
