(function () {
    'use strict';

    window.validateDates = function () {
        var fromDateInput = document.getElementById('fromDateFilter');
        var toDateInput = document.getElementById('toDateFilter');
        var fromDate = fromDateInput ? fromDateInput.value : '';
        var toDate = toDateInput ? toDateInput.value : '';

        if (fromDate && toDate && fromDate > toDate) {
            if (window.toastr) {
                window.toastr.error('عذرًا، يجب أن يكون تاريخ "من" أقل من أو يساوي تاريخ "إلى"', 'خطأ في التاريخ');
            } else {
                window.alert('عذرًا، يجب أن يكون تاريخ "من" أقل من أو يساوي تاريخ "إلى"');
            }
            return false;
        }

        return true;
    };

    function initializePurchaseOrderPage() {
        var canUseBootstrapModal = !!(window.bootstrap && window.bootstrap.Modal);
        var filterForm = document.getElementById('purchaseOrderFiltersForm');
        var supplierFilter = document.getElementById('supplierNameFilter');
        var fromDateFilter = document.getElementById('fromDateFilter');
        var toDateFilter = document.getElementById('toDateFilter');
        var clearFiltersBtn = document.getElementById('clearFiltersBtn');
        var deleteButtons = document.querySelectorAll('.js-open-delete');
        var deleteModalElement = document.getElementById('deleteOrderModal');
        var deleteModal = deleteModalElement && canUseBootstrapModal
            ? new window.bootstrap.Modal(deleteModalElement)
            : null;
        var deleteText = document.getElementById('deleteOrderModalText');
        var deleteIdInput = document.getElementById('deleteOrderId');
        var deleteForm = document.getElementById('deleteOrderForm');
        var printAllBtn = document.getElementById('printAllBtn');
        var exportReportBtn = document.getElementById('exportReportBtn');
        var allRows = Array.from(document.querySelectorAll('#ordersTable tbody .po-row'));
        var emptyStateRow = document.getElementById('ordersEmptyState');
        var pagination = document.getElementById('ordersPagination');
        var pageSizeSelect = document.getElementById('ordersPageSize');
        var pageSize = pageSizeSelect ? parseInt(pageSizeSelect.value || '10', 10) : 10;
        var currentPage = 1;
        var filteredRows = allRows.slice();

        function normalizeText(value) {
            return (value || '').trim().toLocaleLowerCase();
        }

        function parseDate(value) {
            if (!value) {
                return null;
            }

            return new Date(value + 'T00:00:00');
        }

        function getFilteredRows() {
            var selectedSupplier = normalizeText(supplierFilter ? supplierFilter.value : '');
            var fromDate = parseDate(fromDateFilter ? fromDateFilter.value : '');
            var toDate = parseDate(toDateFilter ? toDateFilter.value : '');

            return allRows.filter(function (row) {
                var rowSupplier = normalizeText(row.dataset.supplierName);
                var rowDate = parseDate(row.dataset.orderDate);

                if (selectedSupplier && rowSupplier !== selectedSupplier) {
                    return false;
                }

                if (fromDate && rowDate && rowDate < fromDate) {
                    return false;
                }

                if (toDate && rowDate && rowDate > toDate) {
                    return false;
                }

                return true;
            });
        }

        function getTotalPages() {
            return Math.max(1, Math.ceil(filteredRows.length / pageSize));
        }

        function toggleEmptyState() {
            if (!emptyStateRow) {
                return;
            }

            emptyStateRow.style.display = filteredRows.length === 0 ? 'table-row' : 'none';
        }

        function renderPage(page) {
            var totalPages = getTotalPages();
            currentPage = Math.min(Math.max(page, 1), totalPages);

            var start = (currentPage - 1) * pageSize;
            var end = start + pageSize;

            allRows.forEach(function (row) {
                row.style.display = 'none';
            });

            filteredRows.forEach(function (row, index) {
                row.style.display = index >= start && index < end ? '' : 'none';
            });

            toggleEmptyState();
        }

        function renderPagination() {
            if (!pagination) {
                return;
            }

            var totalPages = getTotalPages();
            var html = '';
            var prevDisabled = currentPage === 1 ? 'disabled' : '';
            html += '<li class="page-item ' + prevDisabled + '"><button type="button" class="page-link" data-page="' + (currentPage - 1) + '">السابق</button></li>';

            for (var p = 1; p <= totalPages; p++) {
                var active = p === currentPage ? 'active' : '';
                html += '<li class="page-item ' + active + '"><button type="button" class="page-link" data-page="' + p + '">' + p + '</button></li>';
            }

            var nextDisabled = currentPage === totalPages ? 'disabled' : '';
            html += '<li class="page-item ' + nextDisabled + '"><button type="button" class="page-link" data-page="' + (currentPage + 1) + '">التالي</button></li>';
            pagination.innerHTML = html;
        }

        function buildExportTable() {
            var sourceTable = document.getElementById('ordersTable');
            if (!sourceTable) {
                return null;
            }

            var clonedTable = sourceTable.cloneNode(false);
            var clonedHead = sourceTable.querySelector('thead').cloneNode(true);
            var clonedBody = document.createElement('tbody');

            clonedHead.querySelectorAll('th.print-hide').forEach(function (el) {
                el.remove();
            });

            if (filteredRows.length === 0) {
                clonedBody.innerHTML = '<tr><td colspan="3" class="text-center py-4 text-muted">لا توجد سجلات حالياً</td></tr>';
            } else {
                filteredRows.forEach(function (row) {
                    var clonedRow = row.cloneNode(true);
                    clonedRow.style.display = '';
                    clonedRow.querySelectorAll('td.print-hide').forEach(function (el) {
                        el.remove();
                    });
                    clonedBody.appendChild(clonedRow);
                });
            }

            clonedTable.appendChild(clonedHead);
            clonedTable.appendChild(clonedBody);
            return clonedTable;
        }

        function applyFilters(resetPage) {
            if (!window.validateDates()) {
                return;
            }

            filteredRows = getFilteredRows();

            if (resetPage) {
                currentPage = 1;
            }

            renderPage(currentPage);
            renderPagination();
        }

        if (filterForm) {
            filterForm.addEventListener('submit', function (event) {
                event.preventDefault();
                applyFilters(true);
            });
        }

        [supplierFilter, fromDateFilter, toDateFilter].forEach(function (control) {
            if (!control) {
                return;
            }

            control.addEventListener('change', function () {
                applyFilters(true);
            });
        });

        if (clearFiltersBtn) {
            clearFiltersBtn.addEventListener('click', function () {
                if (supplierFilter) {
                    supplierFilter.value = '';
                }

                if (fromDateFilter) {
                    fromDateFilter.value = '';
                }

                if (toDateFilter) {
                    toDateFilter.value = '';
                }

                applyFilters(true);
            });
        }

        deleteButtons.forEach(function (btn) {
            btn.addEventListener('click', function () {
                var id = this.dataset.id;
                var orderNumber = this.dataset.orderNumber;

                if (deleteIdInput) {
                    deleteIdInput.value = id;
                }

                if (!canUseBootstrapModal) {
                    if (window.confirm('هل أنت متأكد من حذف أمر الشراء رقم ' + orderNumber + '؟')) {
                        deleteForm && deleteForm.submit();
                    }
                    return;
                }

                if (deleteText) {
                    deleteText.textContent = 'هل أنت متأكد من حذف أمر الشراء رقم ' + orderNumber + '؟';
                }

                deleteModal && deleteModal.show();
            });
        });

        if (printAllBtn) {
            printAllBtn.addEventListener('click', function () {
                var printableTable = buildExportTable();
                if (!printableTable) {
                    return;
                }

                var printWindow = window.open('', '_blank', 'width=1100,height=800');
                if (!printWindow) {
                    return;
                }

                printWindow.document.open();
                printWindow.document.write(
                    '<html><head><meta charset="UTF-8"><title>طباعة أوامر الشراء</title>' +
                    '<style>body{font-family:Tahoma,Arial,sans-serif;padding:24px;direction:rtl;}table{width:100%;border-collapse:collapse;}th,td{border:1px solid #dfe5ea;padding:10px;text-align:center;}thead th{background:#3fa847;color:#fff;}</style>' +
                    '</head><body>' +
                    '<h3 style="margin:0 0 16px;">إدارة أوامر الشراء</h3>' +
                    printableTable.outerHTML +
                    '</body></html>'
                );
                printWindow.document.close();
                printWindow.focus();
                printWindow.onload = function () {
                    printWindow.print();
                    printWindow.close();
                };
            });
        }

        if (exportReportBtn) {
            exportReportBtn.addEventListener('click', function () {
                var exportTable = buildExportTable();
                if (!exportTable) {
                    return;
                }

                var html = '<html><head><meta charset="UTF-8"></head><body>' + exportTable.outerHTML + '</body></html>';
                var blob = new Blob([html], { type: 'application/vnd.ms-excel;charset=utf-8;' });
                var link = document.createElement('a');
                var url = URL.createObjectURL(blob);

                link.href = url;
                link.download = 'PurchaseOrdersReport_' + new Date().toISOString().slice(0, 10) + '.xls';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                URL.revokeObjectURL(url);
            });
        }

        if (pagination) {
            pagination.addEventListener('click', function (event) {
                var btn = event.target.closest('button[data-page]');
                if (!btn) {
                    return;
                }

                var totalPages = getTotalPages();
                var page = parseInt(btn.dataset.page || '1', 10);
                if (Number.isNaN(page) || page < 1 || page > totalPages || page === currentPage) {
                    return;
                }

                renderPage(page);
                renderPagination();
            });
        }

        if (pageSizeSelect) {
            pageSizeSelect.addEventListener('change', function () {
                var nextSize = parseInt(this.value || '10', 10);
                pageSize = Number.isNaN(nextSize) || nextSize <= 0 ? 10 : nextSize;
                currentPage = 1;
                renderPage(currentPage);
                renderPagination();
            });
        }

        applyFilters(true);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializePurchaseOrderPage);
    } else {
        initializePurchaseOrderPage();
    }
})();
