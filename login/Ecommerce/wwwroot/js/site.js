function toggleDarkMode() {
    document.documentElement.classList.toggle('dark');
}

function addToCart(event) {
  event.preventDefault();
  const productId = event.target.dataset.productId;
  const productName = event.target.dataset.productName;
  const productPrice = event.target.dataset.productPrice;
  const productImage = event.target.dataset.productImage;

  // Check if all required data is present
  if (!productId || !productName || !productPrice || !productImage) {
    console.error("Unable to add item to cart - missing data");
    return;
  }

  let cartItems = JSON.parse(localStorage.getItem("cartItems")) || [];
  let cartItem = cartItems.find(item => item.id === productId);
  if (cartItem) {
    // Product already exists in cart, increment quantity and update total price
    cartItem.quantity += 1;
    cartItem.totalPrice = (parseFloat(cartItem.totalPrice) + parseFloat(cartItem.price)).toFixed(2);
  } else {
    // Product doesn't exist in cart, add as new item with initial total price
    cartItem = { id: productId, name: productName, price: productPrice, image: productImage, quantity: 1, totalPrice: productPrice };
    cartItems.push(cartItem);
  }
  localStorage.setItem("cartItems", JSON.stringify(cartItems));
   // Display alert box
   window.alert("Item added to cart");
}




// function addToCart(product) {
//   let cart = JSON.parse(localStorage.getItem("cart")) || {};
//   let productId = product.ProdId;
//   if (cart[productId]) {
//     cart[productId].quantity += 1;
//   } else {
//     cart[productId] = {
//       productId: product.ProdId,
//       name: product.Name,
//       price: product.Price,
//       quantity: 1
//     };
//   }
//   localStorage.setItem("cart", JSON.stringify(cart));
// }


// function addToCart(event) {
//   event.preventDefault();

//   const prodId = event.target.getAttribute("data-product-id");
//   const prodImage = event.target.getAttribute("data-product-image");
//   const prodName = event.target.getAttribute("data-product-name");
//   const prodPrice = event.target.getAttribute("data-product-price");

//   let cartItems = sessionStorage.getItem("cartItems");

//   if (cartItems) {
//     cartItems = JSON.parse(cartItems);

//     if (cartItems[prodId]) {
//       cartItems[prodId].quantity += 1;
//       sessionStorage.setItem("cartItems", JSON.stringify(cartItems));
//       alert("Item quantity updated in cart!");
//       return;
//     }

//     cartItems[prodId] = {
//       id: prodId,
//       image: prodImage,
//       name: prodName,
//       price: prodPrice,
//       quantity: 1,
//     };

//     sessionStorage.setItem("cartItems", JSON.stringify(cartItems));
//     alert("Item added to cart!");

//   } else {
//     const newCartItem = {
//       [prodId]: {
//         id: prodId,
//         image: prodImage,
//         name: prodName,
//         price: prodPrice,
//         quantity: 1,
//       },
//     };

//     sessionStorage.setItem("cartItems", JSON.stringify(newCartItem));
//     alert("Item added to cart!");
//   }
// }
