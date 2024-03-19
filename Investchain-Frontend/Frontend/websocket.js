// Create a new WebSocket object
var socket = new WebSocket("wss://ws.twelvedata.com/v1/quotes/price?apikey=56c70fce91534b639d106bc9b49ff629");

// Define a mapping between symbol and table row
var rows = {};

// Initialize walletMoney
var walletMoney = getCookie("walletMoney") || 100000;

var portfolio = document.getElementById('portfolio');
var depotValueElement = document.getElementById('depot-value');
var portfolioValueElement = document.getElementById('portfolio-value');

// Create an empty array to store the trade history
var tradeHistory = [];

// Create the symbol name mapping
var symbolNames = {
    "AAPL": "Apple Inc.",
    "BTC/USD": "Bitcoin",
    "EBS": "Erste Group",
    "ISP": "Intesa Sanpaolo S.p.A.",
    "DIS": "The Walt Disney Company",
    "VOW3": "Volkswagen",
    "EUR/USD": "Euro/US-Dollar",
    "THYAO": "Turkish Airlines"
};

portfolio.innerHTML = formatEuro(walletMoney);

// Handle the onopen event
socket.onopen = function(event) {
    console.log("WebSocket connection established.");
    // Subscribe to the symbols you want to receive data for
    socket.send('{"action": "subscribe", "params": {"symbols": "AAPL,BTC/USD,EBS:VSE,ISP:MTA,DIS,VOW3:XETR,EUR/USD,THYAO:BIST"}}');
};

// Handle the onmessage event
socket.onmessage = function(event) {
    // Parse the JSON data received from the server
    var data = JSON.parse(event.data);

    // Check the type of event
    if (data.event == "price") {
        // Display the price data
        var symbol = data.symbol;
        var price = data.price;
        var timestamp = data.timestamp;
        var volume = data.day_volume;

        if (rows[symbol]) {
            rows[symbol].innerHTML = "<td>" + symbol + "</td><td>" + symbolNames[symbol] + "</td><td>" + price + "</td><td>" + timestamp + "</td><td>" + volume + "</td><td><button class='btn' role='button' onclick='buy(\"" + symbol + "\", " + price + ")'>Buy</button></td><td><button class='btn' role='button' onclick='sell(\"" + symbol + "\", " + price + ", " + purchasePrice + ")'>Sell</button></td>";

            // Calculate and display the percentage change since purchase
            var purchaseInfo = getPurchaseInfo(symbol);
            if (purchaseInfo) {
                var purchasePrice = purchaseInfo.price;
                var percentageChange = ((price - purchasePrice) / purchasePrice) * 100;
                rows[symbol].innerHTML += "<td>" + percentageChange.toFixed(2) + "%</td>";
            }
        } else {
            console.log("Symbol not found: " + symbol);
        }
    } else if (data.event == "subscribe-status") {
        var status = data.status;
        if (status == "ok") {
            var symbols = data.success;
            var message = "Subscribed to symbols: ";
            for (var i = 0; i < symbols.length; i++) {
                message += symbols[i].symbol + " ";

                var table = document.getElementById("stock-table");
                var row = document.createElement("tr");
                row.setAttribute("id", symbols[i].symbol);

                var symbolCell = document.createElement("td");
                symbolCell.innerHTML = symbols[i].symbol;
                row.appendChild(symbolCell);

                // Add the symbol name column
                var nameCell = document.createElement("td");
                nameCell.innerHTML = symbolNames[symbols[i].symbol] || "Unknown";
                row.appendChild(nameCell);

                var priceCell = document.createElement("td");
                priceCell.innerHTML = "-";
                row.appendChild(priceCell);

                var timestampCell = document.createElement("td");
                timestampCell.innerHTML = "-";
                row.appendChild(timestampCell);

                var volumeCell = document.createElement("td");
                volumeCell.innerHTML = "-";
                row.appendChild(volumeCell);

                var buyCell = document.createElement("td");
                var buyButton = document.createElement("button");
                buyButton.textContent = "Buy";
                buyButton.addEventListener("click", function() {
                    buy(symbols[i].symbol, parseFloat(priceCell.innerHTML));
                });
                buyCell.appendChild(buyButton);
                row.appendChild(buyCell);

                var sellCell = document.createElement("td");
                var sellButton = document.createElement("button");
                sellButton.textContent = "Sell";
                sellButton.addEventListener("click", function() {
                    sell(symbols[i].symbol, parseFloat(priceCell.innerHTML));
                });
                sellCell.appendChild(sellButton);
                row.appendChild(sellCell);
                table.querySelector("tbody").appendChild(row);

                rows[symbols[i].symbol] = row;
            }
            console.log(message);
        } else {
            console.log("Subscription failed.");
        }
    }
};

socket.onclose = function(event) {
    console.log("WebSocket connection closed.");
};

var purchasePrice;
var purchasedAmount;

function buy(symbol, price) {
    var amount = prompt("Enter the amount you want to buy:");
    // Check if amount is greater than 0
    if (amount <= 0 || isNaN(amount)) {
        alert("Invalid amount entered. Please enter a number greater than 0.");
        return;
    }
    var cost = amount * price;
    if (cost > walletMoney) {
        alert("Not enough money in wallet.");
    } else {
        walletMoney -= cost;
        purchasePrice = price;
        sharesOwned[symbol] = (sharesOwned[symbol] || 0) + parseInt(amount);
        totalSharesOwned += parseInt(amount);
        alert(amount + " " + symbol + " bought at " + formatEuro(price) + " each for a total cost of " + formatEuro(cost) + ".");
        // Add the buy trade to the trade history
        var trade = {
            type: "Buy",
            symbol: symbol,
            date: new Date(),
            price: price,
            amount: parseInt(amount)
        };
        tradeHistory.push(trade);
        savePortfolioData(); // Save the updated data
    }
    updatePortfolioValues();
}


var profitLoss = {};
var sharesOwned = {};
var totalSharesOwned = 0;

function sell(symbol, price) {
    var amount = prompt("Enter the amount you want to sell:");
    if (sharesOwned[symbol] >= amount) {
        var p = profitLoss[symbol] || 0;
        var pl = (price - purchasePrice) * amount;
        p += pl;
        profitLoss[symbol] = p;
        sharesOwned[symbol] -= amount;
        totalSharesOwned -= amount;
        walletMoney += price * amount;
        alert(amount + " " + symbol + " sold at " + formatEuro(price) + " each for a total sale of " + formatEuro(price * amount) + ".");
        // Add the sell trade to the trade history
        var trade = {
            type: "Sell",
            symbol: symbol,
            date: new Date(),
            price: price,
            amount: parseInt(amount)
        };
        tradeHistory.push(trade);
        savePortfolioData(); // Save the updated data
    } else {
        alert("Not enough shares of " + symbol + " to sell.");
    }
    updatePortfolioValues();
}

function formatEuro(value) {
    return '€ ' + value.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

function updatePortfolioValues() {
    var depotValue = 0;
    for (var symbol in sharesOwned) {
        var row = rows[symbol];
        var price = parseFloat(row.cells[2].innerHTML);
        var amount = sharesOwned[symbol];
        var value = price * amount;
        depotValue += value;
    }
    var portfolioValue = walletMoney + depotValue;

    var previousPortfolioValue = parseFloat(portfolioValueElement.innerHTML.replace('€ ', ''));
    var percentageChange = ((portfolioValue / previousPortfolioValue) - 1) * 100;
    portfolioValueElement.innerHTML = formatEuro(portfolioValue);

    depotValueElement.innerHTML = formatEuro(depotValue);
    portfolioValueElement.innerHTML = formatEuro(portfolioValue);
    portfolio.innerHTML = formatEuro(walletMoney);

    // Update the trade history table
    var tradeTable = document.getElementById("trade-table");
    tradeTable.innerHTML = ""; // Clear the table first

    for (var i = 0; i < tradeHistory.length; i++) {
        var trade = tradeHistory[i];
        var row = document.createElement("tr");

        var typeCell = document.createElement("td");
        typeCell.innerHTML = trade.type;
        row.appendChild(typeCell);

        var symbolCell = document.createElement("td");
        symbolCell.innerHTML = trade.symbol;
        row.appendChild(symbolCell);
        var dateCell = document.createElement("td");
        dateCell.innerHTML = trade.date.toLocaleString();
        row.appendChild(dateCell);

        var priceCell = document.createElement("td");
        priceCell.innerHTML = formatEuro(trade.price);
        row.appendChild(priceCell);

        var amountCell = document.createElement("td");
        amountCell.innerHTML = trade.amount;
        row.appendChild(amountCell);

        tradeTable.appendChild(row);
    }

}

symbolCell.addEventListener("click", function() {
    redirectToOtherPage(symbols[i].symbol);
});

function redirectToOtherPage(symbol) {
    switch (symbol) {
        case "AAPL":
            window.location.href = "aktienmaerkte.html";
            break;
        case "VOW3":
            window.location.href = "vw.html";
            break;
            // Füge hier weitere Symbole und ihre entsprechenden Seiten hinzu
        default:
            // Standardfall, wenn das Symbol nicht erkannt wird
            break;
    }
}

function savePortfolioData() {
    // Create an object to store the portfolio data
    var portfolioData = {
        walletMoney: walletMoney,
        sharesOwned: sharesOwned,
        totalSharesOwned: totalSharesOwned,
        tradeHistory: tradeHistory
    };

    // Convert the portfolio data object to JSON string
    var jsonData = JSON.stringify(portfolioData);

    // Save the JSON data to cookies
    document.cookie = "portfolioData=" + jsonData + "; expires=" + getCookieExpirationDate(365) + "; path=/";
}

function loadPortfolioData() {
    // Retrieve the portfolio data from cookies
    var cookies = document.cookie.split(";");
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i].trim();
        if (cookie.startsWith("portfolioData=")) {
            var jsonData = cookie.substring("portfolioData=".length);
            var portfolioData = JSON.parse(jsonData);

            // Update the portfolio values with the loaded data
            walletMoney = portfolioData.walletMoney || 0;
            sharesOwned = portfolioData.sharesOwned || {};
            totalSharesOwned = portfolioData.totalSharesOwned || 0;
            tradeHistory = portfolioData.tradeHistory || [];
            break;
        }
    }
}

function getCookie(name) {
    var cookies = document.cookie.split(";");
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i].trim();
        if (cookie.startsWith(name + "=")) {
            return cookie.substring(name.length + 1);
        }
    }
    return null;
}

function getCookieExpirationDate(days) {
    var date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    return date.toUTCString();
}

loadPortfolioData();
updatePortfolioValues();