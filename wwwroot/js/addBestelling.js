function addToCart({ ProductId, Aantal, Opmerking }) {
    fetch('/api/BestellingApi/AddToCart', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            ProductId: parseInt(ProductId, 10),
            Aantal: parseInt(Aantal, 10),
            Opmerking: (Opmerking ?? '').trim()
        })
    })
        .then(response => response.json())
        .then(data => {
            document.getElementById('CartItemsJson').value = JSON.stringify(data.cartItems);
            updateCartTable(data.cartItems, data.totaalBedrag);
        });
}

function removeFromCart(productId, opmerking) {
    fetch('/api/BestellingApi/RemoveFromCart', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            ProductId: parseInt(productId, 10),
            Opmerking: (opmerking ?? '').trim()
        })
    })
        .then(response => response.json())
        .then(data => {
            document.getElementById('CartItemsJson').value = JSON.stringify(data.cartItems);
            updateCartTable(data.cartItems, data.totaalBedrag);
        });
}

function updateCartTable(cartItems, totaalBedrag) {
    const tbody = document.getElementById('cartTableBody');
    tbody.innerHTML = '';
    if (!cartItems || cartItems.length === 0) {
        tbody.innerHTML = '<tr><td colspan="6" class="text-muted">Uw winkelmandje is leeg.</td></tr>';
    } else {
        cartItems.forEach(item => {
            // Escape single quotes in opmerking for JS string
            const normalizedOpmerking = (item.opmerking ?? '').replace(/'/g, "\\'");
            const truncatedOpmerking = truncateOpmerking(item.opmerking);
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.naam}</td>
                <td>${item.aantal}</td>
                <td>${item.prijs.toLocaleString('nl-BE', { style: 'currency', currency: 'EUR' })}</td>
                <td>${(item.aantal * item.prijs).toLocaleString('nl-BE', { style: 'currency', currency: 'EUR' })}</td>
                <td>${truncatedOpmerking ? truncatedOpmerking : ''}</td>
                <td>
                    <button type="button" onclick="removeFromCart(${parseInt(item.productId, 10)}, '${normalizedOpmerking}')" title="Verwijder"
                        style="background: none; border: none; padding: 0; cursor: pointer;">
                        <i class="bi bi-trash text-danger"></i>
                    </button>
                </td>
            `;
            tbody.appendChild(row);
        });
    }
    document.getElementById('cartTotal').innerText = 'Totaalbedrag: ' +
        (typeof totaalBedrag === 'number'
            ? totaalBedrag.toLocaleString('nl-BE', { style: 'currency', currency: 'EUR' })
            : '€ 0,00');
}

// Modal functionality
var modalProductId;
function openAddToCartModal(productId) {
    modalProductId = parseInt(productId, 10);
    const product = findProductById(modalProductId);
    if (product) {
        document.getElementById('modalProductId').value = product.Id;
    }
    var myModal = new bootstrap.Modal(document.getElementById('addToCartModal'));
    myModal.show();
}

document.getElementById('modalAddBtn').addEventListener('click', function () {
    const form = document.getElementById('addToCartForm');
    if (form.checkValidity()) {
        const formData = new FormData(form);
        const data = {
            ProductId: parseInt(formData.get('ProductId'), 10),
            Aantal: parseInt(formData.get('Aantal'), 10),
            Opmerking: formData.get('Opmerking') || null
        };
        addToCart(data);
        const myModalEl = document.getElementById('addToCartModal');
        const modal = bootstrap.Modal.getInstance(myModalEl);
        modal.hide();

        // Reset modal fields to default values
        document.getElementById('modalProductId').value = '';
        document.getElementById('modalAantal').value = 1;
        document.getElementById('modalOpmerking').value = '';
    } else {
        form.reportValidity();
    }
});

function findProductById(productId) {
    productId = parseInt(productId, 10);
    const tabContent = document.getElementById('menuTabContent');
    for (const tabPane of tabContent.querySelectorAll('.tab-pane')) {
        const product = Array.from(tabPane.querySelectorAll('.list-group-item')).find(item => {
            const btn = item.querySelector('button');
            return btn && btn.getAttribute('onclick') && btn.getAttribute('onclick').includes(`openAddToCartModal(${productId})`);
        });
        if (product) {
            const naam = product.querySelector('strong').innerText;
            const beschrijving = product.querySelector('small').innerText.replace('Prijs: ', '');
            return {
                Id: productId,
                Naam: naam,
                Beschrijving: beschrijving,
                Prijs: parseFloat(beschrijving.replace('€', '').replace(',', '.'))
            };
        }
    }
    return null;
}

function truncateOpmerking(opmerking) {
    if (!opmerking) return "";
    return opmerking.length > 10 ? opmerking.substring(0, 9) + "..." : opmerking;
}

// Clear the cart when the create page loads (after bestelling confirmation)
document.addEventListener('DOMContentLoaded', function () {
    // Reset the hidden field
    const cartInput = document.getElementById('CartItemsJson');
    if (cartInput) {
        cartInput.value = '[]';
    }

    // clear the cart table visually
    const tbody = document.getElementById('cartTableBody');
    if (tbody) {
        tbody.innerHTML = '<tr><td colspan="6" class="text-muted">Uw winkelmandje is leeg.</td></tr>';
    }
    const cartTotal = document.getElementById('cartTotal');
    if (cartTotal) {
        cartTotal.innerText = 'Totaalbedrag: € 0,00';
    }
});