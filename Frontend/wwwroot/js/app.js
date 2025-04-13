window.setupGlobalKeydownListener = (textareaId, dotNetObjRef) => {
    document.addEventListener('keydown', (e) => {
        const textarea = document.getElementById(textareaId);
        if (textarea && document.activeElement === textarea) {
            // Check if the Enter key is pressed without Shift or Ctrl
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey) {
                e.preventDefault(); // Prevent default behavior (e.g., adding a new line)
                console.log('Enter key pressed globally');

                // Update the MessageText value in Blazor
                dotNetObjRef.invokeMethodAsync('UpdateMessageText', textarea.value)
                    .then(() => {
                        // Call SendMessage after updating MessageText
                        dotNetObjRef.invokeMethodAsync('SendMessage');
                    });
            }
        }
    });
};
