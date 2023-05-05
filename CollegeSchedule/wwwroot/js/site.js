function scheduleDropdown() {
    var schedule = document.getElementById("hidden_main_schedule");
    var schedule_btn = document.getElementById("schedule_btn");
    var practice_btn = document.getElementById("practice_btn");
    if (schedule.style.display == "none") {
        schedule.style.display = "block";
        schedule_btn.style.background = "#FAFAFA";
        schedule_btn.style.color = "#4794C6";
        schedule_btn.style.border = "2px solid #4794C6";
        practice_btn.style.marginTop = "15px";
    } else {
        schedule.style.display = "none";
        schedule_btn.style.background = "linear-gradient(87.77deg, #367AA5 1.87%, #4794C6 110.82%)";
        schedule_btn.style.color = "#FAFAFA";
        schedule_btn.style.border = "0px";
        practice_btn.style.marginTop = "0px";  
    }
}

function practiceScheduleDropdown() {
    var practice_schedule = document.getElementById("hidden_practice_schedule");
    var practice_schedule_btn = document.getElementById("practice_schedule_btn");
    var teachers_schedule = document.getElementById("teachers_schedule");
    if (practice_schedule.style.display == "none") {
        practice_schedule.style.display = "block";
        practice_schedule_btn.style.background = "#FAFAFA";
        practice_schedule_btn.style.color = "#4794C6";
        practice_schedule_btn.style.border = "2px solid #4794C6";
        teachers_schedule.style.marginTop = "25px";
    } else {
        practice_schedule.style.display = "none";
        practice_schedule_btn.style.background = "linear-gradient(87.77deg, #367AA5 1.87%, #4794C6 110.82%)";
        practice_schedule_btn.style.color = "#FAFAFA";
        practice_schedule_btn.style.border = "0px";
        teachers_schedule.style.marginTop = "19px";
    }
}

function examsScheduleDropdown() {
    var exams_schedule = document.getElementById("hidden_exams_schedule");
    var exams_schedule_btn = document.getElementById("exams_schedule_btn");
    var teachers_schedule = document.getElementById("teachers_schedule");
    if (exams_schedule.style.display == "none") {
        exams_schedule.style.display = "block";
        exams_schedule_btn.style.background = "#FAFAFA";
        exams_schedule_btn.style.color = "#4794C6";
        exams_schedule_btn.style.border = "2px solid #4794C6";
        teachers_schedule.style.marginTop = "25px";
    } else {
        exams_schedule.style.display = "none";
        exams_schedule_btn.style.background = "linear-gradient(87.77deg, #367AA5 1.87%, #4794C6 110.82%)";
        exams_schedule_btn.style.color = "#FAFAFA";
        exams_schedule_btn.style.border = "0px";
        teachers_schedule.style.marginTop = "19px";
    }
}

function displayDelimiter(id) {
    var singleRoom = document.getElementById("single_room_" + id);
    var delimiterRoom = document.getElementById("delimiter_room_" + id);
    var singleSubject = document.getElementById("single_subject_" + id);
    var delimiterSubject = document.getElementById("delimiter_subject_" + id);
    var singleTeacher = document.getElementById("single_teacher_" + id);
    var delimiterTeacher = document.getElementById("delimiter_teacher_" + id);
    singleRoom.hidden = true;
    delimiterRoom.hidden = false;
    singleSubject.hidden = true;
    delimiterSubject.hidden = false;
    singleTeacher.hidden = true;
    delimiterTeacher.hidden = false;
}

function hiddenDelimiter(id) {
    var singleRoom = document.getElementById("single_room_" + id);
    var delimiterRoom = document.getElementById("delimiter_room_" + id);
    var singleSubject = document.getElementById("single_subject_" + id);
    var delimiterSubject = document.getElementById("delimiter_subject_" + id);
    var singleTeacher = document.getElementById("single_teacher_" + id);
    var delimiterTeacher = document.getElementById("delimiter_teacher_" + id);
    singleRoom.hidden = false;
    delimiterRoom.hidden = true;
    singleSubject.hidden = false;
    delimiterSubject.hidden = true;
    singleTeacher.hidden = false;
    delimiterTeacher.hidden = true;
}

function displayTeacherDelimiter(id) {
    var singleRoom = document.getElementById("single_room_" + id);
    var delimiterRoom = document.getElementById("delimiter_room_" + id);
    var singleTeacher = document.getElementById("single_teacher_" + id);
    var delimiterTeacher = document.getElementById("delimiter_teacher_" + id);
    var addNumerator = document.getElementById("add_numerator_" + id);
    var minusTeacherDelimiter = document.getElementById("minus_teacher_delimiter_" + id);
    minusTeacherDelimiter.hidden = false;
    addNumerator.hidden = true;
    singleRoom.hidden = true;
    delimiterRoom.hidden = false;
    singleTeacher.hidden = true;
    delimiterTeacher.hidden = false;
}

function hiddenTeacherDelimiter(id) {
    var singleRoom = document.getElementById("single_room_" + id);
    var delimiterRoom = document.getElementById("delimiter_room_" + id);
    var singleTeacher = document.getElementById("single_teacher_" + id);
    var delimiterTeacher = document.getElementById("delimiter_teacher_" + id);
    var addNumerator = document.getElementById("add_numerator_" + id);
    var minusTeacherDelimiter = document.getElementById("minus_teacher_delimiter_" + id);
    minusTeacherDelimiter.hidden = true;
    addNumerator.hidden = false;
    singleRoom.hidden = false;
    delimiterRoom.hidden = true;
    singleTeacher.hidden = false;
    delimiterTeacher.hidden = true;
}