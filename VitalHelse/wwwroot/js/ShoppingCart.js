async function removItemShoppingCart(productId) {
    
    // Calls the delete function from the controller, await before the reload page so we don't get race condition
    await fetch(`/ShoppingCart/Delete?id=${productId}`, { method: 'DELETE' })

    // Reloads the page
    window.location.reload();
}

async function AddQuantity(productId) {
    await fetch(`/ShoppingCart/AddQuantity/?id=${productId}`, { method: 'PATCH' })
    
    window.location.reload();
}

async function DecreaseQuantity(productId) {
    const response = await fetch(`/ShoppingCart/DecreaseQuantity/?id=${productId}`, { method: 'PATCH' })

    if (!response.ok) {
        const msg = await response.text();
        document.getElementById("error-box").innerText = msg;
        setTimeout(() => document.getElementById("error-box").innerText = "", 3000);
    }
    else {
        window.location.reload(); // reload for å se endret quantity
    }
}

