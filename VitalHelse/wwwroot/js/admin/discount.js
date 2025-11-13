document.addEventListener("DOMContentLoaded", () => {

    const saveBtn = document.getElementById("saveAllDiscountsBtn");

    // REALTIME PRICE UPDATE
    document.querySelectorAll(".discount-input").forEach(input => {
        input.addEventListener("input", () => {
            const row = input.closest("tr");
            const basePrice = parseFloat(row.dataset.basePrice);

            const catRate = parseFloat(row.querySelector(".js-category-rate")?.value ?? row.dataset.categoryRate);
            const prodRate = parseFloat(row.querySelector(".js-product-rate")?.value ?? row.dataset.productRate);

            const effective = prodRate > 0 ? prodRate : catRate;
            const newPrice = basePrice * (1 - effective / 100);

            row.querySelector(".js-new-price").innerText = newPrice.toFixed(2) + " kr";

            saveBtn.classList.remove("saved");
        });
    });

    // SAVE ALL
    saveBtn.addEventListener("click", async () => {

        const updates = [];

        document.querySelectorAll("tr.listview__row").forEach(row => {
            const prodId = parseInt(row.dataset.productId);
            const catId = parseInt(row.dataset.categoryId);

            const newCat = row.querySelector(".js-category-rate")?.value;
            const newProd = row.querySelector(".js-product-rate")?.value;

            if (catId && newCat !== undefined)
                updates.push({ id: catId, rate: parseFloat(newCat), isCategory: true });

            if (newProd !== undefined)
                updates.push({ id: prodId, rate: parseFloat(newProd), isCategory: false });
        });

        await fetch("/Admin/Discounts/UpdateMany", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(updates)
        });

        saveBtn.classList.add("saved");
        saveBtn.innerText = "✔ Lagret!";
        setTimeout(() => saveBtn.innerText = "💾 Lagre endringer", 1500);
    });
});
