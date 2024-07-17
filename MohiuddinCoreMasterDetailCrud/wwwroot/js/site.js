$(document).ready(function () {
    GetCourses();
});

$('#addCourseBtn').click(function () {
    $('#CoursetModalLabel').text("Insert Course");
    $('#save').css('display', 'block');
    $('#update').css('display', 'none');
    $('#CourseId').val(0);
    $('#CoursetModal').modal('show');
});

function Create() {
    var formData = new FormData();
    formData.append('CourseName', $('#CourseName').val());

    $.ajax({
        url: '/Courses/Insert',
        data: formData,
        type: 'POST',
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                GetCourses();
                toastr.success('Course Inserted successfully');
                $('#CoursetModal').modal('hide');
                $('#CoursetModal #CourseName').val('');
            } else {
                toastr.error('Error: ' + response.errors.join(', '));
            }
        },
        error: function () {
            toastr.error('Unable to Insert Data');
        }
    });
}

function Edit(id) {
    $.ajax({
        url: '/Courses/EditCourse',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            if (response.success) {
                $('#CourseId').val(response.data.courseId);
                $('#CourseName').val(response.data.courseName);
                $('#CoursetModalLabel').text("Edit Course");
                $('#save').css('display', 'none');
                $('#update').css('display', 'block');
                $('#CoursetModal').modal('show');
            } else {
                toastr.error('Error: ' + response.message);
            }
        },
        error: function () {
            toastr.error('Unable to Fetch Data');
        }
    });
}

function Update() {
    var formData = new FormData();
    formData.append('CourseId', $('#CourseId').val());
    formData.append('CourseName', $('#CourseName').val());

    $.ajax({
        url: '/Courses/UpdateCourse',
        data: formData,
        type: 'POST',
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                GetCourses();
                toastr.success('Course Updated successfully');
                $('#CoursetModal').modal('hide');
                $('#CoursetModal #CourseName').val('');
            } else {
                toastr.error('Error: ' + (response.errors ? response.errors.join(', ') : response.message));
            }
        },
        error: function () {
            toastr.error('Unable to Update Data');
        }
    });
}

function Delete(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Courses/DeleteCourse',
                type: 'POST',
                data: { id: id },
                success: function (response) {
                    if (response.success) {
                        GetCourses();
                        Swal.fire(
                            'Deleted!',
                            'Course has been deleted.',
                            'success'
                        );
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
                        'Unable to delete the course.',
                        'error'
                    );
                }
            });
        }
    });
}

function GetCourses() {
    $.ajax({
        url: '/Courses/GetCourses',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            var object = '';
            if (!response || response.length === 0) {
                object += '<p class="text-center">Course Not Available</p>';
            } else {
                object += '<table id="courseTable" class="table table-striped table-bordered table-hover">';
                object += '<thead><tr><th>Course ID</th><th>Course Name</th><th>Action</th></tr></thead>';
                object += '<tbody id="courseTblBody">';
                $.each(response, function (index, item) {
                    object += '<tr id="courseRow_' + item.courseId + '">';
                    object += '<td>' + item.courseId + '</td>';
                    object += '<td>' + item.courseName + '</td>';
                    object += '<td><a href="#" class="btn btn-primary" onclick="Edit(' + item.courseId + ')">Edit</a> ';
                    object += '<a href="#" class="btn btn-danger" onclick="Delete(' + item.courseId + ')">Delete</a></td>';
                    object += '</tr>';
                });
                object += '</tbody></table>';
            }

            $('#CourseSection').html(object);

            $('#courseTable').DataTable({
                "paging": true,
                "ordering": true,
                "info": true,
            });
        },
        error: function () {
            toastr.error('Unable to Fetch Data');
        }
    });
}
