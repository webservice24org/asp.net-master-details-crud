$(document).ready(function () {

    GetStudents();
    $('#addStudentBtn').click(function () {
        loadCourses();
        $('#StudentModalLabel').text("Insert Student Information");
        $('#saveStudent').show();
        $('#updateStudent').hide();
        resetStudentModal();
        $('#StudentModal').modal('show');
    });

    $(document).on('click', '.editStudentBtn', function () {
        var studentId = $(this).data('id');
        $.ajax({
            url: '/Students/StudentEdit',
            type: 'GET',
            data: { studentId: studentId },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    var student = response.data;
                    $('#StudentId').val(studentId);
                    $('#StudentName').val(student.studentName);
                    var dobDate = new Date(student.dob);
                    var formattedDob = dobDate.getFullYear() + '-' + ('0' + (dobDate.getMonth() + 1)).slice(-2) + '-' + ('0' + dobDate.getDate()).slice(-2);
                    $('#Dob').val(formattedDob);
                    $('#MobileNo').val(student.mobileNo);
                    $('#IsEnrolled').prop('checked', student.isEnrolled);

                    loadCourses(student.courseId, function () {
                        $('#CourseId').val(student.courseId);
                    });

                    if (student.imageUrl) {
                        $('#imageFile').attr('src', '/Images/' + student.imageUrl).show();
                    } else {
                        $('#imageFile').attr('src', '').hide();
                    }

                    var modulesHtml = '';
                    if (student.modules.length > 0) {
                        $.each(student.modules, function (index, module) {
                            addModuleRow(module);
                        });
                    }

                    $('#StudentModalLabel').text('Edit Student');
                    $('#saveStudent').hide();
                    $('#updateStudent').show();
                    $('#StudentModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('Unable to fetch student data.');
            }
        });
    });

    $('#StudentModal').on('hidden.bs.modal', function () {
        resetStudentModal();
    });

    $(document).on('click', '.detailsStudentBtn', function () {
        var studentId = $(this).data('id');
        $.ajax({
            url: '/Students/GetStudentDetails',
            type: 'GET',
            data: { studentId: studentId },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    var student = response.data;
                    $('#detailsModalLabelName').text(student.studentName);
                    var modalBody = '<div class="row">';
                    modalBody += '<div class="col-md-8">';
                    modalBody += '<div class="table-responsive">';
                    modalBody += '<table class="table table-bordered">';
                    modalBody += '<thead>';
                    modalBody += '<tr><th colspan="2">Student Details</th></tr>';
                    modalBody += '</thead>';
                    modalBody += '<tbody>';
                    modalBody += '<tr><td><strong>Student ID</strong></td><td>' + student.studentId + '</td></tr>';
                    modalBody += '<tr><td><strong>Date of Birth</strong></td><td>' + new Date(student.dob).toLocaleDateString() + '</td></tr>';
                    modalBody += '<tr><td><strong>Mobile No</strong></td><td>' + student.mobileNo + '</td></tr>';
                    modalBody += '<tr><td><strong>Enrolled</strong></td><td>' + (student.isEnrolled ? 'Yes' : 'No') + '</td></tr>';
                    modalBody += '<tr><td><strong>Course</strong></td><td>' + student.courseName + '</td></tr>';
                    modalBody += '</tbody>';
                    modalBody += '</table>';
                    modalBody += '</div>';
                    modalBody += '</div>';

                    modalBody += '<div class="col-md-4">';
                    modalBody += '<p><strong>Profile Picture:</strong><br><img src="/Images/' + student.imageUrl + '" alt="' + student.studentName + '" class="mt-2 img-thumbnail"></p>';
                    modalBody += '</div>';
                    modalBody += '</div>';

                    modalBody += '<h5>Modules</h5>';
                    if (student.modules && student.modules.length > 0) {
                        modalBody += '<div class="table-responsive">';
                        modalBody += '<table class="table table-bordered">';
                        modalBody += '<thead>';
                        modalBody += '<tr>';
                        modalBody += '<th>Module Name</th>';
                        modalBody += '<th>Duration (hours)</th>';
                        modalBody += '</tr>';
                        modalBody += '</thead>';
                        modalBody += '<tbody>';
                        $.each(student.modules, function (index, module) {
                            modalBody += '<tr>';
                            modalBody += '<td>' + module.moduleName + '</td>';
                            modalBody += '<td>' + module.duration + '</td>';
                            modalBody += '</tr>';
                        });
                        modalBody += '</tbody>';
                        modalBody += '</table>';
                        modalBody += '</div>';
                    } else {
                        modalBody += '<p>No modules available.</p>';
                    }

                    $('#detailsModal .modal-body').html(modalBody);
                    $('#detailsModal').modal('show');
                } else {
                    toastr.error(response.message);
                }

            },
            error: function () {
                toastr.error('Unable to fetch student details.');
            }
        });
    });

    $(document).on('click', '.removeModuleBtn', function () {
        $(this).closest('tr').remove();
    });

    $('#addModuleBtn').click(function () {
        addModuleRow();
    });

    function addModuleRow(module = null) {
        var moduleIndex = $('#modulesTable tbody tr').length;
        var moduleRow = `
            <tr>
                <td>
                    <input type="hidden" name="Modules[${moduleIndex}].ModuleId" value="${module ? module.moduleId : ''}" />
                    <input type="text" name="Modules[${moduleIndex}].ModuleName" class="form-control" value="${module ? module.moduleName : ''}" />
                </td>
                <td>
                    <input type="text" name="Modules[${moduleIndex}].Duration" class="form-control" value="${module ? module.duration : ''}" />
                </td>
                <td>
                    <button type="button" class="btn btn-danger removeModuleBtn">Remove</button>
                </td>
            </tr>
        `;
        $('#modulesTable tbody').append(moduleRow);
    }

    function loadCourses(selectedCourseId = 0, callback) {
        $.ajax({
            url: '/Students/GetCourses',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                var options = '<option value="0">---Select Course---</option>';
                $.each(response, function (index, item) {
                    options += `<option value="${item.value}" ${item.value == selectedCourseId ? 'selected' : ''}>${item.text}</option>`;
                });
                $('#CourseId').html(options);

                if (callback) {
                    callback();
                }
            },
            error: function () {
                toastr.error('Unable to fetch courses.');
            }
        });
    }
    $('#saveStudent').click(function () {
        saveStudent();
    });

    $('#updateStudent').click(function () {
        saveStudent();
    });

    function saveStudent() {
        var formData = new FormData($('#studentForm')[0]);
        $.ajax({
            url: $('#StudentId').val() == '0' ? '/Students/InsertStudent' : '/Students/UpdateStudent',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    GetStudents();
                    $('#StudentModal').modal('hide');
                    resetStudentModal();
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('Error occurred while saving student.');
            }
        });
    }

  

    $(document).on('click', '.deleteStudentBtn', function () {
        var studentId = $(this).data('id');
        Swal.fire({
            title: 'Are you sure?',
            text: 'You will not be able to recover this student and associated modules!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Students/DeleteStudent',
                    type: 'POST',
                    data: { studentId: studentId },
                    dataType: 'json',
                    success: function (response) {
                        if (response.success) {
                            Swal.fire(
                                'Deleted!',
                                response.message,
                                'success'
                            ).then(() => {
                                GetStudents();
                                toastr.warning("Student data Deleted!")
                            });
                        } else {
                            Swal.fire(
                                'Error!',
                                response.message,
                                'error'
                            );
                        }
                    },
                    error: function () {
                        Swal.fire(
                            'Error!',
                            'Unable to delete student.',
                            'error'
                        );
                    }
                });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                Swal.fire(
                    'Cancelled',
                    'Student deletion cancelled.',
                    'info'
                );
            }
        });
    });

    function GetStudents() {
        $.ajax({
            url: '/Students/GetStudents',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                var object = '';
                if (!response || response.length === 0) {
                    object += '<p class="text-center">No students available</p>';
                } else {
                    object += '<table id="studentTable" class="table table-striped table-bordered table-hover">';
                    object += '<thead><tr><th>Student ID</th><th>Student Name</th><th>Mobile No</th><th>Image</th><th>Action</th></tr></thead>';
                    object += '<tbody id="studentTblBody">';
                    $.each(response, function (index, item) {
                        object += '<tr id="studentRow_' + item.studentId + '">';
                        object += '<td>' + item.studentId + '</td>';
                        object += '<td>' + item.studentName + '</td>';
                        object += '<td>' + item.mobileNo + '</td>';
                        object += '<td><img src="/Images/' + item.imageUrl + '" alt="' + item.studentName + '" style="max-width: 100px; max-height: 100px;"></td>';
                        object += '<td>';
                        object += '<a href="#" class="btn btn-info detailsStudentBtn" data-id="' + item.studentId + '">Details</a> ';
                        object += '<a href="#" class="btn btn-primary editStudentBtn" data-id="' + item.studentId + '">Edit</a> ';
                        object += '<a href="#" class="btn btn-danger deleteStudentBtn" data-id="' + item.studentId + '">Delete</a>';
                        object += '</td>';
                        object += '</tr>';
                    });
                    object += '</tbody></table>';
                }

                $('#studentBody').html(object);

                $('#studentTable').DataTable({
                    "paging": true,
                    "ordering": true,
                    "info": true,
                    "responsive": true,
                    "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
                    "columnDefs": [
                        { "orderable": false, "targets": -1 }
                    ]
                });
            },
            error: function () {
                toastr.error('Unable to fetch student data.');
            }
        });
    }

    function resetStudentModal() {
        $('#studentForm')[0].reset();
        $('#StudentId').val('0');
        $('#CourseId').val('0');
        $('#modulesTable tbody').empty();
        $('#ProfileFile').val('');
        $('#imageFile').attr('src', '').hide();
    }

});

function readUrl(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#imageFile').attr('src', e.target.result).show();
        };
        reader.readAsDataURL(input.files[0]);
    }
}