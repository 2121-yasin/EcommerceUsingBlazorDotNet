  // function saveToLocalStorage(imageURL, prodName, price) {
  //       var product = {
  //           imageURL: imageURL,
  //           prodName: prodName,
  //           price: price
  //       };
  //       var productJson = JSON.stringify(product);
  //       localStorage.setItem('product', productJson);
  //   }
  function saveToLocalStorage(productId, imageURL, prodName, price) {
    var product = {
      id: productId,
      imageURL: imageURL,
      prodName: prodName,
      price: price
    };
    var productsJson = localStorage.getItem('products');
    var products = productsJson ? JSON.parse(productsJson) : [];
    products.push(product);
    localStorage.setItem('products', JSON.stringify(products));
  }
  

// $(document).ready(function () {
//   var clicked = localStorage.getItem('clicked') === 'true' || false;

//   function setMode() {
//       if (clicked == true) {
//           $('body').attr('data-bs-theme', 'dark');
//           $('#mode i').attr('class', 'bi bi-brightness-high');
//           $('.menu').attr('class', 'menu bi bi-list text-light')
//       } else {
//           $('body').attr('data-bs-theme', 'light');
//           $('#mode i').attr('class', 'bi bi-moon-stars')
//           $('.menu').attr('class', 'menu bi bi-list')

//       }
//   }
//   setMode();

//   $('.toggle').click(function () {
//       clicked = !clicked;
//       setMode();
//       localStorage.setItem('clicked', clicked);
//   });
// });

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
    // Product already exists in cart, ask for confirmation before adding again
    if (window.confirm("This item is already in your cart. Are you sure you want to add it again?")) {
      cartItem.quantity += 1;
      cartItem.totalPrice = (parseFloat(cartItem.totalPrice) + parseFloat(cartItem.price)).toFixed(2);
    } else {
      return;
    }
  } else {
    // Product doesn't exist in cart, add as new item with initial total price
    cartItem = { id: productId, name: productName, price: productPrice, image: productImage, quantity: 1, totalPrice: productPrice };
    cartItems.push(cartItem);
    // Increment the counter and store it in local storage
  // Increment the counter and store it in local storage
  let cartCounter = parseInt(localStorage.getItem("cartCounter")) || 0;
  cartCounter += 1;
  localStorage.setItem("cartCounter", cartCounter.toString());

  // Update the cart counter element on the page
  const cartCounterElement = document.getElementById("cartCounter");
  if (cartCounterElement) {
    cartCounterElement.innerText = cartCounter.toString();
  }

  }
  



  localStorage.setItem("cartItems", JSON.stringify(cartItems));
  // Display alert box
  window.alert("Item added to cart");
}



//add to wishlist
function addToWishlist(event) {
  event.preventDefault();
  const productId = event.target.dataset.productId;
  const productName = event.target.dataset.productName;
  const productPrice = event.target.dataset.productPrice;
  const productImage = event.target.dataset.productImage;

  // Check if all required data is present
  if (!productId || !productName || !productPrice || !productImage) {
    console.error("Unable to add item to wishlist - missing data");
    return;
  }

  let wishlistItems = JSON.parse(sessionStorage.getItem("wishlistItems")) || [];
  let wishlistItem = wishlistItems.find(item => item.id === productId);
  if (wishlistItem) {
    // Product already exists in wishlist
    window.alert("Product already present in wishlist");
    return;
  } else {
    // Product doesn't exist in wishlist, add as new item
    wishlistItem = { id: productId, name: productName, price: productPrice, image: productImage };
    wishlistItems.push(wishlistItem);
    sessionStorage.setItem("wishlistItems", JSON.stringify(wishlistItems));
    window.alert("Item added to wishlist");
  }
}



function DecodeJwtToken(token) {
  var base64Url = token.split('.')[1];
  var base64 = base64Url.replace('-', '+').replace('_', '/');
  return JSON.parse(window.atob(base64));
}

window.RazorpayCheckout = {
  createPayment: function(orderId, keyId, totalAmount) {
      return new Promise((resolve, reject) => {
          var options = {
              "key": keyId,
              "amount": totalAmount * 100,
              "currency": "INR",
              "name": "E Shop",
              "description": "Payment for your order",
              "image": "../../../Assets/eshop.png",
              "order_id": orderId,
              "handler": function(response) {
                // Call RazorpayPaymentSuccessHandler method on payment success
                // DotNet.invokeMethodAsync('Ecommerce', 'RazorpayPaymentSuccessHandler', response.razorpay_payment_id);
                  
                var paymentDetails = {
                  paymentId: response.razorpay_payment_id,
                  // status: response.razorpay_payment_status,
                  // paymentMethod: response.razorpay_payment_method,
                  createdAt: new Date().toISOString(),
                  description: options.description,
                  customer: options.prefill.name,
                  email: options.prefill.email,
                  contact: options.prefill.contact,
                  street: options.prefill.street,
                  city: options.prefill.city,
                  state:options.prefill.state,
                  pincode:options.prefill.pincode,
                  totalFee: (options.amount / 100).toFixed(2),
                  orderId: options.order_id,
                  notes: JSON.stringify(options.notes)
                };
                
                //resolve(`alert('Payment successful.\\nPayment ID: ${paymentDetails.paymentId}\\nStatus: ${paymentDetails.status}\\nPayment Method: ${paymentDetails.paymentMethod}\\nCreated At: ${paymentDetails.createdAt}\\nDescription: ${paymentDetails.description}\\nCustomer: ${paymentDetails.customer}\\nEmail: ${paymentDetails.email}\\nContact: ${paymentDetails.contact}\\nStreet: ${paymentDetails.street}\\nCity: ${paymentDetails.city}\\nState: ${paymentDetails.state}\\nPincode: ${paymentDetails.pincode}\\nOrder ID: ${paymentDetails.orderId}\\nNotes: ${paymentDetails.notes}\\nTotal Fee Paid: ${paymentDetails.totalFee}')`);
                //localStorage.setItem('paymentDetails', JSON.stringify(paymentDetails));
                 // Store paymentDetails in session storage
                 sessionStorage.setItem('paymentDetails', JSON.stringify(paymentDetails));
                 resolve(`alert('Payment successful.\\nPayment ID: ${paymentDetails.paymentId}\\nCreated At: ${paymentDetails.createdAt}\\nDescription: ${paymentDetails.description}\\nName: ${paymentDetails.customer}\\nEmail: ${paymentDetails.email}\\nContact: ${paymentDetails.contact}\\nStreet: ${paymentDetails.street}\\nCity: ${paymentDetails.city}\\nState: ${paymentDetails.state}\\nPincode: ${paymentDetails.pincode}\\nOrder ID: ${paymentDetails.orderId}\\nNotes: ${paymentDetails.notes}\\nTotal Fee Paid: ${paymentDetails.totalFee}')`);
                
              },
              "prefill": {
                  "name": "",
                  "email": "",
                  "contact": "",
                  "street": "",
                  "city": "",
                  "state": "",
                  "pincode": ""
              },
              "notes": {
                  "address": "Razorpay Corporate Office"
              },
              "theme": {
                  "color": "#F37254"
              }
          };
          
          var token = localStorage.getItem('token');
          var decodedToken = DecodeJwtToken(token);
          
          options.prefill.name = decodedToken.UserName;
          options.prefill.email = decodedToken.Email;
          options.prefill.contact = decodedToken.Phone;

          options.prefill.street = decodedToken.Street;
          options.prefill.city = decodedToken.City;
          options.prefill.state = decodedToken.State;
          options.prefill.pincode = decodedToken.Pincode;
          
          var rzp = new Razorpay(options);
          rzp.open();
      });
  }
};

function updateAmountToPay(amountToPayElement, totalAmount) {
  amountToPayElement.textContent = "Amount to Pay: " + totalAmount;
}
// function DecodeJwtToken(token) {
//   var base64Url = token.split('.')[1];
//   var base64 = base64Url.replace('-', '+').replace('_', '/');
//   return JSON.parse(window.atob(base64));
// }

// window.RazorpayCheckout = {
//   createPayment: function(orderId, keyId, totalAmount) {
//       return new Promise((resolve, reject) => {
//           var options = {
//               "key": keyId,
//               "amount": totalAmount * 100,
//               "currency": "INR",
//               "name": "E Shop",
//               "description": "Payment for your order",
//               "image": "../../../Assets/eshop.jpg",
//               "order_id": orderId,
//               "handler": function(response) {
//                 //  /// Call RazorpayPaymentSuccessHandler method on payment success
//                 //    DotNet.invokeMethodAsync('Ecommerce', 'RazorpayPaymentSuccessHandler', response.razorpay_payment_id);
                  
//                   resolve(`alert('Payment successful. Payment ID: ${response.razorpay_payment_id}')`);
//               },
//               "prefill": {
//                   "name": "",
//                   "email": "",
//                   "contact": "",
//                   "street": "",
//                   "city": "",
//                   "state": "",
//                   "pincode": ""
//               },
//               "notes": {
//                   "address": "Razorpay Corporate Office"
//               },
//               "theme": {
//                   "color": "#F37254"
//               },
//               "webhook": "/webhookhandler"
//           };
          
//           var token = localStorage.getItem('token');
//           var decodedToken = DecodeJwtToken(token);
          
//           options.prefill.name = decodedToken.UserName;
//           options.prefill.email = decodedToken.Email;
//           options.prefill.contact = decodedToken.Phone;

//           options.prefill.street = decodedToken.Street;
//           options.prefill.city = decodedToken.City;
//           options.prefill.state = decodedToken.State;
//           options.prefill.pincode = decodedToken.Pincode;
          
//           var rzp = new Razorpay(options);
//           rzp.open();
//       });
//   }
// };


//redirect with token to details page


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
