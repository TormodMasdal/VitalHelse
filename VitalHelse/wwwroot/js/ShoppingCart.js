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

    // Removes the product from the page instead of reloading the entire page
    document.getElementById(`row-${productId}`).remove();

    const tbody = document.querySelector('#shopping-cart tbody');
    if (!tbody || tbody.rows.length === 0) {
        // If there is no items left, reload the entire page
        window.location.reload();
    }
}

async function AddToCart(productId){
    
    // Saves antiforgerytoken to protect against CSRF-attacks
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Fetch the delete function from the controller
   const response = await fetch(`/ShoppingCart/AddToCart/?id=${productId}`, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': token
        }
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


async function AddQuantity(productId, unitPrice) {
    const quantityElement = document.getElementById(`quantity-${productId}`);
    const totalElement = document.getElementById(`total-${productId}`);

    // Save old values in case of rollback, parse to int because we will treat it as a number
    const oldQuantity = parseInt(quantityElement.innerText);
    const oldTotal = totalElement.innerText;

    // Optimistic update, to make website more responsive
    const newQuantity = oldQuantity + 1;
    quantityElement.innerText = newQuantity.toString();
    totalElement.innerText = (newQuantity * unitPrice).toString() + " kr";

    // Saves the Anti forgery token
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Fetch the response in case of error
    const response = await fetch(`/ShoppingCart/AddQuantity/?id=${productId}`, {
        method: 'PATCH',
        headers: { 'RequestVerificationToken': token }
    });

    // If the response is an error
    if (!response.ok) {
        // Rollback to the cart values before the error occured
        quantityElement.innerText = oldQuantity.toString();
        totalElement.innerText = oldTotal;

        // Prints error message
        const msg = await response.text();
        document.getElementById("error-box").innerText = msg;
        setTimeout(() => document.getElementById("error-box").innerText = "", 3000);
    }
}


async function DecreaseQuantity(productId, unitPrice) {

    const quantityElement = document.getElementById(`quantity-${productId}`);
    const totalElement = document.getElementById(`total-${productId}`);

    // Save old values in case of rollback, parse to int because we will treat it as a number
    const oldQuantity = parseInt(quantityElement.innerText);
    const oldTotal = totalElement.innerText;
    
    // Optimistic update, to make website more responsive
    const newQuantity = oldQuantity - 1;
    quantityElement.innerText = newQuantity.toString();
    totalElement.innerText = (newQuantity * unitPrice).toString() + " kr";
    
    // Saves antiforgerytoken to protect against CSRF-attacks
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Saves the response from the decreaseQuantity function, so that we may print out error message if they try to decrease more than allowed
    const response = await fetch(`/ShoppingCart/DecreaseQuantity/?id=${productId}`, {
        method: 'PATCH',
        headers: { 'RequestVerificationToken': token }
    });

    // Prints the error message if bad request is returned, and keeps it there for 3 seconds
    if (!response.ok) {
        
        // Rollback to the cart values before the error occured
        quantityElement.innerText = oldQuantity.toString();
        totalElement.innerText = oldTotal;
        
        // Prints error message
        const msg = await response.text();
        document.getElementById("error-box").innerText = msg;
        setTimeout(() => document.getElementById("error-box").innerText = "", 3000);
    } 
}


