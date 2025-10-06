using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public class MiniAppController : ControllerBase
    {
        [HttpGet("/miniapp")]
        public IActionResult ServeMiniApp()
        {
            var htmlContent = @"
<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Каталог трубной продукции</title>
    <script src=""https://telegram.org/js/telegram-web-app.js""></script>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
            background: var(--tg-theme-bg-color, #ffffff);
            color: var(--tg-theme-text-color, #000000);
            padding: 20px;
            line-height: 1.6;
        }
        .container {
            max-width: 100%;
        }
        .header {
            text-align: center;
            margin-bottom: 30px;
            padding: 20px;
            background: var(--tg-theme-secondary-bg-color, #f4f4f5);
            border-radius: 16px;
        }
        .product-list {
            display: flex;
            flex-direction: column;
            gap: 15px;
            margin-bottom: 80px;
        }
        .product-card {
            background: var(--tg-theme-secondary-bg-color, #f4f4f5);
            padding: 20px;
            border-radius: 12px;
            border: 1px solid var(--tg-theme-section-border-color, #e5e5e5);
        }
        .product-name {
            font-weight: 600;
            font-size: 18px;
            margin-bottom: 10px;
        }
        .product-details {
            font-size: 14px;
            color: var(--tg-theme-hint-color, #666);
            margin-bottom: 10px;
        }
        .product-price {
            font-weight: 600;
            color: var(--tg-theme-button-color, #2481cc);
            margin-bottom: 15px;
        }
        .add-to-cart {
            width: 100%;
            padding: 12px;
            border: none;
            border-radius: 8px;
            background: var(--tg-theme-button-color, #2481cc);
            color: var(--tg-theme-button-text-color, #ffffff);
            font-weight: 600;
            cursor: pointer;
        }
        .cart {
            position: fixed;
            bottom: 0;
            left: 0;
            right: 0;
            background: var(--tg-theme-bg-color, #fff);
            border-top: 1px solid var(--tg-theme-section-border-color, #e5e5e5);
            padding: 16px;
            box-shadow: 0 -2px 10px rgba(0,0,0,0.1);
        }
        .cart-total {
            font-weight: 600;
            font-size: 18px;
            margin-bottom: 12px;
            text-align: center;
        }
        .checkout-btn {
            width: 100%;
            padding: 16px;
            border: none;
            border-radius: 12px;
            background: var(--tg-theme-button-color, #2481cc);
            color: var(--tg-theme-button-text-color, #ffffff);
            font-weight: 600;
            font-size: 16px;
            cursor: pointer;
        }
        .loading {
            text-align: center;
            padding: 40px;
            color: var(--tg-theme-hint-color, #666);
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🏭 Каталог труб</h1>
            <p>Трубная металлургическая компания</p>
        </div>

        <div id=""productsContainer"" class=""product-list"">
            <div class=""loading"">
                <h3>Загрузка каталога...</h3>
                <p>Пожалуйста, подождите</p>
            </div>
        </div>
    </div>

    <div class=""cart"">
        <div class=""cart-total"" id=""cartTotal"">Корзина пуста</div>
        <button class=""checkout-btn"" onclick=""checkout()"" disabled id=""checkoutBtn"">
            Оформить заказ
        </button>
    </div>

    <script>
        const tg = window.Telegram.WebApp;
        
        // Инициализация Telegram Mini App
        tg.expand();
        tg.ready();
        
        let products = [];
        let cart = [];
        
        // Получаем данные пользователя
        const user = tg.initDataUnsafe?.user;
        
        // Загружаем продукты
        async function loadProducts() {
            try {
                const response = await fetch('/api/products');
                products = await response.json();
                renderProducts();
            } catch (error) {
                document.getElementById('productsContainer').innerHTML = 
                    '<div class=""loading"">Ошибка загрузки каталога</div>';
            }
        }
        
        function renderProducts() {
            const container = document.getElementById('productsContainer');
            
            if (products.length === 0) {
                container.innerHTML = '<div class=""loading"">Товары не найдены</div>';
                return;
            }
            
            container.innerHTML = products.map(product => `
                <div class=""product-card"">
                    <div class=""product-name"">${product.name}</div>
                    <div class=""product-details"">
                        📍 ${product.warehouse}<br>
                        📏 Диаметр: ${product.diameter}мм<br>
                        🛡️ Стенка: ${product.wallThickness}мм<br>
                        📋 ${product.standard}<br>
                        ⚙️ ${product.steelGrade}<br>
                        📦 Остаток: ${product.stockMeters.toFixed(2)} м
                    </div>
                    <div class=""product-price"">
                        💰 ${product.pricePerMeter} руб/м | ${product.pricePerTon} руб/т
                    </div>
                    <button class=""add-to-cart"" onclick=""addToCart(${product.id})"">
                        Добавить в корзину
                    </button>
                </div>
            `).join('');
        }
        
        function addToCart(productId) {
            const product = products.find(p => p.id === productId);
            if (!product) return;
            
            cart.push(product);
            updateCartDisplay();
            
            tg.showPopup({
                title: 'Успешно',
                message: 'Товар добавлен в корзину'
            });
        }
        
        function updateCartDisplay() {
            const total = cart.reduce((sum, item) => sum + item.pricePerMeter, 0);
            const cartTotalElement = document.getElementById('cartTotal');
            const checkoutBtn = document.getElementById('checkoutBtn');
            
            if (cart.length > 0) {
                cartTotalElement.textContent = `Итого: ${total.toFixed(2)} руб. (${cart.length} товаров)`;
                checkoutBtn.disabled = false;
            } else {
                cartTotalElement.textContent = 'Корзина пуста';
                checkoutBtn.disabled = true;
            }
        }
        
        function checkout() {
            const orderData = {
                telegramUserId: user?.id || 0,
                items: cart.map(item => ({
                    productId: item.id,
                    product: item,
                    quantity: 1,
                    isInMeters: true,
                    finalPrice: item.pricePerMeter
                })),
                customerInfo: {
                    firstName: user?.first_name || 'Пользователь',
                    lastName: user?.last_name || '',
                    inn: '',
                    phone: '',
                    email: ''
                }
            };
            
            tg.showPopup({
                title: 'Заказ оформлен',
                message: `Заказ на ${cart.length} товаров отправлен в обработку!`
            });
            
            // Очищаем корзину
            cart = [];
            updateCartDisplay();
        }
        
        // Загружаем продукты при старте
        loadProducts();
        
        // Показываем информацию о пользователе
        if (user) {
            console.log('User:', user);
        }
    </script>
</body>
</html>";

            return Content(htmlContent, "text/html");
        }

        [HttpGet("/")]
        public IActionResult RedirectToMiniApp()
        {
            return Redirect("/miniapp");
        }
    }
}