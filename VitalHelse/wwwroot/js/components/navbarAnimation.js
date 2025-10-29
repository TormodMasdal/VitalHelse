
window.addEventListener("scroll", function() {
    // Hente element etter id
    const navbar = document.getElementById("navbar");
    // Sjekke om brukeren har skrollet mer en 30 piksler 
    if (window.scrollY > 30) {
        //Hvis ja legg til en klasse
        navbar.classList.add("shrink");
    } 
    else {
        // Hvis ikke fjern klassen
        navbar.classList.remove("shrink");
    }
});