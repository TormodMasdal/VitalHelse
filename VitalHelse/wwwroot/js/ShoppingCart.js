async function removItemShoppingCart(productId) {
    
    // Calls the delete function from the controller, await before the reload page so we don't get race condition
    await fetch(`/ShoppingCart/Delete?id=${productId}`, { method: 'DELETE' })

    // Reloads the page so we get updated shopping cart
    window.location.reload();
}

async function AddQuantity(productId) {
    await fetch(`/ShoppingCart/AddQuantity/?id=${productId}`, { method: 'PATCH' })
    
    window.location.reload();
}

async function DecreaseQuantity(productId) {
    // Calls the controller function decrease quantity, and svaes the result so we can print error message if quantity allready is a 1
    const response = await fetch(`/ShoppingCart/DecreaseQuantity/?id=${productId}`, { method: 'PATCH' })

    // If error message, print it on the screen for 3 seconds
    if (!response.ok) {
        const msg = await response.text();
        document.getElementById("error-box").innerText = msg;
        setTimeout(() => document.getElementById("error-box").innerText = "", 3000);
    }
    else {
        // If no error reload the page so we get updated shopping cart
        window.location.reload(); // reload for å se endret quantity
    }
}

