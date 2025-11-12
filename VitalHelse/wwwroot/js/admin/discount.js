async function saveDiscount(id, isCategory) {
    const rate = document.getElementById(`discount-${id}`).value;
    const response = await fetch('/AdminDiscount/UpdateDiscount', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `id=${id}&rate=${rate}&isCategory=${isCategory}`
    });

    if (response.ok) {
        alert('Rabatt lagret!');
        location.reload();
    } else {
        alert('Noe gikk galt.');
    }
}
