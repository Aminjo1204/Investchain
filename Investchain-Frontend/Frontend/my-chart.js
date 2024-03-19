var xValues;
var yValues;
var buttonValue = "5y";

document.body.addEventListener("click", event => {
    if (event.target.nodeName == "BUTTON") {
        console.log("Clicked", event.target.textContent);
        buttonValue = event.target.value;
        console.log(buttonValue);
        myFunction();
    }
});

function myFunction() {
    var assetname = document.getElementById("nutzerNameInput").value;

    const options = {
        method: 'GET',
        headers: {
            'X-RapidAPI-Key': '7a79eb630emshabd241ebf54ecf6p1d1703jsnf99e83b9d89f',
            'X-RapidAPI-Host': 'apidojo-yahoo-finance-v1.p.rapidapi.com'
        }
    };

    fetch('https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v3/get-chart?interval=1mo&symbol=' + assetname + '&range=' + buttonValue + '&region=US&includePrePost=false&useYfid=true&includeAdjustedClose=true&events=capitalGain%2Cdiv%2Csplit', options)
        .then(response => response.json())
        .then((response) => {
            var highValues = response.chart.result[0].indicators.quote[0].high;
            // Extracting only the high values
            yValues = highValues.map(entry => entry * 1); // Convert values to numbers
            if (buttonValue == "5y") {
                // If 5 years button is clicked, xValues will have 5 years (60 months)
                xValues = Array.from({length: 60}, (_, index) => index + 1);
            } else {
                // If other button is clicked, xValues will have 12 months (1 year)
                xValues = Array.from({length: 12}, (_, index) => index + 1);
            }
            myFunction2();
        })
        .catch(err => console.error(err));
}

function myFunction2() {
    //Chart-Design beginnt hier
    new Chart("myChart", {
        type: "line",
        data: {
            labels: xValues,
            datasets: [{
                tickColor: "rgba(0,0,0,0.5)",
                fill: true,
                lineTension: 0,
                backgroundColor: "rgba(0,255,255,0.5)",
                borderColor: "rgba(0,0,0,1)",
                data: yValues
            }]
        },
        options: {
            responsive: true,
            legend: { display: true },
            scales: {
                yAxes: [{ ticks: { min: 0, max: 300 } }],
            },
            animations: {
                radius: {
                    duration: 1000,
                    easing: 'linear',
                    loop: (context) => context.active
                }
            }
        }
    });
}

// Initial Call to fetch data and render chart
myFunction();
