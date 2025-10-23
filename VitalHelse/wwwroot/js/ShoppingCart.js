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
    await fetch(`/ShoppingCart/DecreaseQuantity/?id=${productId}`, { method: 'PATCH' })

    window.location.reload();
}

