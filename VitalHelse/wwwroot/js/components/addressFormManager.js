// Run the function when HTML is loaded
document.addEventListener("DOMContentLoaded", function () {

    // Get and save all the necessary elemnts 
    const hasAddresses = document.getElementById("address-page").dataset.hasAddresses === "true";
    const link = document.getElementById("show-form-link");
    const cancelBtn = document.getElementById("cancel-add-address");
    const formWrap = document.getElementById("add-address-form-wrapper");
    const listWrap = document.getElementById("address-list-wrapper");
    const noAddressBox = document.getElementById("no-address-box");

    // Function to only show the add address form (hides everything else)
    function showFormOnly() {
        if (listWrap) listWrap.style.display = "none";
        if (noAddressBox) noAddressBox.style.display = "none";
        formWrap.style.display = "block";
    }

    // Function that changes view with the list if there are addresses or only to text for adding address
    function showListOrEmpty() {
        // Hide add address form
        formWrap.style.display = "none";
        if (hasAddresses) {
            if (listWrap) listWrap.style.display = "block";
        } else {
            if (noAddressBox) noAddressBox.style.display = "block";
        }
    }
    
    // If the achor to add address is found
    if (link) {
        // When clicked
        link.addEventListener("click", function (e) {
            // Prevent the link from navigating
            e.preventDefault();
            
            showFormOnly();
        });
    }
    
    // If the cancel button on add form is found
    if (cancelBtn) {
        // When clicked
        cancelBtn.addEventListener("click", function () {
            // Save the form element
            const form = document.getElementById("add-address-form");
            // If the element is found
            if (form) {
                // Reset the form
                form.reset();
            }
            showListOrEmpty();
        });
    }
});