class Portfolio {
    constructor(startingBalance) {
        this.balance = startingBalance;
        this.portfolio = {};
        this.orderHistory = [];
    }

    executeOrder(symbol, orderType, shares, price) {
        if (orderType === "Kaufen") {
            if (this.balance >= price * shares) {
                this.balance -= price * shares;
                if (symbol in this.portfolio) {
                    this.portfolio[symbol] += shares;
                } else {
                    this.portfolio[symbol] = shares;
                }
                this.orderHistory.push({symbol, orderType, shares, price});
                return true;
            }
        } else if (orderType === "Verkaufen") {
            if (symbol in this.portfolio && this.portfolio[symbol] >= shares) {
                this.balance += price * shares;
                this.portfolio[symbol] -= shares;
                this.orderHistory.push({symbol, orderType, shares, price});
                return true;
            }
        }
        return false;
    }

    displayPortfolio() {
        console.log("Portfolio:");
        for (const [symbol, shares] of Object.entries(this.portfolio)) {
            console.log(`- ${shares} shares of ${symbol}`);
        }
        console.log(`Balance: € ${this.balance.toFixed(2)}`);
    }

    displayOrderHistory() {
        console.log("Order History:");
        for (const order of this.orderHistory) {
            console.log(`- ${order.shares} shares of ${order.symbol} ${order.orderType}ed at € ${order.price.toFixed(2)}`);
        }
    }
}

const portfolio = new Portfolio(100000);

// Example of executing orders
portfolio.executeOrder("AAPL", "Kaufen", 100, 120);
portfolio.executeOrder("GOOG", "Kaufen", 50, 1000);
portfolio.executeOrder("AAPL", "Verkaufen", 50, 125);

// Example of displaying portfolio and order history
portfolio.displayPortfolio();
portfolio.displayOrderHistory();