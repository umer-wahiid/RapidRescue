"use strict";

$('#image').on('change', function () {
    var file = this.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#imagePreview').attr('src', e.target.result).show();
            $('#clearImage').show();
        }
        reader.readAsDataURL(file);
    }
});

$('#clearImage').on('click', function () {
    $('#image').val('');
    $('#imagePreview').hide().attr('src', '#');
    $(this).hide();
});

$('label[for="image"]').on('click', function () {
    event.preventDefault();
    $('#image').click();
});

var dash = {
    chart: function (comp, inProg, pend, can) {
        $("#barChartContainer").dxChart({
            dataSource: [
                { category: "Completed", value: comp },
                { category: "In Progress", value: inProg },
                { category: "Pending", value: pend },
                { category: "Cancelled", value: can }
            ],
            commonSeriesSettings: {
                type: "bar"
            },
            series: [{
                argumentField: "category",
                valueField: "value",
                name: "Status",
                color: "#79c8ff"
            }],
            legend: {
                visible: false
            },
            argumentAxis: {
                label: {
                    rotationAngle: -45
                }
            }
        });
    }
}

var notification = {
    markAllAsRead: function () {
        $.ajax({
            url: '/EmergencyRequests/ClearNotifications',
            type: 'POST',
            success: function (response) {
                $('#notificationCount').text(0);
                $('#signalRNotifications').html('');
            },
            error: function (xhr, status, error) {
                Notify('danger', error);
            }
        });
    }
}

function showLoader(callback) {
    const loader = document.getElementById('loader');
    if (loader) {
        loader.style.display = 'flex';
    }
    setTimeout(function () {
        if (callback && typeof callback === 'function') {
            callback();
        }
    }, 500);
}

function hideLoader() {
    document.getElementById('loader').style.display = 'none';
}

function Notify(state, message, time) {
    var content = {};

    // Map state to corresponding icons
    var iconMap = {
        success: "fa fa-check-circle",
        info: "fa fa-info-circle",
        warning: "fa fa-exclamation-triangle",
        danger: "fa fa-exclamation-circle"
    };
    var titleMap = {
        success: "Success",
        info: "Info",
        warning: "Warning",
        danger: "Error"
    };

    var icon = iconMap[state] || "fa fa-bell";

    content.message = message;
    content.title = titleMap[state] || "Error Message";
    content.icon = icon;
    $.notify(content, {
        type: state,
        placement: {
            from: "top",
            align: "right",
        },
        time: time || 2000,
        delay: 0,
        delay: 3000,
        allow_dismiss: true,
        z_index: 9999999
    });
}

var user = {
    onInit: function () {
        showLoader(function () {
            user.initializeGrid();
            hideLoader();
        });

        $('#userModal').on('hidden.bs.modal', function () {
            user.clearForm(); // Call the clearForm function when modal is hidden
        });
        $('#DeleteConfirmationModal').on('hidden.bs.modal', function () {
            user.clearForm(); // Call the clearForm function when modal is hidden
        });
    },
    roleNames: {
        1: "Admin",
        2: "Patient",
        3: "EMT",
        4: "Driver"
    },
    initializeGrid: function () {
        $.ajax({
            url: '/Users/Get',
            type: 'GET',
            async: true,
            success: function (response) {
                if (response.success) {

                    var dataSource = response.data.map(u => ({
                        ...u,
                        role: user.roleNames[u.role] || "Unknown"
                    }));;

                    $("#userGrid").dxDataGrid({
                        dataSource: dataSource,
                        columns: [
                            {
                                dataField: "firstName",
                                caption: "First Name"
                            },
                            {
                                dataField: "lastName",
                                caption: "Last Name"
                            },
                            {
                                dataField: "role",
                                caption: "Role"
                            },
                            {
                                dataField: "phone",
                                caption: "Phone"
                            },
                            {
                                dataField: "email",
                                caption: "Email"
                            },
                            {
                                dataField: "address",
                                caption: "Address"
                            },
                            {
                                dataField: "createdDate",
                                caption: "Created Date",
                                dataType: "date",
                                format: "yyyy/MM/dd HH:mm:ss",
                            },
                            {
                                dataField: "updatedDate",
                                caption: "Updated Date",
                                dataType: "date",
                                format: "yyyy/MM/dd HH:mm:ss",
                            },
                            {
                                dataField: "Action",
                                caption: "Action",
                                width: 150,
                                cellTemplate: function (container, options) {
                                    var actionDiv = $("<div class='text-center'>");

                                    actionDiv.append(
                                        $("<button>")
                                            .addClass("btn btn-view btn-link btn-primary btn-sm p-2 viewScheduleBtn")
                                            .attr({
                                                title: "View",
                                                "onclick": "user.populate(" + options.data.id + ",true)"
                                            })
                                            .append($("<i>").addClass("fa fa-eye"))
                                    );
                                    actionDiv.append(
                                        $("<button>")
                                            .addClass("btn btn-link btn-primary btn-sm p-2")
                                            .attr({
                                                title: "Edit",
                                                "onclick": "user.populate(" + options.data.id + ")"
                                            })
                                            .append($("<i>").addClass("fa fa-edit"))
                                    );
                                    actionDiv.append(
                                        $("<button>")
                                            .addClass("btn btn-link btn-danger btn-sm p-2")
                                            .attr({
                                                title: "Delete",
                                                "data-bs-toggle": "modal",
                                                "data-bs-target": "#DeleteConfirmationModal"
                                            })
                                            .append($("<i>").addClass("fa fa-trash"))
                                            .on("click", function () {
                                                user.setDeleteUserId(options.data.id); // Set the userId for deletion
                                            })
                                    );


                                    actionDiv.appendTo(container);
                                }
                            }
                        ],
                        showBorders: true,
                        sorting: {
                            mode: "multiple"
                        },
                        paging: {
                            enabled: true,
                            pageSize: 5
                        },
                        pager: {
                            showPageSizeSelector: true,
                            allowedPageSizes: [5, 10, 20],
                            showInfo: true
                        },

                        headerFilter: {
                            visible: true
                        },
                        searchPanel: {
                            visible: true,
                        },
                        filterRow: {
                            visible: true,
                            applyFilter: 'auto'
                        },
                        headerFilter: {
                            visible: true
                        },
                        export: {
                            enabled: true,
                            fileName: 'UsersData',
                        },
                        summary: {
                            totalItems: [{
                                column: "id",
                                summaryType: "count",
                                displayFormat: "Total: {0}"
                            }]
                        },
                        columnAutoWidth: true, // Adjust column width automatically

                    });
                } else {
                    Notify('danger', response.message);
                }
            },
            error: function (xhr, status, error) {
                debugger;
                Notify('danger', error);
            }
        });
    },
    userId: null,
    save: function () {
        debugger;
        var form = $('#frmUser');
        var formData = new FormData(form[0]);
        var url = this.userId ? `/Users/Edit/${this.userId}` : '/Users/Add';
        if (form.valid()) {
            $.ajax({
                url: url,
                type: 'POST',
                processData: false,  // Important!
                contentType: false,  // Important!
                data: formData,
                success: function (response) {
                    if (response.success) {
                        Notify('success', response.message);
                        if (window.location.href.includes('UserAuth')) {
                            window.location.href = "/";
                        } else {
                            $('#userModal').modal('hide');
                            user.initializeGrid();
                        }
                    } else {
                        Notify('danger', response.message);
                    }
                },
                error: function (xhr, status, error) {
                    debugger;
                    // Handle error response
                    Notify('danger', error);
                }
            });
        }
    },
    populate: function (userId, isView) {
        this.userId = userId;
        $.ajax({
            url: `/Users/GetById/${userId}`,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    var user = response.data;
                    debugger;
                    // Populate form fields
                    $('#firstName').val(user.firstName);
                    $('#lastName').val(user.lastName);
                    $('#address').val(user.address);
                    $('#phone').val(user.phone);
                    $('#Role').val(user.role);
                    $('#email').val(user.email);
                    $('#confirmPassword').val(user.password);
                    $('#password').val(user.password);

                    //$('#password').val('');
                    //$('#confirmPassword').val('');
                    if (user.image) {
                        $('#imagePreview').attr('src', `/${user.image}`).show();
                    } else {
                        $('#imagePreview').hide();
                        $('#clearImage').hide();
                    }

                    if (isView) {
                        // Disable form fields for view mode
                        $('#frmUser input').prop('disabled', true);
                        $('#frmUser select').prop('disabled', true);
                        $('#userModal .modal-title').text('View User');
                        $("#btnSave").hide();
                    } else {
                        // Enable form fields for edit mode
                        $('#frmUser input').prop('disabled', false);
                        $('#userModal .modal-title').text('Edit User');
                    }
                    $('#userModal').modal('show');

                } else {
                    Notify('danger', response.message);
                }
            },
            error: function (xhr, status, error) {
                // Handle error response
                Notify('danger', error);
            }
        });
    },
    delete: function () {
        debugger;
        $.ajax({
            url: `/Users/Delete/${this.userId}`,
            type: 'POST',
            success: function (response) {
                if (response.success) {

                    Notify('success', response.message);
                    this.userId = null;
                    user.initializeGrid();

                } else {
                    Notify('danger', response.message);
                    this.userId = null;
                }
            },
            error: function (xhr, status, error) {
                // Handle error response
                Notify('danger', error);
            }
        });
        $('#DeleteConfirmationModal').modal('hide');
    },
    clearForm: function () {
        $('#frmUser')[0].reset();
        $('#frmUser select').prop('disabled', false);

        $('#frmUser').find('.text-danger').text('');
        this.userId = null;
        $("#btnSave").show();
        $('#imagePreview').hide().attr('src', '#');
        $('#clearImage').hide();
        $('#image').val('');
    },
    setDeleteUserId: function (userId) {
        this.userId = userId;
    }
};

var ambulance = {
    onInit: function () {
        showLoader(function () {
            ambulance.initializeGrid();
            hideLoader();
        });

        $('#ambulanceModal').on('hidden.bs.modal', function () {
            ambulance.clearForm(); // Call the clearForm function when modal is hidden
        });
        $('#DeleteConfirmationModal').on('hidden.bs.modal', function () {
            ambulance.clearForm(); // Call the clearForm function when modal is hidden
        });
    },
    equipmentLevels: {
        1: "Basic",
        2: "Advanced"
    },
    initializeGrid: function () {
        $.ajax({
            url: '/Ambulances/Get',
            type: 'GET',
            async: true,
            success: function (response) {
                if (response.success) {
                    var dataSource = response.data.map(a => ({
                        ...a,
                        equipmentLevel: ambulance.equipmentLevels[a.equipmentLevel] || "Unknown"
                    }));;
                    $("#ambulanceGrid").dxDataGrid({
                        dataSource: dataSource,
                        columns: [
                            {
                                dataField: "image",
                                caption: "Image",
                                cellTemplate: function (container, options) {
                                    $("<img>")
                                        .attr("src", `/${options.value}`)
                                        .attr("alt", "Profile Pic")
                                        .addClass("profilePic")
                                        .appendTo(container);
                                },
                                width: 100,
                                alignment: "center"
                            },
                            {
                                dataField: "equipmentLevel",
                                caption: "Equipment Level"
                            },
                            {
                                dataField: "vehicleNumber",
                                caption: "Vehicle Number"
                            },

                            {
                                caption: "Driver",
                                calculateCellValue: function (data) {
                                    return data.driver.firstName + " " + data.driver.lastName;
                                }
                            },
                            {
                                dataField: "createdDate",
                                caption: "Created Date",
                                dataType: "date",
                                format: "yyyy/MM/dd HH:mm:ss",
                            },
                            {
                                dataField: "updatedDate",
                                caption: "Updated Date",
                                dataType: "date",
                                format: "yyyy/MM/dd HH:mm:ss",
                            },
                            {
                                dataField: "Action",
                                caption: "Action",
                                width: 150,
                                cellTemplate: function (container, options) {
                                    var actionDiv = $("<div class='text-center'>");

                                    actionDiv.append(
                                        $("<button>")
                                            .addClass("btn btn-view btn-link btn-primary btn-sm p-2 viewScheduleBtn")
                                            .attr({
                                                title: "View",
                                                "onclick": "ambulance.populate(" + options.data.id + ",true)"
                                            })
                                            .append($("<i>").addClass("fa fa-eye"))
                                    );
                                    actionDiv.append(
                                        $("<button>")
                                            .addClass("btn btn-link btn-primary btn-sm p-2")
                                            .attr({
                                                title: "Edit",
                                                "onclick": "ambulance.populate(" + options.data.id + ")"
                                            })
                                            .append($("<i>").addClass("fa fa-edit"))
                                    );
                                    actionDiv.append(
                                        $("<button>")
                                            .addClass("btn btn-link btn-danger btn-sm p-2")
                                            .attr({
                                                title: "Delete",
                                                "data-bs-toggle": "modal",
                                                "data-bs-target": "#DeleteConfirmationModal"
                                            })
                                            .append($("<i>").addClass("fa fa-trash"))
                                            .on("click", function () {
                                                ambulance.setDeleteAmbulanceId(options.data.id); // Set the ambulanceId for deletion
                                            })
                                    );


                                    actionDiv.appendTo(container);
                                }
                            }
                        ],
                        showBorders: true,
                        sorting: {
                            mode: "multiple"
                        },
                        paging: {
                            enabled: true,
                            pageSize: 5
                        },
                        pager: {
                            showPageSizeSelector: true,
                            allowedPageSizes: [5, 10, 20],
                            showInfo: true
                        },

                        headerFilter: {
                            visible: true
                        },
                        searchPanel: {
                            visible: true,
                        },
                        filterRow: {
                            visible: true,
                            applyFilter: 'auto'
                        },
                        headerFilter: {
                            visible: true
                        },
                        export: {
                            enabled: true,
                            fileName: 'AmbulancesData',
                        },
                        summary: {
                            totalItems: [{
                                column: "id",
                                summaryType: "count",
                                displayFormat: "Total: {0}"
                            }]
                        },
                        columnAutoWidth: true, // Adjust column width automatically

                    });
                } else {
                    Notify('danger', response.message);
                }
            },
            error: function (xhr, status, error) {
                debugger;
                Notify('danger', error);
            }
        });
    },
    ambulanceId: null,
    save: function () {
        debugger;
        var form = $('#frmAmbulance');
        var formData = new FormData(form[0]);
        var url = this.ambulanceId ? `/Ambulances/Edit/${this.ambulanceId}` : '/Ambulances/Add';
        if (form.valid()) {
            $.ajax({
                url: url,
                type: 'POST',
                processData: false,  // Important!
                contentType: false,  // Important!
                data: formData,
                success: function (response) {
                    if (response.success) {

                        Notify('success', response.message);
                        $('#ambulanceModal').modal('hide');
                        ambulance.initializeGrid();
                    } else {
                        Notify('danger', response.message);
                    }
                },
                error: function (xhr, status, error) {
                    debugger;
                    // Handle error response
                    Notify('danger', error);
                }
            });
        }
    },
    populate: function (ambulanceId, isView) {
        this.ambulanceId = ambulanceId;
        $.ajax({
            url: `/Ambulances/GetById/${ambulanceId}`,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    var ambulance = response.data;
                    debugger;
                    // Populate form fields
                    $('#vehicleNumber').val(ambulance.vehicleNumber);

                    //$('#password').val('');
                    //$('#confirmPassword').val('');
                    if (ambulance.image) {
                        $('#imagePreview').attr('src', `/${ambulance.image}`).show();
                    } else {
                        $('#imagePreview').hide();
                        $('#clearImage').hide();
                    }

                    if (ambulance.driverId) {
                        $('#lastName').val(ambulance.driverId);
                    }

                    if (ambulance.equipmentLevel) {
                        $('#EquipmentLevel').val(ambulance.equipmentLevel);
                    }

                    if (isView) {
                        $('#frmAmbulance input').prop('disabled', true);
                        $('#frmAmbulance select').prop('disabled', true);
                        $('#ambulanceModal .modal-title').text('View Ambulance');
                        $("#btnSave").hide();
                    } else {
                        $('#frmAmbulance input').prop('disabled', false);
                        $('#ambulanceModal .modal-title').text('Edit Ambulance');
                    }
                    $('#ambulanceModal').modal('show');

                } else {
                    Notify('danger', response.message);
                }
            },
            error: function (xhr, status, error) {
                // Handle error response
                Notify('danger', error);
            }
        });
    },
    delete: function () {
        debugger;
        $.ajax({
            url: `/Ambulances/Delete/${this.ambulanceId}`,
            type: 'POST',
            success: function (response) {
                if (response.success) {

                    Notify('success', response.message);
                    this.ambulanceId = null;
                    ambulance.initializeGrid();

                } else {
                    Notify('danger', response.message);
                    this.ambulanceId = null;
                }
            },
            error: function (xhr, status, error) {
                // Handle error response
                Notify('danger', error);
            }
        });
        $('#DeleteConfirmationModal').modal('hide');
    },
    clearForm: function () {
        $('#frmAmbulance')[0].reset();
        $('#frmAmbulance select').prop('disabled', false);

        $('#frmambulance').find('.text-danger').text('');
        this.ambulanceId = null;
        $("#btnSave").show();
        $('#imagePreview').hide().attr('src', '#');
        $('#clearImage').hide();
        $('#image').val('');
    },
    setDeleteAmbulanceId: function (ambulanceId) {
        this.ambulanceId = ambulanceId;
    }
};

var firstAid = {

    firstAidId: null,
    onInit: function () {
        showLoader(function () {
            firstAid.initializeGrid();
            hideLoader();
        });

        $('#firstAidModal').on('hidden.bs.modal', function () {
            firstAid.clearForm(); // Call the clearForm function when modal is hidden
        });
        $('#DeleteConfirmationModal').on('hidden.bs.modal', function () {
            firstAid.clearForm(); // Call the clearForm function when modal is hidden
        });
    },

    initializeGrid: function () {
        $.ajax({
            url: '/FirstAids/Get',
            type: 'GET',
            async: true,
            success: function (response) {
                if (response.success) {
                    var dataSource = response.data;
                    $("#firstAidGrid").dxDataGrid({
                        dataSource: dataSource,
                        columns: [
                            {
                                dataField: "symptoms",
                                caption: "Symptoms"
                            },
                            {
                                dataField: "firstAidDetail",
                                caption: "First-Aid Details",
                                cssClass: "wrap-text", 
                                cellTemplate: function (container, options) {
                                    $("<div>").text(options.value).css("white-space", "normal").appendTo(container); 
                                }
                            },
                            {
                                dataField: "createdDate",
                                caption: "Created Date",
                                dataType: "date",
                                format: "yyyy/MM/dd HH:mm:ss",
                            },
                            {
                                dataField: "updatedDate",
                                caption: "Updated Date",
                                dataType: "date",
                                format: "yyyy/MM/dd HH:mm:ss",
                            },
                            {
                                dataField: "Action",
                                caption: "Action",
                                width: 150,
                                cellTemplate: function (container, options) {
                                    var actionDiv = $("<div class='text-center'>");

                                    actionDiv.append(
                                        $("<button>")
                                            .addClass("btn btn-view btn-link btn-primary btn-sm p-2 viewScheduleBtn")
                                            .attr({
                                                title: "View",
                                                "onclick": "firstAid.populate(" + options.data.id + ",true)"
                                            })
                                            .append($("<i>").addClass("fa fa-eye"))
                                    );
                                    actionDiv.append(
                                        $("<button>")
                                            .addClass("btn btn-link btn-primary btn-sm p-2")
                                            .attr({
                                                title: "Edit",
                                                "onclick": "firstAid.populate(" + options.data.id + ")"
                                            })
                                            .append($("<i>").addClass("fa fa-edit"))
                                    );
                                    actionDiv.append(
                                        $("<button>")
                                            .addClass("btn btn-link btn-danger btn-sm p-2")
                                            .attr({
                                                title: "Delete",
                                                "data-bs-toggle": "modal",
                                                "data-bs-target": "#DeleteConfirmationModal"
                                            })
                                            .append($("<i>").addClass("fa fa-trash"))
                                            .on("click", function () {
                                                firstAid.setDeleteFirstAidId(options.data.id); // Set the firstAidId for deletion
                                            })
                                    );


                                    actionDiv.appendTo(container);
                                }
                            }
                        ],
                        showBorders: true,
                        sorting: {
                            mode: "multiple"
                        },
                        paging: {
                            enabled: true,
                            pageSize: 5
                        },
                        pager: {
                            showPageSizeSelector: true,
                            allowedPageSizes: [5, 10, 20],
                            showInfo: true
                        },

                        headerFilter: {
                            visible: true
                        },
                        searchPanel: {
                            visible: true,
                        },
                        filterRow: {
                            visible: true,
                            applyFilter: 'auto'
                        },
                        headerFilter: {
                            visible: true
                        },
                        export: {
                            enabled: true,
                            fileName: 'FirstAidsData',
                        },
                        summary: {
                            totalItems: [{
                                column: "id",
                                summaryType: "count",
                                displayFormat: "Total: {0}"
                            }]
                        },
                        columnAutoWidth: true, // Adjust column width automatically

                    });
                } else {
                    Notify('danger', response.message);
                }
            },
            error: function (xhr, status, error) {
                debugger;
                Notify('danger', error);
            }
        });
    },

    save: function () {
        debugger;
        var form = $('#frmFirstAid');
        var formData = new FormData(form[0]);
        var url = this.firstAidId ? `/FirstAids/Edit/${this.firstAidId}` : '/FirstAids/Add';
        if (form.valid()) {
            $.ajax({
                url: url,
                type: 'POST',
                processData: false,  // Important!
                contentType: false,  // Important!
                data: formData,
                success: function (response) {
                    if (response.success) {

                        Notify('success', response.message);
                        $('#firstAidModal').modal('hide');
                        firstAid.initializeGrid();
                    } else {
                        Notify('danger', response.message);
                    }
                },
                error: function (xhr, status, error) {
                    debugger;
                    // Handle error response
                    Notify('danger', error);
                }
            });
        }
    },

    populate: function (firstAidId, isView) {
        this.firstAidId = firstAidId;
        $.ajax({
            url: `/FirstAids/GetById/${firstAidId}`,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    var firstAid = response.data;
                    debugger;
                    // Populate form fields
                    $('#symptoms').val(firstAid.symptoms);
                    $('#firstAidDetail').val(firstAid.firstAidDetail);
                    if (isView) {
                        $('#frmFirstAid input').prop('disabled', true);
                        $('#frmFirstAid textarea').prop('disabled', true);
                        $('#firstAidModal .modal-title').text('View FirstAid');
                        $("#btnSave").hide();
                    } else {
                        $('#frmFirstAid input').prop('disabled', false);
                        $('#frmFirstAid textarea').prop('disabled', true);
                        $('#firstAidModal .modal-title').text('Edit FirstAid');
                    }
                    $('#firstAidModal').modal('show');

                } else {
                    Notify('danger', response.message);
                }
            },
            error: function (xhr, status, error) {
                // Handle error response
                Notify('danger', error);
            }
        });
    },

    delete: function () {
        debugger;
        $.ajax({
            url: `/FirstAids/Delete/${this.firstAidId}`,
            type: 'POST',
            success: function (response) {
                if (response.success) {

                    Notify('success', response.message);
                    this.firstAidId = null;
                    firstAid.initializeGrid();

                } else {
                    Notify('danger', response.message);
                    this.firstAidId = null;
                }
            },
            error: function (xhr, status, error) {
                // Handle error response
                Notify('danger', error);
            }
        });
        $('#DeleteConfirmationModal').modal('hide');
    },

    clearForm: function () {
        $('#frmFirstAid')[0].reset();
        $('#frmFirstAid select').prop('disabled', false);

        $('#frmfirstAid').find('.text-danger').text('');
        this.firstAidId = null;
        $("#btnSave").show();
    },

    setDeleteFirstAidId: function (firstAidId) {
        this.firstAidId = firstAidId;
    }
};

var emergencyRequest = {
    onInit: function () {
        showLoader(function () {
            emergencyRequest.initializeGrid();
            hideLoader();
        });
    },
    equipmentLevels: {
        1: "Basic",
        2: "Advanced"
    },
    requestType: {
        1: "Emergency",
        2: "NonEmergency"
    },
    RequestStatus: {
        1: "Pending",
        2: "Dispatched",
        3: "EnRoute",
        4: "Arrived",
        5: "Completed",
        6: "Canceled"
    },
    initializeGrid: function () {

        $.ajax({
            url: '/EmergencyRequests/Get',
            type: 'GET',
            async: true,
            success: function (response) {

                if (response.success) {

                    var dataSource = response.data.map(a => ({
                        ...a,
                        equipmentLevel: emergencyRequest.equipmentLevels[a.equipmentLevel] || "Unknown",
                        status: emergencyRequest.RequestStatus[a.status] || "Unknown",
                        type: emergencyRequest.requestType[a.type] || "Unknown",
                    }));

                    $("#emergencyRequestGrid").dxDataGrid({

                        dataSource: dataSource,
                        columns: [
                            {
                                caption: "User Name",
                                calculateCellValue: function (data) {
                                    return data.user.firstName + " " + data.user.lastName;
                                }
                            },
                            {
                                dataField: "hospitalName",
                                caption: "Hospital Name"
                            },
                            {
                                dataField: "emergencyContact",
                                caption: "Emergency Contact"
                            },
                            {
                                dataField: "equipmentLevel",
                                caption: "Equipment Level"
                            },
                            {
                                dataField: "ambulance.vehicleNumber", // Assuming ambulance is linked to EmergencyRequest
                                caption: "Ambulance Number",
                                calculateCellValue: function (data) {
                                    return data.ambulance?.vehicleNumber ? data.ambulance?.vehicleNumber : "UnAssigned";
                                }
                            },

                            {
                                dataField: "status",
                                caption: "Request Status"
                            },
                            {
                                dataField: "createdDate",
                                caption: "Request Date",
                                dataType: "date",
                                format: "yyyy/MM/dd HH:mm:ss",
                            },
                            {
                                dataField: "Action",
                                caption: "Action",
                                width: 150,
                                cellTemplate: function (container, options) {
                                    var actionDiv = $("<div class='text-center'>");


                                    actionDiv.append(
                                        $("<button>")
                                            .addClass("btn btn-link btn-primary btn-sm p-2")
                                            .attr({
                                                title: "Edit",
                                                "onclick": "emergencyRequest.populate(" + options.data.id + ")"
                                            })
                                            .append($("<i>").addClass("fa fa-edit"))
                                    );



                                    actionDiv.appendTo(container);
                                }
                            }
                        ],
                        showBorders: true,
                        sorting: {
                            mode: "multiple"
                        },
                        paging: {
                            enabled: true,
                            pageSize: 5
                        },
                        pager: {
                            showPageSizeSelector: true,
                            allowedPageSizes: [5, 10, 20],
                            showInfo: true
                        },

                        headerFilter: {
                            visible: true
                        },
                        searchPanel: {
                            visible: true,
                        },
                        filterRow: {
                            visible: true,
                            applyFilter: 'auto'
                        },
                        headerFilter: {
                            visible: true
                        },
                        export: {
                            enabled: true,
                            fileName: 'AmbulancesData',
                        },
                        summary: {
                            totalItems: [{
                                column: "id",
                                summaryType: "count",
                                displayFormat: "Total: {0}"
                            }]
                        },
                        columnAutoWidth: true, // Adjust column width automatically

                    });
                } else {
                    Notify('danger', response.message);
                }
            },
            error: function (xhr, status, error) {
                debugger;
                Notify('danger', error);
            }
        });
    },
    emergencyRequestId: null,
    save: function () {
        debugger;
        var form = $('#frmEmergencyRequest');
        var formData = new FormData(form[0]);
        var url = this.emergencyRequestId ? `/EmergencyRequests/Edit/${this.emergencyRequestId}` : '/EmergencyRequests/Add';
        if (form.valid()) {
            $.ajax({
                url: url,
                type: 'POST',
                processData: false,  // Important!
                contentType: false,  // Important!
                data: formData,
                success: function (response) {
                    if (response.success) {

                        Notify('success', response.message);
                        $('#frmEmergencyRequest')[0].reset();
                        if (emergencyRequest.emergencyRequestId) {
                            $('#emergencyRequestModal').modal('hide');
                            emergencyRequest.initializeGrid();
                        } else {
                            window.location.href = '/EmergencyRequests/RequestTracking';
                        }
                    } else {
                        Notify('danger', response.message);
                    }
                },
                error: function (xhr, status, error) {
                    debugger;
                    // Handle error response
                    Notify('danger', error);
                }
            });
        }
    },
    populate: function (requestId, isView) {
        this.emergencyRequestId = requestId;
        $.ajax({
            url: `/EmergencyRequests/GetById/${this.emergencyRequestId}`,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    var request = response.data;
                    debugger;
                    $('#pickupAddress').val(request.pickupAddress);
                    $('#hospitalName').val(request.hospitalName);
                    $('.emergencyContact').val(request.emergencyContact);
                    $('#medicalInfo').val(request.medicalInfo);
                    $('#UserId').val(request.userId);
                    $('#Type').val(request.type);
                    $('#EquipmentLevel').val(request.equipmentLevel);
                    $('#Status').val(request.status);
                    $('#ambulanceId').val(request.ambulanceId);

                    $('#frmEmergencyRequest input, #frmEmergencyRequest select').prop('disabled', true);
                    $('#Status').prop('disabled', false);
                    $('#ambulanceId').prop('disabled', false);
                    if (isView) {
                        $('#btnSave').hide();
                    } else {
                        //$('#frmEmergencyRequest input, #frmEmergencyRequest select').prop('disabled', false);
                        $('#btnSave').show();
                    }
                    medicalProfile.populate(request.userId);
                    console.log(request.userId)
                    $('#emergencyRequestModal').modal('show');
                } else {
                    Notify('danger', response.message);
                }
            },
            error: function (xhr, status, error) {
                // Handle error response
                Notify('danger', error);
            }
        });
    },
    delete: function () {
        debugger;
        $.ajax({
            url: `/Ambulances/Delete/${this.ambulanceId}`,
            type: 'POST',
            success: function (response) {
                if (response.success) {

                    Notify('success', response.message);
                    this.ambulanceId = null;
                    ambulance.initializeGrid();

                } else {
                    Notify('danger', response.message);
                    this.ambulanceId = null;
                }
            },
            error: function (xhr, status, error) {
                // Handle error response
                Notify('danger', error);
            }
        });
        $('#DeleteConfirmationModal').modal('hide');
    },
    clearForm: function () {
        $('#frmAmbulance')[0].reset();
        $('#frmAmbulance select').prop('disabled', false);

        $('#frmambulance').find('.text-danger').text('');
        this.ambulanceId = null;
        $("#btnSave").show();
        $('#imagePreview').hide().attr('src', '#');
        $('#clearImage').hide();
        $('#image').val('');
    },
    setDeleteAmbulanceId: function (ambulanceId) {
        this.ambulanceId = ambulanceId;
    }
};

var contact = {
    save: function () {
        debugger;
        var form = $('#contactForm');
        var formData = new FormData(form[0]);
        var url = '/Contact/Add';

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                debugger;

                if (response.success) {
                    form[0].reset();
                    Notify('success', response.message);
                } else {
                    Notify('danger', response.message);
                }
            },

            error: function (xhr, status, error) {
                debugger;
                Notify('danger', error);
            }
        });
    }
}

var feedback = {
    save: function (e) {
        debugger
        e.preventDefault();
        var form = $('#feedbackForm');
        var formData = new FormData(form[0]);
        var url = '/Feeback/Add';

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,

            success: function (response) {
                if (response.success) {
                    form[0].reset();
                    Notify('success', response.message);
                } else {
                    Notify('danger', response.message);
                }
            },

            error: function (xhr, status, error) {
                debugger;
                Notify('danger', error);
            }
        })
    }
}

var medicalProfile = {
    userId: null,
    save: function () {
        debugger;
        var form = $('#frmUser');
        var formData = new FormData(form[0]);
        var url = '/MedicalProfile/Add';
        if (form.valid()) {
            $.ajax({
                url: url,
                type: 'POST',
                processData: false,  // Important!
                contentType: false,  // Important!
                data: formData,
                success: function (response) {
                    if (response.success) {
                        Notify('success', response.message);
                    } else {
                        Notify('danger', response.message);
                    }
                },
                error: function (xhr, status, error) {
                    debugger;
                    // Handle error response
                    Notify('danger', error);
                }
            });
        }
    },
    populate: function (userId) {
        debugger
        this.userId = userId;
        $.ajax({
            url: `/MedicalProfile/GetById/${userId}`,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    var user = response.data;
                    console.log(user)
                    // Populate form fields
                    $('#medicalHistory').val(user.medicalHistory);
                    $('#allergies').val(user.allergies);
                    $('#emergencyContact').val(user.emergencyContact);
                    console.log($('#emergencyContact').length);
                    console.log(user.emergencyContact);
                } else {
                    Notify('danger', response.message);
                }
            },
            error: function (xhr, status, error) {
                // Handle error response
                Notify('danger', error);
            }
        });
    }
};
// Example usage
//Notify('success', 'Your changes have been saved!',3000);
//Notify('danger', 'An error occurred while saving your changes.',2000);
//Notify('info', 'Here is some information for you.');
//Notify('warning', 'This is a warning message.');