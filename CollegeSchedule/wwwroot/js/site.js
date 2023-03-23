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
        teachers_schedule.style.marginTop = "15px";
    } else {
        practice_schedule.style.display = "none";
        practice_schedule_btn.style.background = "linear-gradient(87.77deg, #367AA5 1.87%, #4794C6 110.82%)";
        practice_schedule_btn.style.color = "#FAFAFA";
        practice_schedule_btn.style.border = "0px";
        teachers_schedule.style.marginTop = "0px";
    }
}