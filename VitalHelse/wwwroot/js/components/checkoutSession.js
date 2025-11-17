document.addEventListener("DOMContentLoaded", function () {

    const selectAddressForm = document.getElementById("select-address-form");

    if (selectAddressForm) {
        selectAddressForm.addEventListener("submit", function (e) {
            e.preventDefault();

            const selected = selectAddressForm.querySelector("input[name='SelectedAddressId']:checked");
            
            // If none are selected
            if (!selected) {
                alert("Du må velge en adresse før du går videre.");
                return;
            }

            const selectedId = selected.value;

            // Store the address in sessionStorage
            sessionStorage.setItem("checkoutSelectedAddressId", selectedId);

            // Testing
            console.log("valgt id:", sessionStorage.getItem("checkoutSelectedAddressId"));
            
            // Redirect the user
            //window.location.href = "/Checkout/Shipping";
        });
    }
});
