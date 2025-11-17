// This is your test publishable API key.
const stripe = Stripe("pk_test_51SChPUC3eESM2GvNExbkerY8b3hhTHtCshe5iLw221DEAWWuLg3uqUS6y3J7gbCLz9VLhUKXpOSB8XigMg6n2RsO00LaXHEigo");

initialize();

// Create a Checkout Session
async function initialize() {
    const selectedAddressId = sessionStorage.getItem("selectedAddressId");
    const fetchClientSecret = async () => {
        const response = await fetch("/create-checkout-session", {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                selectedAddressId: selectedAddressId ? parseInt(selectedAddressId, 10) : null
            })
        });
        const { clientSecret } = await response.json();
        return clientSecret;
    };

    const checkout = await stripe.initEmbeddedCheckout({
        fetchClientSecret,
    });

    // Mount Checkout
    checkout.mount('#checkout');
}