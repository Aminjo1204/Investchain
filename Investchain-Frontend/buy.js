document.getElementById('trade-form').addEventListener('submit', function(event) {
    event.preventDefault();
    
    const symbol = document.getElementById('symbol').value;
    const action = document.getElementById('action').value;
    const quantity = document.getElementById('quantity').value;

    // Simuliere den Handelsauftrag
    simulateTrade(symbol, action, quantity);
});

function simulateTrade(symbol, action, quantity) {
    const responseDiv = document.getElementById('response');
    const table = document.getElementById('trade-table');
    const currentTime = new Date().toLocaleString();
    const price = "123.45"; // Hier kannst du den tatsächlichen Preis der Aktie einfügen

    // Erzeuge eine neue Zeile für die Tabelle
    const newRow = document.createElement('tr');
    
    // Fülle die Zellen der Zeile mit Daten
    newRow.innerHTML = `
        <td>${action === 'buy' ? 'Kauf' : 'Verkauf'}</td>
        <td>${symbol}</td>
        <td>${currentTime}</td>
        <td>${price}</td>
        <td>${quantity}</td>
    `;
    
    // Füge die neue Zeile zur Tabelle hinzu
    table.appendChild(newRow);

    // Zeige eine Bestätigungsnachricht an
    const message = `Auftrag erhalten: ${action === 'buy' ? 'Kauf' : 'Verkauf'} ${quantity} Anteile von ${symbol}`;
    responseDiv.textContent = message;

    // Aktualisiere das Depot (Simulationscode)
    updatePortfolio(symbol, action, quantity);
}

function updatePortfolio(symbol, action, quantity) {
    // Hier würdest du normalerweise das Depot des Benutzers aktualisieren
    // und den Kontostand entsprechend anpassen.
    // Dies ist jedoch nur eine Simulation.
    console.log(`Depot aktualisiert: ${action === 'buy' ? 'Kauf' : 'Verkauf'} ${quantity} Anteile von ${symbol}`);
}
