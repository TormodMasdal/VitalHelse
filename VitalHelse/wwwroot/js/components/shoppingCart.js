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

// Helper that formats numers as norwegian currency (1000 as 1000,00 kr)
const nok = new Intl.NumberFormat('no-NO', { style: 'currency', currency: 'NOK' });

async function AddQuantity(productId, unitPrice) {
    const quantityElement = document.getElementById(`quantity-${productId}`);
    const totalElement = document.getElementById(`total-${productId}`);

    // Save old values in case of rollback, parse to int because we will treat it as a number
    const oldQuantity = parseInt(quantityElement.value, 10) || 0; // || 0 means if the field is not a number(empty or error) then use 0
    const oldTotal = totalElement.innerText;

    // Add quantity
    const newQuantity = oldQuantity + 1;
    
    // Display the new value
    quantityElement.value = String(newQuantity);
    totalElement.innerText = nok.format(newQuantity * unitPrice);

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
        quantityElement.value = String(oldQuantity);
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
    const oldQuantity = parseInt(quantityElement.value, 10) || 0;
    const oldTotal = totalElement.innerText;
    
    // Decrease quantity
    const newQuantity = oldQuantity - 1;
    
    // If quatity is 0 or less remove the item
    if (newQuantity <= 0) {
        document.getElementById(`row-${productId}`).remove();
    }
    else {
        quantityElement.value = String(newQuantity);
        totalElement.innerText = nok.format(newQuantity * unitPrice);
    }
    
    
    // Saves antiforgerytoken to protect against CSRF-attacks
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Saves the response from the decreaseQuantity function, so that we may print out error message if they try to decrease more than allowed
    const response = await fetch(`/ShoppingCart/DecreaseQuantity/?id=${productId}`, {
        method: 'PATCH',
        headers: { 'RequestVerificationToken': token }
    });

    // Prints the error message if bad request is returned, and keeps it there for 3 seconds
    if (!response.ok) {
        if (newQuantity <= 0) {
            window.location.reload();
        }
        else {
            // Rollback to the cart values before the error occured
            quantityElement.value = String(oldQuantity);
            totalElement.innerText = oldTotal;
        }
        
        // Prints error message
        const msg = await response.text();
        document.getElementById("error-box").innerText = msg;
        setTimeout(() => document.getElementById("error-box").innerText = "", 3000);
    } 
}


async function UpdateQuantity(productId, unitPrice) {
    
    // Get the inputfield for quantity and the total price element
    const quantityElement = document.getElementById(`quantity-${productId}`);
    const totalElement = document.getElementById(`total-${productId}`);
    
    // Parse inputvalue to int
    const newQuantity = parseInt(quantityElement.value, 10);

    // If the user inputs something other that a number
    if (isNaN(newQuantity) || newQuantity <= 0) {
        // Set the value to 1
        quantityElement.value = 1;
        return;
    }

    // Update the total price
    totalElement.innerText = nok.format(newQuantity * unitPrice);

    // Saves antiforgerytoken to protect against CSRF-attacks
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Send the new quantity
    const response = await fetch(`/ShoppingCart/SetQuantity/?id=${productId}&quantity=${newQuantity}`, {
        method: 'PATCH',
        headers: { 'RequestVerificationToken': token }
    });

    // If an error occours
    if (!response.ok) {
        const msg = await response.text();
        document.getElementById("error-box").innerText = msg;
        setTimeout(() => document.getElementById("error-box").innerText = "", 3000);
    } else {
        // If the server adjusted the quantity, update it in the input box
        const data = await response.json();
        quantityElement.value = data.correctedQuantity;
        totalElement.innerText = nok.format(data.correctedQuantity * unitPrice);
    }
}