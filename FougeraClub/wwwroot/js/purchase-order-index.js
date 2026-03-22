(function () {
    'use strict';

    window.validateDates = function () {
        var fromDateInput = document.querySelector('input[name="fromDate"]');
        var toDateInput = document.querySelector('input[name="toDate"]');
        var fromDate = fromDateInput ? fromDateInput.value : '';
        var toDate = toDateInput ? toDateInput.value : '';

        if (fromDate && toDate && fromDate > toDate) {
            if (window.toastr) {
                window.toastr.error('عذراً، يجب أن يكون تاريخ "من" أقل من أو يساوي تاريخ "إلى"', 'خطأ في التاريخ');
            } else {
                window.alert('عذراً، يجب أن يكون تاريخ "من" أقل من أو يساوي تاريخ "إلى"');
            }
            return false;
        }

        return true;
    };

    function initializePurchaseOrderPage() {
        var canUseBootstrapModal = !!(window.bootstrap && window.bootstrap.Modal);
        var addButtons = document.querySelectorAll('.js-open-add');
        var editButtons = document.querySelectorAll('.js-open-edit');
        var deleteButtons = document.querySelectorAll('.js-open-delete');
        var printButtons = document.querySelectorAll('.js-open-print');

        function createModalApi(id) {
            var el = document.getElementById(id);
            if (!el || !canUseBootstrapModal) {
                return {
                    show: function () { },
                    hide: function () { }
                };
            }

            var instance = new window.bootstrap.Modal(el);
            return {
                show: function () { instance.show(); },
                hide: function () { instance.hide(); }
            };
        }

        var addModal = createModalApi('addOrderModal');
        var editModal = createModalApi('editOrderModal');
        var deleteModal = createModalApi('deleteOrderModal');
        var printModal = createModalApi('printOrderModal');

        var editText = document.getElementById('editOrderModalText');
        var editBtn = document.getElementById('editOrderConfirmBtn');

        var deleteText = document.getElementById('deleteOrderModalText');
        var deleteIdInput = document.getElementById('deleteOrderId');
        var deleteForm = document.getElementById('deleteOrderForm');

        var printText = document.getElementById('printOrderModalText');
        var printConfirmBtn = document.getElementById('printOrderConfirmBtn');
        var exportReportBtn = document.getElementById('exportReportBtn');
        var clearFiltersBtn = document.getElementById('clearFiltersBtn');
        var tableRows = Array.from(document.querySelectorAll('#ordersTable tbody .po-row'));
        var pagination = document.getElementById('ordersPagination');
        var paginationSummary = document.getElementById('ordersPaginationSummary');
        var pageSizeSelect = document.getElementById('ordersPageSize');
        var pageSize = pageSizeSelect ? parseInt(pageSizeSelect.value || '10', 10) : 10;
        var currentPage = 1;
        var selectedPrintRowId = null;

        function getTotalPages() {
            return Math.max(1, Math.ceil(tableRows.length / pageSize));
        }

        addButtons.forEach(function (btn) {
            btn.addEventListener('click', function () {
                if (!canUseBootstrapModal) {
                    window.location.href = '/Admin/PurchaseOrder/AddEdit';
                    return;
                }
                addModal.show();
            });
        });

        editButtons.forEach(function (btn) {
            btn.addEventListener('click', function () {
                var id = this.dataset.id;
                var orderNumber = this.dataset.orderNumber;
                if (!canUseBootstrapModal) {
                    window.location.href = '/Admin/PurchaseOrder/AddEdit/' + id;
                    return;
                }
                if (editText) {
                    editText.textContent = 'هل تريد تعديل أمر الشراء رقم ' + orderNumber + '؟';
                }
                if (editBtn) {
                    editBtn.href = '/Admin/PurchaseOrder/AddEdit/' + id;
                }
                editModal.show();
            });
        });

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
                deleteModal.show();
            });
        });

        printButtons.forEach(function (btn) {
            btn.addEventListener('click', function () {
                selectedPrintRowId = this.dataset.id;
                var orderNumber = this.dataset.orderNumber;
                var supplier = this.dataset.supplier;
                var date = this.dataset.date;

                if (!canUseBootstrapModal) {
                    window.location.href = '/Admin/PurchaseOrder/Print/' + selectedPrintRowId;
                    return;
                }

                if (printText) {
                    printText.innerHTML = '<div class="mb-2"><strong>رقم أمر الشراء:</strong> ' + orderNumber + '</div>'
                        + '<div class="mb-2"><strong>اسم المورد:</strong> ' + supplier + '</div>'
                        + '<div><strong>التاريخ:</strong> ' + date + '</div>';
                }
                printModal.show();
            });
        });

        var printAllBtn = document.querySelector('[data-print-all="true"]');
        if (printAllBtn) {
            printAllBtn.addEventListener('click', function () {
                selectedPrintRowId = null;
                if (!canUseBootstrapModal) {
                    window.print();
                    return;
                }
                if (printText) {
                    printText.textContent = 'سيتم طباعة قائمة أوامر الشراء بالكامل. أثناء الطباعة سيتم إخفاء أزرار الإضافة/التعديل/الحذف.';
                }
            });
        }

        if (exportReportBtn) {
            exportReportBtn.addEventListener('click', function () {
                var sourceTable = document.getElementById('ordersTable');
                if (!sourceTable) return;

                var cloned = sourceTable.cloneNode(true);
                cloned.querySelectorAll('th.print-hide, td.print-hide').forEach(function (el) { el.remove(); });

                var html = '<html><head><meta charset="UTF-8"></head><body>' + cloned.outerHTML + '</body></html>';
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

        if (clearFiltersBtn) {
            clearFiltersBtn.addEventListener('click', function () {
                window.location.href = '/Admin/PurchaseOrder/Index';
            });
        }

        function renderPage(page) {
            var totalPages = getTotalPages();
            currentPage = page;
            var start = (page - 1) * pageSize;
            var end = start + pageSize;

            tableRows.forEach(function (row, index) {
                row.style.display = index >= start && index < end ? '' : 'none';
            });

            if (paginationSummary) {
                if (tableRows.length === 0) {
                    paginationSummary.textContent = 'عرض 0 من 0';
                    return;
                }

                var fromRecord = start + 1;
                var toRecord = Math.min(end, tableRows.length);
                paginationSummary.textContent = 'عرض ' + fromRecord + ' - ' + toRecord + ' من ' + tableRows.length;
            }
        }

        function renderPagination() {
            var totalPages = getTotalPages();
            if (!pagination) return;
            if (tableRows.length <= pageSize) {
                pagination.innerHTML = '';
                return;
            }

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

        if (pagination) {
            pagination.addEventListener('click', function (e) {
                var btn = e.target.closest('button[data-page]');
                if (!btn) return;
                var totalPages = getTotalPages();
                var page = parseInt(btn.dataset.page || '1', 10);
                if (Number.isNaN(page) || page < 1 || page > totalPages || page === currentPage) return;
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

        renderPage(1);
        renderPagination();

        function cleanupPrintState() {
            document.querySelectorAll('.po-row').forEach(function (row) {
                row.classList.remove('po-print-hidden');
            });
            document.body.classList.remove('po-print-mode');
        }

        if (printConfirmBtn) {
            printConfirmBtn.addEventListener('click', function () {
                printModal.hide();
                if (selectedPrintRowId) {
                    window.location.href = '/Admin/PurchaseOrder/Print/' + selectedPrintRowId;
                    return;
                }

                document.body.classList.add('po-print-mode');
                setTimeout(function () {
                    window.print();
                    setTimeout(cleanupPrintState, 300);
                }, 200);
            });
        }

        window.addEventListener('afterprint', cleanupPrintState);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializePurchaseOrderPage);
    } else {
        initializePurchaseOrderPage();
    }
})();
