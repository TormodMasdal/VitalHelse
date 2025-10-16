let postalCodeData = [];

// Last JSON-fil
fetch("/js/postdata.json")
    .then(res => res.json())
    .then(data => {
        postalCodeData = data;
    })
    .catch(err => console.log(err));

function updateArea() {
    const postalCodeInput = document.getElementById('postnummer');
    const areaInput = document.getElementById('poststed');
    const postalCode = postalCodeInput.value.trim();

    // Finn objektet med riktig postnummer
    const match = postalCodeData.find(item => item.postnummer === postalCode);

    if (match) {
        areaInput.value = match.poststed;
    } else {
        areaInput.value = "";
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const postalCodeInput = document.getElementById('postnummer');
    postalCodeInput.addEventListener('input', updateArea);
});
