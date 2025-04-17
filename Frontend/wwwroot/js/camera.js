window.startCamera = (videoElementId) => {
    const video = document.getElementById(videoElementId);
    if (!video) {
        console.error(`Video element with ID '${videoElementId}' not found.`);
        return;
    }

    navigator.mediaDevices.getUserMedia({ video: true })
        .then((stream) => {
            video.srcObject = stream;
            video.play();
        })
        .catch((err) => {
            console.error("Error accessing camera:", err);
        });
};

window.captureFrame = (canvasElement, videoElement) => {
    const canvas = document.getElementById(canvasElement);
    const video = document.getElementById(videoElement);

    if (!canvas || !video) {
        console.error("Canvas or video element not found.");
        return null;
    }

    const ctx = canvas.getContext("2d");
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

    return canvas.toDataURL("image/png").split(",")[1]; // Return Base64 frame
};
