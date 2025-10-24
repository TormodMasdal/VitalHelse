async function removItemShoppingCart(productId) {
    
    // Saves antiforgerytoken to protect against CSRF-attacks
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Fetch the delete function from the controller
    await fetch(`/ShoppingCart/Delete?id=${productId}`, {
        method: 'DELETE',
        headers: {
            'RequestVerificationToken': token
        }
    });

    // Reloads the page so it is updated
    window.location.reload();
}


async function AddQuantity(productId) {
    
    // Saves antiforgerytoken to protect against CSRF-attacks
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Fetch the add quantity function from the controller
    await fetch(`/ShoppingCart/AddQuantity/?id=${productId}`, {
        method: 'PATCH',
        headers: { 'RequestVerificationToken': token }
    });

    // Reloads the page so it is updated
    window.location.reload();
}

async function DecreaseQuantity(productId) {
    // Saves antiforgerytoken to protect against CSRF-attacks
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Saves the response from the decreaseQuantity function, so that we may print out error message if they try to decrease more than allowed
    const response = await fetch(`/ShoppingCart/DecreaseQuantity/?id=${productId}`, {
        method: 'PATCH',
        headers: { 'RequestVerificationToken': token }
    });

    // Prints the error message if bad request is returned, and keeps it there for 3 seconds
    if (!response.ok) {
        const msg = await response.text();
        document.getElementById("error-box").innerText = msg;
        setTimeout(() => document.getElementById("error-box").innerText = "", 3000);
    } else {
        window.location.reload();
    }
}


