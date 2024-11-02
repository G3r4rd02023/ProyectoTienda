const showInPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: (res) => {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
        },
        error: (err) => {
            console.error("Error loading popup:", err);
        }
    });
}

const jQueryAjaxPost = (form) => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: (res) => {
                if (res.isValid) {
                    $('#view-all').html(res.html);
                    $('#form-modal').modal('hide');
                    location.reload();
                } else {
                    $('#form-modal .modal-body').html(res.html);
                }
            },
            error: (err) => {
                console.error("Error submitting form:", err);
            }
        });

        return false;
    } catch (ex) {
        console.error("Exception during form submission:", ex);
    }
}

((modalDeleteDialog) => {
    const methods = {
        openModal,
        deleteItem
    };

    let item_to_delete;

    function openModal(modalName, classOrId, sourceEvent, deletePath, eventClassOrId) {
        const textEvent = classOrId ? `.${modalName}` : `#${modalName}`;
        $(textEvent).on('click', (e) => {
            item_to_delete = e.currentTarget.dataset.id;
            deleteItem(sourceEvent, deletePath, eventClassOrId);
        });
    }

    function deleteItem(sourceEvent, deletePath, eventClassOrId) {
        const textEvent = eventClassOrId ? `.${sourceEvent}` : `#${sourceEvent}`;
        $(textEvent).on('click', (e) => {
            window.location.href = `${deletePath}${item_to_delete}`;
        });
    }

    modalDeleteDialog.sc_deleteDialog = methods;
})(window);