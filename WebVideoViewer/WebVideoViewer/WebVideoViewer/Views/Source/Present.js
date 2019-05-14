function init() {
    var videoTag = document.getElementById("viewer_video");
    var sourceTag = document.getElementById("viewer_source");
    videoTag.addEventListener("error", function(ev) {
        alert("failed viewing video: " + ev.message);
    }, true);
    videoTag.play();
}
document.addEventListener("DOMContentLoaded", init, false);
