let cart = JSON.parse(localStorage.getItem("cart")) || [];

function renderCart() {
  const cartDiv = document.getElementById("cart");
  cartDiv.innerHTML = "";

  let total = 0;

  cart.forEach(item => {
    total += item.price * item.quantity;
    cartDiv.innerHTML += `
      <div>
        ${item.name} - ${item.quantity} x ${item.price}
        <button onclick="removeItem(${item.id})">Xóa</button>
      </div>
    `;
  });

  document.getElementById("total").innerText = total;
}

function removeItem(id) {
  cart = cart.filter(item => item.id !== id);
  localStorage.setItem("cart", JSON.stringify(cart));
  renderCart();
}

renderCart();
