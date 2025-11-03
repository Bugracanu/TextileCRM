// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Global Arama Fonksiyonu
$(document).ready(function () {
    const searchInput = $('#globalSearch');
    const searchResults = $('#searchResults');
    const searchContent = $('#searchContent');
    let searchTimeout;

    // Arama kutusuna yazıldığında
    searchInput.on('input', function () {
        const searchTerm = $(this).val().trim();

        // Timeout'u temizle (debounce için)
        clearTimeout(searchTimeout);

        if (searchTerm.length < 2) {
            searchResults.hide();
            return;
        }

        // 500ms bekle ve sonra ara
        searchTimeout = setTimeout(function () {
            performSearch(searchTerm);
        }, 500);
    });

    // Arama kutusuna focus olunca
    searchInput.on('focus', function () {
        const searchTerm = $(this).val().trim();
        if (searchTerm.length >= 2) {
            searchResults.show();
        }
    });

    // Dışarı tıklandığında sonuçları gizle
    $(document).on('click', function (e) {
        if (!$(e.target).closest('#globalSearch, #searchResults').length) {
            searchResults.hide();
        }
    });

    function performSearch(searchTerm) {
        searchContent.html('<div class="text-center p-2"><div class="spinner-border spinner-border-sm text-primary" role="status"></div><p class="mb-0 mt-1" style="font-size: 0.875rem;">Aranıyor...</p></div>');
        searchResults.show();

        $.ajax({
            url: '/Home/GlobalSearch',
            type: 'GET',
            data: { searchTerm: searchTerm },
            success: function (data) {
                displaySearchResults(data, searchTerm);
            },
            error: function () {
                searchContent.html('<div class="text-center text-danger p-2" style="font-size: 0.875rem;"><i class="bi bi-exclamation-triangle"></i><p class="mb-0 mt-1">Hata oluştu</p></div>');
            }
        });
    }

    function displaySearchResults(data, searchTerm) {
        if (!data.customers.length && !data.orders.length && !data.products.length) {
            searchContent.html('<div class="text-center text-muted p-2" style="font-size: 0.875rem;"><i class="bi bi-inbox"></i><p class="mb-0 mt-1">Sonuç bulunamadı</p></div>');
            return;
        }

        let html = '';
        let totalResults = 0;

        // Müşteriler - maksimum 3 sonuç
        if (data.customers.length > 0) {
            html += '<h6 class="dropdown-header"><i class="bi bi-people-fill"></i> Müşteriler</h6>';
            data.customers.slice(0, 3).forEach(function (customer) {
                html += `<a class="dropdown-item" href="/Customer/Details/${customer.id}">
                            <div class="d-flex justify-content-between align-items-center">
                                <div style="flex: 1; min-width: 0;">
                                    <div style="font-weight: 500;">${highlightText(customer.name, searchTerm)}</div>
                                    <small class="text-muted d-block text-truncate">${customer.email || ''}</small>
                                </div>
                                <i class="bi bi-arrow-right-circle text-muted ms-2"></i>
                            </div>
                         </a>`;
                totalResults++;
            });
            if (data.customers.length > 3) {
                html += `<div class="dropdown-item text-center text-muted" style="font-size: 0.75rem;">+${data.customers.length - 3} daha fazla</div>`;
            }
            html += '<div class="dropdown-divider"></div>';
        }

        // Siparişler - maksimum 3 sonuç
        if (data.orders.length > 0) {
            html += '<h6 class="dropdown-header"><i class="bi bi-box-seam"></i> Siparişler</h6>';
            data.orders.slice(0, 3).forEach(function (order) {
                const statusBadge = getOrderStatusBadge(order.status);
                html += `<a class="dropdown-item" href="/Order/Details/${order.id}">
                            <div class="d-flex justify-content-between align-items-center">
                                <div style="flex: 1; min-width: 0;">
                                    <div style="font-weight: 500;">#${order.id} - ${highlightText(order.customerName, searchTerm)}</div>
                                    <small class="d-block">${statusBadge} <span class="text-muted">${order.totalAmount.toFixed(2)} TL</span></small>
                                </div>
                                <i class="bi bi-arrow-right-circle text-muted ms-2"></i>
                            </div>
                         </a>`;
                totalResults++;
            });
            if (data.orders.length > 3) {
                html += `<div class="dropdown-item text-center text-muted" style="font-size: 0.75rem;">+${data.orders.length - 3} daha fazla</div>`;
            }
            html += '<div class="dropdown-divider"></div>';
        }

        // Ürünler - maksimum 3 sonuç
        if (data.products.length > 0) {
            html += '<h6 class="dropdown-header"><i class="bi bi-tag-fill"></i> Ürünler</h6>';
            data.products.slice(0, 3).forEach(function (product) {
                html += `<a class="dropdown-item" href="/Product/Details/${product.id}">
                            <div class="d-flex justify-content-between align-items-center">
                                <div style="flex: 1; min-width: 0;">
                                    <div style="font-weight: 500;">${highlightText(product.name, searchTerm)}</div>
                                    <small class="text-muted d-block">Stok: ${product.stockQuantity} • ${product.unitPrice.toFixed(2)} TL</small>
                                </div>
                                <i class="bi bi-arrow-right-circle text-muted ms-2"></i>
                            </div>
                         </a>`;
                totalResults++;
            });
            if (data.products.length > 3) {
                html += `<div class="dropdown-item text-center text-muted" style="font-size: 0.75rem;">+${data.products.length - 3} daha fazla</div>`;
            }
        }

        searchContent.html(html);
    }

    function highlightText(text, searchTerm) {
        if (!text) return '';
        const regex = new RegExp(`(${searchTerm})`, 'gi');
        return text.replace(regex, '<mark>$1</mark>');
    }

    function getOrderStatusBadge(status) {
        const badges = {
            'Pending': '<span class="badge bg-warning text-dark" style="font-size: 0.65rem;">Beklemede</span>',
            'InProduction': '<span class="badge bg-info" style="font-size: 0.65rem;">Üretimde</span>',
            'Completed': '<span class="badge bg-success" style="font-size: 0.65rem;">Tamamlandı</span>',
            'Delivered': '<span class="badge bg-primary" style="font-size: 0.65rem;">Teslim</span>',
            'Cancelled': '<span class="badge bg-danger" style="font-size: 0.65rem;">İptal</span>',
            'Processing': '<span class="badge bg-secondary" style="font-size: 0.65rem;">İşleniyor</span>',
            'Confirmed': '<span class="badge bg-success" style="font-size: 0.65rem;">Onaylandı</span>',
            'New': '<span class="badge bg-info" style="font-size: 0.65rem;">Yeni</span>'
        };
        return badges[status] || `<span class="badge bg-secondary" style="font-size: 0.65rem;">${status}</span>`;
    }
});
